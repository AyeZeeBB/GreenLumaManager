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
using System.Text.RegularExpressions;
using Guna.UI2.WinForms.Suite;

namespace GreenLumaManager
{
    public partial class Form1 : Form
    {
        List<AppIDItem> items = new List<AppIDItem>();

        string all_appids_json = "";
        public JObject obj = null;
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
                Settings.Default.FolderPath = "C:\\Program Files (x86)\\Steam\\AppList\\";
                Settings.Default.Save();
                RefreshListAsync();
            }

            if(!string.IsNullOrEmpty(Settings.Default.FolderPath))
            {
                RefreshListAsync();
            }
            else
            {
                MessageBox.Show("Please select the path to your Greenlumas AppList Folder");

                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    DialogResult result = folderBrowserDialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        string selectedFolder = folderBrowserDialog.SelectedPath;
                        Settings.Default.FolderPath = selectedFolder;
                        Settings.Default.Save();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }

                RefreshListAsync();
            }


            guna2Button4.Text = string.IsNullOrEmpty(Settings.Default.SavedAppIds) ? "AppList Is Enabled!" : "AppList Is Disabled";
            guna2Button4.ForeColor = string.IsNullOrEmpty(Settings.Default.SavedAppIds) ? Color.White : Color.Red;
        }

        private async void RefreshListAsync()
        {
            if (Settings.Default.FolderPath == "")
                return;

            if(!Directory.Exists(Settings.Default.FolderPath))
                return;

            foreach (AppIDItem item in items)
                item.Dispose();

            string[] txtFiles = Directory.GetFiles(Settings.Default.FolderPath, "*.txt");

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
                        await AddAppID(line);
                    }
                }
            }
        }

        private string GetAppLabel(string appid)
        {
            appid = Regex.Replace(appid, "[^0-9]", "");

            var appsArray = obj["applist"]["apps"];
            int targetAppId = string.IsNullOrEmpty(appid) ? 0 : int.Parse(appid);
            var targetApp = appsArray.FirstOrDefault(app => (int)app["appid"] == targetAppId);

            if (targetApp != null)
                return Regex.Replace((string)targetApp["name"], "[^a-zA-Z0-9\\s-]", "");
            else
                return $"Couldnt Get Apps Name";
        }

        private async Task<List<string>> GetAppInfo(string appId)
        {
            appId = Regex.Replace(appId, "[^0-9]", "");

            if (appId == "0" || string.IsNullOrEmpty(appId))
                appId = "1";

            string cacheFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            if (!Directory.Exists(cacheFolderPath))
            {
                Directory.CreateDirectory(cacheFolderPath);
            }

            string imagePath = Path.Combine(cacheFolderPath, $"{appId}.jpg");

            JObject jsonResponse = null;

            if (!File.Exists(imagePath))
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync($"https://store.steampowered.com/api/appdetails?appids={appId}");
                    jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());

                    if (!bool.TryParse(jsonResponse[appId]?["success"]?.ToString(), out bool success) || !success)
                        return new List<string>() { "", "", "" };

                    string imageUrl = jsonResponse[appId]["data"]["capsule_image"]?.ToString();
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Download the image
                        byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                        // Save the image to the cache folder
                        File.WriteAllBytes(imagePath, imageBytes);
                    }
                }
            }
            else
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync($"https://store.steampowered.com/api/appdetails?appids={appId}");
                    jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());

                    if (!bool.TryParse(jsonResponse[appId]?["success"]?.ToString(), out bool success) || !success)
                        return new List<string>() { "", "", "" };
                }
            }

            var dlcCount = jsonResponse[appId]?["data"]?["dlc"]?.Count() ?? 0;

            return new List<string>
            {
                jsonResponse[appId]["data"]["type"]?.ToString(),
                $"DLC Count: {dlcCount}",
                imagePath
            };
        }

        private void DeleteOldFiles()
        {
            string[] txtFiles = Directory.GetFiles(Settings.Default.FolderPath, "*.txt");
            foreach (string txtFile in txtFiles)
            {
                File.Delete(txtFile);
            }
        }

        private void folder_path_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.FolderPath = Settings.Default.FolderPath;
            Settings.Default.Save();
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            await AddAppID("730");
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                Settings.Default.FolderPath = folderBrowserDialog.SelectedPath;
                Settings.Default.FolderPath = Settings.Default.FolderPath;
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
                using (StreamWriter writer = new StreamWriter(Settings.Default.FolderPath + $"\\{i}.txt"))
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
                    using (StreamWriter writer = new StreamWriter(Settings.Default.FolderPath + $"\\{i}.txt"))
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

        bool wasGreaterThan7 = false;

        public async Task AddAppID(string _appid)
        {
            AppIDItem appid = new AppIDItem();
            appid.Size = items.Count + 1 < 7 ? new Size(1016, 65) : new Size(992, 65);
            appid.SetAppID(_appid);
            appid.mainForm = this;
            appid.Parent = flowLayoutPanel1;
            appid.appid_textbox.TextChanged += async (sender, e) =>
            {
                appid.appid_value = appid.appid_textbox.Text;
                appid.app_label.Text = GetAppLabel(appid.appid_textbox.Text);

                List<string> list2 = await GetAppInfo(appid.appid_textbox.Text);
                appid.picture_box.ImageLocation = list2[2];
                appid.dlc_label.Text = list2[1];
                appid.type_label.Text = list2[0].ToUpper();
                appid.dlc_button.Visible = !string.IsNullOrEmpty(list2[1]);
            };
            appid.close_button.Click += (sender, e) =>
            {
                appid.Dispose();
                items.Remove(appid);
                FixAllItemsWidth();
            };

            appid.app_label.Text = GetAppLabel(_appid);

            List<string> list = await GetAppInfo(_appid);
            appid.picture_box.ImageLocation = list[2];
            appid.dlc_label.Text = list[1];
            appid.type_label.Text = list[0].ToUpper();
            appid.dlc_button.Visible = !string.IsNullOrEmpty(list[1]);

            items.Add(appid);

            FixAllItemsWidth();
        }

        private void FixAllItemsWidth()
        {
            if (items.Count >= 7 && !wasGreaterThan7)
                wasGreaterThan7 = true;
            else if (items.Count < 7 && wasGreaterThan7)
                wasGreaterThan7 = false;
            else
                return;

            Size new_size = items.Count < 7 ? new Size(1016, 65) : new Size(992, 65);
            Size panel_size = items.Count < 7 ? new Size(1015, 32) : new Size(991, 32);
            guna2Panel2.Size = panel_size;

            foreach (AppIDItem item in items)
            {
                item.Size = new_size;
            }
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
