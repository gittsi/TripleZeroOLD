using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Autofac;
using TripleZero.Infrastructure.DI;
using TripleZero.Modules;
using TripleZero.Helper;
using TripleZero.Core.Settings;

namespace TripleZero
{
    class Program
    {
        //private DiscordSocketClient _client;

        //public static void Main(string[] args)
        //    => new Program().MainAsync().GetAwaiter().GetResult();

        //public async Task MainAsync()
        //{
        //    _client = new DiscordSocketClient();

        //    _client.Log += Log;
        //    _client.MessageReceived += MessageReceived;

        //    string token = "abcdefg..."; // Remember to keep this private!
        //    await _client.LoginAsync(TokenType.Bot, "Mzc4MDg1OTIwNzM1MzYzMDcz.DOWXlw.UL-EctlIXnBYJux1hijB406_tag");
        //    await _client.StartAsync();

        //    // Block this task until the program is closed.
        //    await Task.Delay(-1);
        //}

        //private async Task MessageReceived(SocketMessage message)
        //{
        //    if (message.Content == "!ping")
        //    {
        //        Task.Delay(5000);
        //        await message.Channel.SendMessageAsync("Pong!");                
        //    }
        //}

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        static Autofac.IContainer autoFacContainer = null;
        static ApplicationSettings applicationSettings = null;
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
                commands = scope.Resolve<CommandService>();
                client = scope.Resolve<DiscordSocketClient>();

                var appSettings = applicationSettings.GetTripleZeroBotSettings();

                await InstallCommands();

                await client.LoginAsync(TokenType.Bot, appSettings.DiscordSettings.Token);
                await client.StartAsync();
                await client.SetGameAsync(string.Format("{0}help", appSettings.DiscordSettings.Prefix));
            }

            //client.MessageReceived += MessageReceived;           


            await Task.Delay(2000);

            Logo.ConsolePrintLogo(); //prints application name,version etc 
            //await TestCharAliasesDelete();
            //await TestDelete();
            //await TestGuildPlayers("41st");
            //await TestPlayerReport("tsitas_66");
            //await TestGuildModule("41s", "gk");
            //await TestCharacterModule("tsitas_66", "cls");
            await client.GetUser("TSiTaS", "1984").SendMessageAsync(Logo.GetLogo());
            await Task.Delay(-1);


        }
        public async Task InstallCommands()
        {
            client.Log += Log;
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
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            var prefix = applicationSettings.GetTripleZeroBotSettings().DiscordSettings.Prefix;
            if (msg.HasCharPrefix(Convert.ToChar(prefix), ref pos) || msg.HasMentionPrefix(client.CurrentUser, ref pos))
            {
                // Create a Command Context.
                var context = new SocketCommandContext(client, msg);

                Consoler.WriteLineInColor(string.Format("User : '{0}' sent the following command : '{1}'", context.Message.Author.ToString(), context.Message.ToString()), ConsoleColor.Green);
                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed succesfully).
                var result = await commands.ExecuteAsync(context, pos, services);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed (not advised for most situations).
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Consoler.WriteLineInColor(string.Format("error  : '{0}' ", result.ErrorReason), ConsoleColor.Green);
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
                }

                if (result.Error == CommandError.UnknownCommand)
                {
                    var message = msg.Channel.SendMessageAsync($"I am pretty sure that there is no command `{msg}`!!!\nTry `{prefix}help` to get an idea!").Result;
                    await Task.Delay(3000);
                    await message.DeleteAsync();
                }
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
