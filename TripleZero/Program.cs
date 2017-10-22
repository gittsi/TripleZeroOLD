using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TripleZero.Model;
using Autofac;
using TripleZero.Infrastructure.DI;
using System.Reflection;
using TripleZero.Modules;

namespace TripleZero
{
    class Program
    {
        static Autofac.IContainer autoFacContainer = null;
        private DiscordSocketClient client;
        private IConfiguration _config;
        private IServiceProvider services;
        private CommandService commands;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        

        public async Task MainAsync()
        {
            Console.WriteLine("Hello World!");

            ///////////initialize autofac
            autoFacContainer = AutofacConfig.ConfigureContainer();
            using (var scope = autoFacContainer.BeginLifetimeScope())
            {
                //applicationSettings = scope.Resolve<ApplicationSettings>();
                //gsdgsdg = scope.Resolve<fsdgfsf>();
                scope.Resolve<TripleZeroBot>();
            }

            client = new DiscordSocketClient();
            _config = BuildConfig();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, _config["token"]);
            await client.StartAsync();

            client.UserJoined += UserJoined;

            await Task.Delay(-1);

        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModuleAsync<SomeModule>();
        }

        public async Task UserJoined(SocketGuildUser user)
        {
            var channel = client.GetChannel(370581837560676354) as SocketTextChannel;

            await channel.SendMessageAsync("safsgasgags");
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(string.Concat(Directory.GetCurrentDirectory(),"/config"))
                .AddJsonFile("config.json")
                .Build();
        }

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
                var result = await commands.ExecuteAsync(context, pos, services);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed (not advised for most situations).
                //if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                //    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
