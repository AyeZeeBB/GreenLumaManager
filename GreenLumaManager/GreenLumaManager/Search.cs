using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenLumaManager
{
    public partial class Search : Form
    {
        public Form1 mainForm = null;
        public JObject obj = null;

        List<int> list = new List<int>();

        public Search()
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

        private void PopulateList(string search)
        {
            UpdateUI(() => guna2Button3.Enabled = false);
            UpdateUI(() => guna2Button2.Enabled = false);
            UpdateUI(() => checkedListBox1.Items.Clear());
            list.Clear();

            var appsArray = obj["applist"]["apps"] as JArray;

            Parallel.ForEach(appsArray, app =>
            {
                string appName = Regex.Replace((string)app["name"], "[^a-zA-Z0-9\\s-]", "");
                if (appName.ToLower().Contains(search))
                {
                    int appId = (int)app["appid"];
                    list.Add(appId);
                    checkedListBox1.Invoke((MethodInvoker)delegate {
                        checkedListBox1.Items.Add($"{(int)app["appid"]} : {appName}");
                    });
                }
            });

            search_label.Invoke((MethodInvoker)delegate
            {
                if (list.Count == 0)
                {
                    search_label.ForeColor = Color.Red;
                    search_label.Text = $"Found 0 results";
                }
                else
                {
                    search_label.ForeColor = Color.Green;
                    search_label.Text = $"Found {list.Count} results";
                }
            });

            UpdateUI(() => guna2Button3.Enabled = true);
            UpdateUI(() => guna2Button2.Enabled = true);
            UpdateUI(() => searching.Visible = false);
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (search_text.Text.Length < 3)
            {
                search_label.ForeColor = Color.Red;
                search_label.Text = $"Please enter more then 3 characters";
                return;
            }

            searching.Visible = true;
            Thread newThread = new Thread(() => PopulateList(search_text.Text.ToLower()));
            newThread.Start();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                bool isChecked = checkedListBox1.GetItemChecked(i);
                if(isChecked)
                {
                    mainForm.AddAppID(list[i].ToString());
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }
    }
}
