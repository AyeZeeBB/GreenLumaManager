using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private void PopulateList(string search)
        {
            guna2Button3.Invoke((MethodInvoker)delegate
            {
                guna2Button3.Enabled = false;
            });

            guna2Button2.Invoke((MethodInvoker)delegate
            {
                guna2Button2.Enabled = false;
            });

            checkedListBox1.Invoke((MethodInvoker)delegate {
                checkedListBox1.Items.Clear();
            });

            guna2ProgressBar1.Invoke((MethodInvoker)delegate {
                guna2ProgressBar1.Visible = true;
            });

            list.Clear();

            var appsArray = obj["applist"]["apps"] as JArray;

            foreach (var app in appsArray)
            {
                string appname = (string)app["name"];
                if (!appname.ToLower().Contains(search))
                    continue;

                list.Add((int)app["appid"]);
                checkedListBox1.Invoke((MethodInvoker)delegate {
                    checkedListBox1.Items.Add($"{appname} | ({(int)app["appid"]})");
                });
            }

            guna2Button3.Invoke((MethodInvoker)delegate
            {
                guna2Button3.Enabled = true;
            });

            guna2Button2.Invoke((MethodInvoker)delegate
            {
                guna2Button2.Enabled = true;
            });

            guna2ProgressBar1.Invoke((MethodInvoker)delegate {
                guna2ProgressBar1.Visible = false;
            });
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (search_text.Text.Length < 3)
                return;

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
