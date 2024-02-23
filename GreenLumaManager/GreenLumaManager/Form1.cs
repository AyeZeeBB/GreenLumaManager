using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GreenLumaManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenLumaManager
{
    public partial class Form1 : Form
    {

        string all_appids_json = "";
        JObject obj = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_LoadAsync(object sender, EventArgs e)
        {
            WebClient webClient = new WebClient();
            all_appids_json = webClient.DownloadString($"https://api.steampowered.com/ISteamApps/GetAppList/v2/");
            obj = JObject.Parse(all_appids_json);

            if (string.IsNullOrEmpty(Settings.Default.FolderPath) && Directory.Exists("C:\\Program Files (x86)\\Steam\\AppList\\"))
            {
                folder_path.Text = "C:\\Program Files (x86)\\Steam\\AppList\\";
                Settings.Default.FolderPath = folder_path.Text;
                Settings.Default.Save();
                RefreshListAsync();
            }

            if(!string.IsNullOrEmpty(Settings.Default.FolderPath))
            {
                folder_path.Text = Settings.Default.FolderPath;
                RefreshListAsync();
            }

            if (string.IsNullOrEmpty(Settings.Default.SavedAppIds))
            {
                button4.Text = "AppList Is Enabled!";
                button4.ForeColor = Color.Green;
            }
            else
            {
                button4.Text = "AppList Is Disabled";
                button4.ForeColor = Color.Red;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Create a new instance of FolderBrowserDialog
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            // Show the dialog and capture the result
            DialogResult result = folderBrowserDialog.ShowDialog();

            // Check if the user clicked OK
            if (result == DialogResult.OK)
            {
                folder_path.Text = folderBrowserDialog.SelectedPath;
                Settings.Default.FolderPath = folder_path.Text;
                Settings.Default.Save();
                RefreshListAsync();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RefreshListAsync();
        }

        private void RefreshListAsync()
        {
            if (folder_path.Text == "")
                return;

            if(!Directory.Exists(folder_path.Text))
                return;

            string[] txtFiles = Directory.GetFiles(folder_path.Text, "*.txt");
            List<string> raw_appids = new List<string>();
            List<string> raw_names = new List<string>();

            foreach (string txtFile in txtFiles)
            {
                if (txtFile.Contains("disabled"))
                    continue;

                using (StreamReader reader = new StreamReader(txtFile))
                {
                    // Read and display lines from the file until the end is reached
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        raw_appids.Add(line);
                    }
                }
            }

            AppIds.Lines = raw_appids.ToArray();

            //Add Names To List
            foreach(string appid in raw_appids)
            {
                var appsArray = obj["applist"]["apps"];
                int targetAppId = int.Parse(appid);
                var targetApp = appsArray.FirstOrDefault(app => (int)app["appid"] == targetAppId);

                if (targetApp != null)
                    raw_names.Add((string)targetApp["name"]);
                else
                    raw_names.Add($"Couldnt Get {appid}'s Name");
            }

            richTextBox1.Lines = raw_names.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DeleteOldFiles();

            string[] lines = AppIds.Lines;

            int i = 0;
            foreach (string line in lines)
            {
                using (StreamWriter writer = new StreamWriter(folder_path.Text + $"\\{i}.txt"))
                {
                    writer.WriteLine(line);
                }

                i++;
            }

            RefreshListAsync();
        }

        private void DeleteOldFiles()
        {
            string[] txtFiles = Directory.GetFiles(folder_path.Text, "*.txt");
            foreach (string txtFile in txtFiles)
            {
                File.Delete(txtFile);
            }
        }

        private void folder_path_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.FolderPath = folder_path.Text;
            Settings.Default.Save();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.SavedAppIds))
            {
                DeleteOldFiles();

                string[] lines = AppIds.Lines;
                string finished = "";

                int i = 0;
                foreach(string line in lines)
                {
                    if(i == 0)
                        finished += line;
                    else
                        finished += "," + line;

                    i++;
                }

                Settings.Default.SavedAppIds = finished;
                Settings.Default.Save();

                AppIds.Clear();
                richTextBox1.Clear();
                
                button4.Text = "AppList Is Disabled";
                button4.ForeColor = Color.Red;
            }
            else
            {
                string[] lines = Settings.Default.SavedAppIds.Split(',');

                int i = 0;
                foreach (string line in lines)
                {
                    using (StreamWriter writer = new StreamWriter(folder_path.Text + $"\\{i}.txt"))
                    {
                        writer.WriteLine(line);
                    }

                    i++;
                }

                RefreshListAsync();

                Settings.Default.SavedAppIds = "";
                Settings.Default.Save();

                button4.Text = "AppList Is Enabled!";
                button4.ForeColor = Color.Green;
            }
        }
    }

}
