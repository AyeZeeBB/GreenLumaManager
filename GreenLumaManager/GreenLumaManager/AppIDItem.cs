using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenLumaManager
{
    public partial class AppIDItem : UserControl
    {
        public string appid_value = "0";
        public int position;
        public Guna2Button close_button;
        public Guna2TextBox appid_textbox;
        public Guna2HtmlLabel app_label;

        public AppIDItem()
        {
            InitializeComponent();
        }

        public void SetAppID(string appid)
        {
            guna2TextBox1.Text = appid;
            appid_value = appid;
            close_button = guna2Button1;
            appid_textbox = guna2TextBox1;
            app_label = guna2HtmlLabel1;
        }

        private void AppIDItem_Load(object sender, EventArgs e)
        {

        }
    }
}
