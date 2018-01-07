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
            //ExecuteCommand(Command.UpdateUnknownGuild, "Order 66 501st Division#@#32#@#order-66-501st-division", null);
            string pathToProgram = "..\\Forms/ConsoleSwGohParser.Core.dll";
            if (File.Exists(pathToProgram))
            {
                string guildname = textGuildName.Text;
                string guildID = textGuildID.Text;
                string guildfixedname = textGuildFixed.Text;

                string argsString = pathToProgram + " " + "UpdateUnknownGuild" + " \"" + guildname + "#@#" + guildID + "#@#" + guildfixedname + "\"";

                var process = Process.Start("dotnet", argsString);
                process.WaitForExit();
                var exitCode = process.ExitCode;
            }
            else
            {
                MessageBox.Show("Cannot Find " + pathToProgram);
            }
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
                        FilterDefinition<QueueDto> filter2 = Builders<QueueDto>.Filter.Eq("Guild", guildname);
                        DeleteResult res2 = collection.DeleteMany(filter2);

                        if (res2.IsAcknowledged)
                        {
                            MessageBox.Show(res2.DeletedCount.ToString () + " Queues Deleted");
                        }
                        else
                        {
                            MessageBox.Show("Result not Acknowledged");
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
    }
}
