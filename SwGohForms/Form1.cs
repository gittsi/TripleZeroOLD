using MongoDB.Bson;
using MongoDB.Driver;
using SWGoH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SwGohForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void butGuildLoad_Click(object sender, EventArgs e)
        {
            string guildname = textGuildName.Text;
            DialogResult ress = MessageBox.Show(this, "Guild LOAD : " + guildname, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (ress == DialogResult.No) return;

            //ExecuteCommand(Command.UpdateUnknownGuild, "Order 66 501st Division#@#32#@#order-66-501st-division", null);
            string pathToProgram = Application.StartupPath + "\\..\\Parser\\ConsoleSwGohParser.Core.dll";
            if (File.Exists(pathToProgram))
            {
                pathToProgram = "\"" + pathToProgram + "\"";

                string guildID = textGuildID.Text;
                string guildfixedname = textGuildFixed.Text;

                string argsString = pathToProgram + " " + "UpdateUnknownGuild" + " \"" + guildname + "#@#" + guildID + "#@#" + guildfixedname + "\"";

                //var process = Process.Start("dotnet", argsString);
                //process.WaitForExit();
                //var exitCode = process.ExitCode;

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = argsString,
                        UseShellExecute = true,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        CreateNoWindow = true
                    }

                };

                process.Start();
            }
            //else
            //{
            //    MessageBox.Show("Cannot Find " + pathToProgram);
            //}
        }

        private void butDelFromQ_Click(object sender, EventArgs e)
        {
            if (Settings.Get())
            {
                string uri = @"mongodb://Dev:dev123qwe@ds" + SWGoH.Settings.appSettings.DatabaseID1.ToString() + ".mlab.com:" + SWGoH.Settings.appSettings.DatabaseID2.ToString() + "/" + SWGoH.Settings.appSettings.Database;
                var client = new MongoClient(uri);

                IMongoDatabase db = client.GetDatabase(SWGoH.Settings.appSettings.Database);
                bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(5000);
                if (isMongoLive)
                {
                    IMongoCollection<QueueDto> collection = db.GetCollection<QueueDto>("Queue");
                    if (collection != null)
                    {
                        string guildname = textDelFromQ.Text;

                        DialogResult ress = MessageBox.Show(this, "Deleting from Queue for guild : " + guildname, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        if (ress == DialogResult.Yes)
                        {
                            FilterDefinition<QueueDto> filter2 = Builders<QueueDto>.Filter.Eq("Guild", guildname);
                            DeleteResult res2 = collection.DeleteMany(filter2);

                            if (res2.IsAcknowledged)
                            {
                                MessageBox.Show(res2.DeletedCount.ToString() + " Queues Deleted");
                            }
                            else
                            {
                                MessageBox.Show("Result not Acknowledged");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Cannot connect to database!!!!!");
                }
            }
            else
            {
                MessageBox.Show("Cannot Load Settings!!!!!");
            }
        }

        private void butDelFromP_Click(object sender, EventArgs e)
        {
            if (Settings.Get())
            {
                string uri = @"mongodb://Dev:dev123qwe@ds" + SWGoH.Settings.appSettings.DatabaseID1.ToString() + ".mlab.com:" + SWGoH.Settings.appSettings.DatabaseID2.ToString() + "/" + SWGoH.Settings.appSettings.Database;
                var client = new MongoClient(uri);

                IMongoDatabase db = client.GetDatabase(SWGoH.Settings.appSettings.Database);
                bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(5000);
                if (isMongoLive)
                {
                    IMongoCollection<PlayerDto> collection = db.GetCollection<PlayerDto>("Player");
                    if (collection != null)
                    {
                        string guildname = textDelFromQ.Text;

                        DialogResult ress = MessageBox.Show(this, "Deleting from Player for guild : " + guildname, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        if (ress == DialogResult.Yes)
                        {
                            FilterDefinition<PlayerDto> filter2 = Builders<PlayerDto>.Filter.Eq("GuildName", guildname);
                            DeleteResult res2 = collection.DeleteMany(filter2);

                            if (res2.IsAcknowledged)
                            {
                                MessageBox.Show(res2.DeletedCount.ToString() + " Players Deleted");
                            }
                            else
                            {
                                MessageBox.Show("Result not Acknowledged");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Cannot connect to database!!!!!");
                }
            }
            else
            {
                MessageBox.Show("Cannot Load Settings!!!!!");
            }
        }

        private List<int> GetZetas(List<PlayerDto> orderedPlayers , IMongoDatabase db, string charactername)
        {
            List<int> zetas = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 };
            CharacterConfigDto characterConfig = null;
            IMongoCollection<CharacterConfigDto> collection2 = db.GetCollection<CharacterConfigDto>("Config.Character");
            if (collection2 != null)
            {
                FilterDefinition<CharacterConfigDto> filter2 = Builders<CharacterConfigDto>.Filter.Eq("Name", charactername);
                List<CharacterConfigDto> results = collection2.Find(filter2).ToList();
                if (results.Count > 0) characterConfig = results[0];
            }
            
            foreach (PlayerDto player in orderedPlayers)
            {
                CharacterDto character = player.Characters.FirstOrDefault(charac => charac.Name == charactername);
                if (character == null) return zetas;
                if (!(character.Level > 0))
                {
                    zetas[0]++;
                    continue;
                }
                int countZeta = 0;
                foreach (var ability in character.Abilities)
                {
                    ConfigAbility configAbility = characterConfig.Abilities?.Where(p => p.Name == ability.Name).FirstOrDefault();
                    if (configAbility?.AbilityType == SWGoH.Enums.ModEnum.ConfigAbilityType.Zeta)
                    {
                        if (ability.Level == ability.MaxLevel)
                        {
                            countZeta += 1;
                        }
                    }
                }
                if (countZeta == 0)
                    zetas[1]++;
                else
                    zetas[countZeta + 1]++;
            }
            return zetas;
        }
        private string PrepareStrMessage(string charactername , List<int> zetas)
        {
            

            int withzeta = 0;
            for (int i = zetas.Count; i > 1; i--)
            {
                int count = i - 2;
                if (zetas[i - 1] > 0 && count > 0) withzeta += zetas[i - 1];
            }

            string ret = charactername + ":("+ withzeta.ToString() + " zeta) ";

            //for (int i = zetas.Count; i > 1; i--)         
            //{
            //    int count = i - 2;
            //    if (count == 0) ret += "(" + "No" + " zeta:" + zetas[i - 1].ToString() + ")";
            //    else if (zetas[i-1] > 0) ret += "(" + count.ToString ()+" zeta : "+ zetas[i-1].ToString () +")";
            //}
            if (zetas[0] > 0) ret += "Locked : " + zetas[0].ToString();
            return ret;
        }
        private void butFullGuildReport_Click(object sender, EventArgs e)
        {
            string guildname = textGuildName.Text;
            DialogResult ress = MessageBox.Show(this, "Full Report for Guild : " + guildname, "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (ress == DialogResult.No) return;

            if (Settings.Get())
            {
                string finalname = Application.StartupPath + "\\FullReport.txt";
                StreamWriter stream = new StreamWriter(finalname, false);
                stream.AutoFlush = false;

                string uri = @"mongodb://Dev:dev123qwe@ds" + SWGoH.Settings.appSettings.DatabaseID1.ToString() + ".mlab.com:" + SWGoH.Settings.appSettings.DatabaseID2.ToString() + "/" + SWGoH.Settings.appSettings.Database;
                var client = new MongoClient(uri);
                IMongoDatabase db = client.GetDatabase(SWGoH.Settings.appSettings.Database);
                bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(5000);
                if (isMongoLive)
                {
                    IMongoCollection<PlayerDto> collection = db.GetCollection<PlayerDto>("Player");
                    if (collection != null)
                    {
                        

                        FilterDefinition<PlayerDto> filter = Builders<PlayerDto>.Filter.Eq("GuildName", guildname);
                        List<PlayerDto> results = collection.Find(filter).ToList();
                        List<PlayerDto> orderedPlayers = results.OrderByDescending(t => t?.Characters?[0]?.Abilities?.Sum(m => m?.Level)).ToList();

                        stream.WriteLine("KGB Report for guild : " + guildname);
                        stream.WriteLine();

                        string charactername = "";
                        List<int> zetas = null;

                        charactername = "Rey (Jedi Training)";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Barriss Offee";
                        zetas = GetZetas(orderedPlayers , db, charactername);
                        stream.WriteLine (PrepareStrMessage(charactername, zetas));

                        charactername = "Commander Luke Skywalker";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "R2-D2";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Grand Admiral Thrawn";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "BB-8";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Darth Vader";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Darth Maul";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Savage Opress";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Mother Talzin";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Wampa";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        charactername = "Hermit Yoda";
                        zetas = GetZetas(orderedPlayers, db, charactername);
                        stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        //charactername = "First Order Executioner";
                        //zetas = GetZetas(orderedPlayers, db, charactername);
                        //stream.WriteLine(PrepareStrMessage(charactername, zetas));

                        stream.Flush();
                        stream.Close();
                        MessageBox.Show("Finished!!!!");
                    }

                }
                else
                {
                    MessageBox.Show("Cannot connect to database!!!!!");
                }
            }
            else
            {
                MessageBox.Show("Cannot Load Settings!!!!!");
            }

        }

        private void GetPlayerZeta()
        {
            
        }
    }
}
