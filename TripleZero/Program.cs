using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Autofac;
using TripleZero.Infrastructure.DI;
using TripleZero.Modules;
using TripleZero.Configuration;
using TripleZero.Helper;
using TripleZero._Mapping;

namespace TripleZero
{
    class Program
    {
        static Autofac.IContainer autoFacContainer = null;
        static ApplicationSettings applicationSettings = null;
        static MongoDBSettings mongoDBSettings = null;
        private DiscordSocketClient client = null;
        private IServiceProvider services = null;
        private CommandService commands = null;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            ///////////initialize autofac
            autoFacContainer = AutofacConfig.ConfigureContainer();
            using (var scope = autoFacContainer.BeginLifetimeScope())
            {
                applicationSettings = scope.Resolve<ApplicationSettings>();
                mongoDBSettings = scope.Resolve<MongoDBSettings>();
                commands = scope.Resolve<CommandService>();
                client = scope.Resolve<DiscordSocketClient>();
                scope.Resolve<IMappingConfiguration>();

                var appSettings = applicationSettings.Get();

                await InstallCommands();

                await client.LoginAsync(TokenType.Bot, appSettings.DiscordSettings.Token);
                await client.StartAsync();
            }


            //client.MessageReceived += MessageReceived;

            await Task.Delay(3000);

            Logo.ConsolePrintLogo(); //prints application name,version etc 
            //await TestCharAliasesDelete();
            //await TestDelete();
            //await TestGuildPlayers("41st");
            //await TestPlayerReport("tsitas_66");
            //await TestGuildModule("41s", "gk");
            //await TestCharacterModule("tsitas_66", "cls");

            await Task.Delay(-1);

        }        
        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModuleAsync<GuildModule>();
            await commands.AddModuleAsync<CharacterModule>();
            await commands.AddModuleAsync<ModsModule>();
            await commands.AddModuleAsync<PlayerModule>();
            await commands.AddModuleAsync<AdminModule>();
            await commands.AddModuleAsync<HelpModule>();
            await commands.AddModuleAsync<FunModule>();
            await commands.AddModuleAsync<DBStatsModule>();
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
            if (msg.Author.Id == client.CurrentUser.Id || msg.Author.IsBot) return;








            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasCharPrefix(Convert.ToChar(applicationSettings.Get().DiscordSettings.Prefix), ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
            {
                // Create a Command Context.
                var context = new SocketCommandContext(client, msg);

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed succesfully).
                var result = await commands.ExecuteAsync(context, pos, services);

                Consoler.WriteLineInColor(string.Format("User : '{0}' sent the following command : '{1}'", context.Message.Author.ToString(), context.Message.ToString()), ConsoleColor.Green);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed (not advised for most situations).
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
        #region "tests"  
        private async Task TestGuildPlayers(string guildAlias)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format(".guildPlayers {0}", guildAlias));
        }
        private async Task TestPlayerReport(string username)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format(".player-report {0}", username));
        }
        private async Task TestPlayerMods(string username)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format(".mods -speed {0} 10", username));
        }
        private async Task TestGuildModule(string guild, string characterName)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format(".guild {0} {1}", guild, characterName));
        }
        private async Task TestCharacterModule(string userName, string characterName)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format(".ch {0} {1}", userName, characterName));
        }
        #endregion
    }
}
