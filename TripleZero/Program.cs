using System;
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
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using TripleZero._Mapping;

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
                var aaaa = scope.Resolve<GuildSettings>();
                commands = scope.Resolve<CommandService>();                
                client = scope.Resolve<DiscordSocketClient>();                
                var aaa = scope.Resolve<IMappingConfiguration>();


                Logo(); //prints application name,version etc 

                var appSettings = applicationSettings.Get();                

                await InstallCommands();

                await client.LoginAsync(TokenType.Bot, appSettings.DiscordSettings.Token);
                await client.StartAsync();

                

                Consoler.WriteLineInColor(client.LoginState.ToString(), ConsoleColor.DarkMagenta);

                //client.UserJoined += UserJoined;
                Consoler.WriteLineInColor(client.ConnectionState.ToString(), ConsoleColor.DarkMagenta);
            }


            //client.MessageReceived += MessageReceived;
            await Task.Delay(2000);
            await TestGuildModule("a", "grievous");

            await Task.Delay(-1);

        }


       

        private async Task TestGuildModule(string guild, string characterName)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync("^guild 53 greedo");
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
            await commands.AddModuleAsync<GuildModule>();
        }

        public async Task MessageReceived(SocketGuildUser user)
        {
            var channel = client.GetChannel(370581837560676354) as SocketTextChannel;

            await channel.SendMessageAsync("safsgasgags");
        }

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








            /////////////////////////////Don't forget to exclude bots///////////////////////
            //if (msg.Author.Id == client.CurrentUser.Id || msg.Author.IsBot) return;






            

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

                Consoler.WriteLineInColor(string.Format("User : '{0}' sent the following command : '{1}'", context.Message.Author.ToString(), context.Message.ToString()),ConsoleColor.Green);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed (not advised for most situations).
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
