using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Autofac;
using TripleZero.Infrastructure.DI;
using System.Reflection;
using TripleZero.Modules;
using TripleZero.Configuration;
using TripleZero.Helper;

namespace TripleZero
{
    class Program
    {
        static Autofac.IContainer autoFacContainer = null;
        static ApplicationSettings applicationSettings = null;
        static HelpModule helpModule = null;
        static MathModule mathModule = null;
        static TestModule testModule = null;

        private DiscordSocketClient client;        
        private IServiceProvider services;
        private CommandService commands;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();        

        public async Task MainAsync()
        {
            ///////////initialize autofac
            autoFacContainer = AutofacConfig.ConfigureContainer();
            using (var scope = autoFacContainer.BeginLifetimeScope())
            {
                applicationSettings = scope.Resolve<ApplicationSettings>();
                commands = scope.Resolve<CommandService>();                
                client = scope.Resolve<DiscordSocketClient>();


                Logo(); //prints application name,version etc 

                var appSettings = applicationSettings.Get();

                //client = new DiscordSocketClient();
                //_config = BuildConfig();
                //commands = new CommandService();
                //services = new ServiceCollection().BuildServiceProvider();

                await InstallCommands();

                await client.LoginAsync(TokenType.Bot, appSettings.DiscordSettings.Token);
                await client.StartAsync();

                //client.UserJoined += UserJoined;
            }

            
            //client.MessageReceived += MessageReceived;

            await Task.Delay(-1);

        }

        private static void Logo() //prints application name,version etc
        {
            //get application Settings
            var appSettings = applicationSettings.Get();

            Version version = Assembly.GetEntryAssembly().GetName().Version;
            Consoler.WriteLineInColor(string.Format("{0} - {1}", appSettings.GeneralSettings.ApplicationName, appSettings.GeneralSettings.Environment), ConsoleColor.DarkYellow);
            Consoler.WriteLineInColor(string.Format("Application Version : {0}", version), ConsoleColor.DarkYellow);
            Consoler.WriteLineInColor(string.Format("Json Version : {0}", appSettings.GeneralSettings.JsonSettingsVersion), ConsoleColor.DarkYellow);
            Console.Title = string.Format("{0} - version {1}", appSettings.GeneralSettings.ApplicationName, version);
            Console.WriteLine(); Console.WriteLine();
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModuleAsync<MathModule>();
            await commands.AddModuleAsync<HelpModule>();
            await commands.AddModuleAsync<TestModule>();
        }

        //public async Task MessageReceived(SocketGuildUser user)
        //{
        //    var channel = client.GetChannel(370581837560676354) as SocketTextChannel;

        //    await channel.SendMessageAsync("safsgasgags");
        //}

        //public async Task UserJoined(SocketGuildUser user)
        //{
        //    var channel = client.GetChannel(370581837560676354) as SocketTextChannel;

        //    await channel.SendMessageAsync("safsgasgags");
        //}
        

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            // We don't want the bot to respond to itself or other bots.
            // NOTE: Selfbots should invert this first check and remove the second
            // as they should ONLY be allowed to respond to messages from the same account.
            if (msg.Author.Id == client.CurrentUser.Id || msg.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasCharPrefix('^', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
            {
                // Create a Command Context.
                var context = new SocketCommandContext(client, msg);

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed succesfully).
                var result = await commands.ExecuteAsync(context, pos ,services);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed (not advised for most situations).
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
