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
        public Guna2Button dlc_button;
        public Guna2TextBox appid_textbox;
        public Guna2HtmlLabel app_label;
        public Guna2HtmlLabel dlc_label;
        public Guna2HtmlLabel type_label;
        public Guna2PictureBox picture_box;


        public Form1 mainForm = null;

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
            picture_box = guna2PictureBox1;
            dlc_label = dlc_string;
            type_label = guna2HtmlLabel2;
            dlc_button = guna2Button2;
        }

        private void AppIDItem_Load(object sender, EventArgs e)
        {

        }

        private void dlc_string_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(dlc_label.Text))
            { return; }

            DLC dlc = new DLC();
            dlc.Show();
            dlc.mainForm = mainForm;
            dlc.PopulateList(appid_value);
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;
            string text = textBox.Text;

            // Remove any non-numeric characters
            string filteredText = new string(text.Where(char.IsDigit).ToArray());

            // Update the TextBox text if it was modified
            if (filteredText != text)
            {
                textBox.Text = filteredText;

                // Move the cursor to the end of the text
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void guna2TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is a digit or a control key (like backspace or delete)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // Mark the event as handled to prevent the character from being entered
                e.Handled = true;
            }
        }
    }
}
