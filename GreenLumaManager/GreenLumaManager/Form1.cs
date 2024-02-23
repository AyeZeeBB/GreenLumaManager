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
        List<AppIDItem> items = new List<AppIDItem>();

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
                guna2Button4.Text = "AppList Is Enabled!";
                guna2Button4.ForeColor = Color.White;
            }
            else
            {
                guna2Button4.Text = "AppList Is Disabled";
                guna2Button4.ForeColor = Color.Red;
            }
        }

        private void RefreshListAsync()
        {
            if (folder_path.Text == "")
                return;

            if(!Directory.Exists(folder_path.Text))
                return;

            foreach (AppIDItem item in items)
                item.Dispose();

            string[] txtFiles = Directory.GetFiles(folder_path.Text, "*.txt");

            items = new List<AppIDItem>();

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
                        AddAppID(line);
                    }
                }
            }
        }

        private string GetAppLabel(string appid)
        {
            var appsArray = obj["applist"]["apps"];
            int targetAppId = string.IsNullOrEmpty(appid) ? 0 : int.Parse(appid);
            var targetApp = appsArray.FirstOrDefault(app => (int)app["appid"] == targetAppId);

            if (targetApp != null)
                return (string)targetApp["name"];
            else
                return $"Couldnt Get Apps Name";
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

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            AddAppID("730");
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                folder_path.Text = folderBrowserDialog.SelectedPath;
                Settings.Default.FolderPath = folder_path.Text;
                Settings.Default.Save();
                RefreshListAsync();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            DeleteOldFiles();

            int i = 0;
            foreach (AppIDItem item in items)
            {
                using (StreamWriter writer = new StreamWriter(folder_path.Text + $"\\{i}.txt"))
                {
                    writer.WriteLine(item.appid_textbox.Text);
                }

                i++;
            }

            RefreshListAsync();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.SavedAppIds))
            {
                DeleteOldFiles();

                string finished = "";

                int i = 0;
                foreach (AppIDItem item in items)
                {
                    if (i == 0)
                        finished += item.appid_textbox.Text;
                    else
                        finished += "," + item.appid_textbox.Text;

                    i++;
                }

                Settings.Default.SavedAppIds = finished;
                Settings.Default.Save();

                RefreshListAsync();

                guna2Button4.Text = "AppList Is Disabled";
                guna2Button4.ForeColor = Color.Red;
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

                guna2Button4.Text = "AppList Is Enabled!";
                guna2Button4.ForeColor = Color.White;
            }
        }

        public void AddAppID(string _appid)
        {
            AppIDItem appid = new AppIDItem();
            appid.SetAppID(_appid);
            appid.Parent = flowLayoutPanel1;
            appid.appid_textbox.TextChanged += (sender, e) =>
            {
                appid.app_label.Text = GetAppLabel(appid.appid_textbox.Text);
            };
            appid.close_button.Click += (sender, e) =>
            {
                appid.Dispose();
                items.Remove(appid);
            };

            appid.app_label.Text = GetAppLabel(_appid);

            items.Add(appid);
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            Search search = new Search();
            search.mainForm = this;
            search.obj = obj;
            search.Show();
        }
    }

}
