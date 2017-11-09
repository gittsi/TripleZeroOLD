using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Configuration;
using TripleZero.Infrastructure.DI;

namespace TripleZero.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;                

        public HelpModule(CommandService service)
        {
            _service = service;
            //_AppSettings = applicationSettings.Get();
        }

        [Command("info")]
        [Summary("Gets general info")]
        public async Task InfoAsync()
        {
            var applicationSettings = IResolver.Current.ApplicationSettings.Get();

            string prefix = applicationSettings.DiscordSettings.Prefix;
            Version version = Assembly.GetEntryAssembly().GetName().Version;

            string retStr = "";
            retStr += string.Format("\n{0} - {1}", applicationSettings.GeneralSettings.ApplicationName, applicationSettings.GeneralSettings.Environment);
            retStr += string.Format("\nApplication Version : {0}", version);
            retStr += string.Format("\nPrefix : {0}", prefix);

             await ReplyAsync($"{retStr}");
        }


        [Command("help")]
        [Summary("Gets general help")]            
        public async Task HelpAsync()
        {
            
            string prefix = IResolver.Current.ApplicationSettings.Get().DiscordSettings.Prefix;
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use :"                
            };

            //builder.Description += "\n dsagsdgsdg";

            foreach (var module in _service.Modules)
            {
                foreach (var cmd in module.Commands)
                {      
                    if(cmd.Aliases.Count>0 && cmd.Aliases[0]!="help") //dont give help for help command :p
                    {
                        builder.AddField(x =>
                        {
                            x.Name = string.Concat(prefix, string.Join(", ", cmd.Aliases));
                            //x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                            //          $"Summary: {cmd.Summary}";
                            x.Value = $"{cmd.Summary}\n";
                            x.IsInline = false;
                        });
                    }                    
                }
            }


            //foreach (var module in _service.Modules)
            //{
            //    string description = null;
            //    foreach (var cmd in module.Commands)
            //    {
            //        var result = await cmd.CheckPreconditionsAsync(Context);
            //        if (result.IsSuccess)
            //            description += $"{prefix}{cmd.Aliases.First()}\n";
            //    }

            //    if (!string.IsNullOrWhiteSpace(description))
            //    {
            //        builder.AddField(x =>
            //        {
            //            x.Name = module.Name;
            //            x.Value = description;
            //            x.IsInline = false;
            //        });
            //    }
            //}

            await ReplyAsync("", false, builder.Build());
        }

        //[Command("help3")]
        //[Summary("get help3 3425345 3464 346 3464 ")]
        //public async Task HelpAsync3(int xaxa)
        //{
        //    var builder = new EmbedBuilder()
        //    {
        //        Color = new Color(114, 137, 218),
        //        Description = "These are the commands you can use :"
        //    };

        //    builder.AddField(x =>
        //    {
        //        x.Name = string.Join(", ", "ASFASGFASg" );
        //        //x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
        //        //          $"Summary: {cmd.Summary}";
        //        x.Value = "fasf";
        //        x.IsInline = true;                
        //    });

        //    await ReplyAsync("", false, builder.Build());
        //}

        [Command("help")]
        [Summary("Gets helps for specific command")]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            string prefix = IResolver.Current.ApplicationSettings.Get().DiscordSettings.Prefix;
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                if (cmd.Aliases.Count > 0 && cmd.Aliases[0] != "help") //dont give help for help command :p
                {
                    builder.AddField(x =>
                    {
                        x.Name = string.Join(", ", cmd.Aliases);
                        x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                                  $"Summary: {cmd.Summary}";
                        x.IsInline = false;
                    });
                }
                
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
