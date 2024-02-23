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
        public Button close_button;
        public Button save_button;

        public AppIDItem()
        {
            InitializeComponent();
        }

        public void SetAppID(string appid, int i)
        {
            textBox1.Text = appid;
            appid_value = appid;
            position = i;
            close_button = button1;
            save_button = button2;
        }

        private void AppIDItem_Load(object sender, EventArgs e)
        {

        }
    }
}
