using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenLumaManager
{
    public partial class DLC : Form
    {
        public Form1 mainForm = null;

        List<int> list = new List<int>();

        public DLC()
        {
            InitializeComponent();
        }

        private void Search_Load(object sender, EventArgs e)
        {
            
        }

        void UpdateUI(Action action)
        {
            if (action == null) return;
            Invoke((MethodInvoker)delegate { action(); });
        }

        public void PopulateList(string app_id)
        {
            guna2Button2.Enabled = false;
            checkedListBox1.Items.Clear();
            list.Clear();

            WebClient webClient = new WebClient();
            string appid_json = webClient.DownloadString($"https://store.steampowered.com/api/appdetails?appids={app_id}");
            JObject _app = JObject.Parse(appid_json);

            var dlcArray = _app[app_id]["data"]["dlc"] as JArray;

            foreach( var dlc in dlcArray)
            {
                var appsArray = mainForm.obj["applist"]["apps"];
                var targetApp = appsArray.FirstOrDefault(app => (int)app["appid"] == (int)dlc);

                string DLC_NAME = "";
                if (targetApp != null)
                    DLC_NAME = Regex.Replace((string)targetApp["name"], "[^a-zA-Z0-9\\s-]", "");

                string appName = Regex.Replace(DLC_NAME, "[^a-zA-Z0-9\\s-]", ""); ;
                list.Add((int)dlc);
                checkedListBox1.Items.Add($"{(int)dlc} : {appName}");
            }

            guna2Button2.Enabled = true;
        }

        private async void guna2Button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                bool isChecked = checkedListBox1.GetItemChecked(i);
                if(isChecked)
                {
                    await mainForm.AddAppID(list[i].ToString());
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }
    }
}
