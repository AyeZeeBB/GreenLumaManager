using GreenLumaManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GreenLumaManager
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            folder_path.Text = Settings.Default.FolderPath;

            long folderSize = 0;
            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache")))
                folderSize = GetFolderSize(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"));

            guna2Button1.Text = $"Clear Cache: {FormatSize(folderSize)}";
        }

        static long GetFolderSize(string folderPath)
        {
            long size = 0;

            try
            {
                // Get the files in the folder
                string[] files = Directory.GetFiles(folderPath);

                // Calculate the size of each file and sum it up
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    size += fileInfo.Length;
                }

                // Get the subdirectories and calculate their sizes recursively
                string[] subDirectories = Directory.GetDirectories(folderPath);
                foreach (string directory in subDirectories)
                {
                    size += GetFolderSize(directory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return size;
        }

        static string FormatSize(long size)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double sizeInDouble = size;

            while (sizeInDouble >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                sizeInDouble /= 1024;
                suffixIndex++;
            }

            return $"{sizeInDouble:0.##} {suffixes[suffixIndex]}";
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
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache")))
                return;

            string[] files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"));
            foreach( string file in files )
            {
                File.Delete(file);
            }

            long folderSize = GetFolderSize(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"));
            guna2Button1.Text = $"Clear Cache: {FormatSize(folderSize)}";
        }
    }
}
