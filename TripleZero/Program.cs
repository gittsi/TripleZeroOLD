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
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SwGoh;
using MongoDB.Bson.Serialization;
using SWGoH;

namespace TripleZero
{
    class Program
    {
        static Autofac.IContainer autoFacContainer = null;
        static ApplicationSettings applicationSettings = null;
        static MongoDBSettings mongoDBSettings = null;

        private DiscordSocketClient client=null;        
        private IServiceProvider services=null;
        private CommandService commands=null;

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


                Logo(); //prints application name,version etc 

                var appSettings = applicationSettings.Get();                

                await InstallCommands();

                await client.LoginAsync(TokenType.Bot, appSettings.DiscordSettings.Token);
                await client.StartAsync();

                

                //Consoler.WriteLineInColor(client.LoginState.ToString(), ConsoleColor.DarkMagenta);

                //client.UserJoined += UserJoined;
                //Consoler.WriteLineInColor(client.ConnectionState.ToString(), ConsoleColor.DarkMagenta);
            }


            //client.MessageReceived += MessageReceived;

            await Task.Delay(3000);
            //await TestCharAliasesDelete();
            //await TestDelete();
            //await TestGuildPlayers("41st");
            //await TestPlayerReport("tsitas_66");
            //await TestGuildModule("41s", "gk");
            //await TestCharacterModule("tsitas_66", "cls");

            await Task.Delay(-1);

        }

        #region "tests"

        private async Task TestCharAliasesDelete()
        {
            await Task.FromResult(1);

            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

            List<CharacterConfig> charactersConfig = IResolver.Current.CharacterConfig.GetCharactersConfig().Result;
            
            foreach(var characterConfig in charactersConfig)
            {
                if(characterConfig.Aliases.Where(p=> p =="Empty" || p.ToLower()=="true" ).Any())
                {
                    characterConfig.Aliases.Remove("Empty");

                    JObject data = null;
                    try
                    {
                        data = new JObject(
                                               new JProperty("Name", characterConfig.Name),
                                               new JProperty("Command", characterConfig.Command),
                                               new JProperty("SWGoHUrl", characterConfig.SWGoHUrl),
                                               new JProperty("Aliases", characterConfig.Aliases)
                                               );
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(ex.Message);
                    }

                    var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/{0}?apiKey={1}", characterConfig.Id, apiKey);
                    using (HttpClient client1 = new HttpClient())
                    {
                        HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                    }
                }
               
                
            }
            


           
        }

        private async Task TestPlayerDelete()
        {
            using (HttpClient client = new HttpClient())
            {
                var queryData = string.Concat("q={\"PlayerName\":\"", "jonni", "\"}");
                var orderby = "s={\"Date\":1}";
                string apikey = "JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";
                string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player/?{0}&{1}&apiKey={2}", queryData, orderby, apikey);
                var response = await client.GetStringAsync(url);
                List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                var result1 = BsonSerializer.Deserialize<QueuePlayer>(document.FirstOrDefault());

             
                if (result1 != null)
                {
                    var deleteurl = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player/{0}?apiKey={1}", result1.Id, apikey);
                    WebRequest request = WebRequest.Create(deleteurl);
                    request.Method = "DELETE";
                    HttpWebResponse response1 = (HttpWebResponse)request.GetResponse();
                }
            }
        }


        private async Task TestGuildPlayers(string guildAlias)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format("$guildPlayers {0}", guildAlias));
        }
        private async Task TestPlayerReport(string username)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format("$playerreport {0}", username));
        }

        private async Task TestPlayerMods(string username)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format("^mods -speed {0} 10", username));
        }

        private async Task TestGuildModule(string guild, string characterName)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format("^guild {0} {1}", guild, characterName));
        }

        private async Task TestCharacterModule(string userName, string characterName)
        {
            var channel = client.GetChannel(371410170791854101) as SocketTextChannel;

            await channel.SendMessageAsync(string.Format("^ch {0} {1}", userName, characterName));
        }
        #endregion





        private static void Logo() //prints application name,version etc
        {
            //get application Settings
            var appSettings = applicationSettings.Get();

            Version version = Assembly.GetEntryAssembly().GetName().Version;
            Consoler.WriteLineInColor(string.Format("{0} - {1}", appSettings.GeneralSettings.ApplicationName, appSettings.GeneralSettings.Environment), ConsoleColor.DarkYellow);
            Consoler.WriteLineInColor(string.Format("Application Version : {0}", version), ConsoleColor.DarkYellow);
            //Consoler.WriteLineInColor(string.Format("Json Version : {0}", appSettings.GeneralSettings.JsonSettingsVersion), ConsoleColor.DarkYellow);
            Console.Title = string.Format("{0} - version {1}", appSettings.GeneralSettings.ApplicationName, version);
            Console.WriteLine(); Console.WriteLine();
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
