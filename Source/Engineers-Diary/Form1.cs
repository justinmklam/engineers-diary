using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Engineers_Diary
{
    public partial class Form1 : Form
    {
        string path_archive = @"Engineers-Diary-Archive";

        public virtual bool ShortcutsEnabled { get; set; }

        public Form1()
        {
            InitializeComponent();
            updateArchiveList();
            txtAuthor.Text = Properties.Settings.Default.author;
        }

        private void txtEntry_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtEntry.Text))
                btnCommit.Enabled = true;
            else
                btnCommit.Enabled = false;
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            List<string> to_write = new List<string>();
            string output;

            if (!Directory.Exists(path_archive))
                Directory.CreateDirectory(path_archive);

            string filename = DateTime.Now.ToString("yyyy-MM-dd");
            string timestamp = DateTime.Now.ToString("h:mm:ss tt");

            string filename_toread = path_archive + "\\" + filename + ".txt";

            if (File.Exists(filename_toread))
            {
                to_write = File.ReadAllLines(filename_toread).ToList();
            }
            else
            {
                to_write.AddRange(constructHeader());
                lstArchive.Items.Insert(0, filename);
            }


            if (!String.IsNullOrEmpty(txtSubject.Text))
                output = "{0} - [{1}] {2}";
            else
                output = "{0} - {2}";

            output = String.Format(output, timestamp, txtSubject.Text, txtEntry.Text);

            //to_write.Add(timestamp + txtSubject.Text + txtEntry.Text);
            to_write.Add(output);
            to_write.Add("");

            filename += ".txt";

            System.IO.File.WriteAllLines(path_archive + "\\" + filename, to_write);

            txtEntry.Clear();
            txtSubject.Clear();
            txtArchiveEntry.Text = System.IO.File.ReadAllText(path_archive + "\\" + filename);
            lstArchive.SelectedIndex = 0;
            txtEntry.Select();
        }

        private List<string> constructHeader()
        {
            List<string> header = new List<string>();
            header.Add("---");
            header.Add("DATE: " + DateTime.Now.ToString("ddd, d MMM yyyy"));
            //header.Add("TIME: " + DateTime.Now.ToString("h:mm:ss tt"));
            header.Add("AUTHOR: " + txtAuthor.Text);
            header.Add("---");
            header.Add("");

            return header;
        }

        private void updateArchiveList()
        {
            DirectoryInfo dinfo = new DirectoryInfo(path_archive);
            try
            {
                FileInfo[] Files = dinfo.GetFiles("*.txt");

                foreach (FileInfo file in Files)
                {
                    lstArchive.Items.Insert(0, Path.GetFileNameWithoutExtension(file.Name));
                }
            }
            catch (DirectoryNotFoundException) { }

        }

        private void lstArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtArchiveEntry.Clear();

            try
            {
                string file_to_read = lstArchive.SelectedItem.ToString() + ".txt";
                //string[] lines = System.IO.File.ReadAllLines(path_archive + "\\" + file_to_read);

                txtArchiveEntry.Text = System.IO.File.ReadAllText(path_archive + "\\" + file_to_read);
            }
            catch (NullReferenceException) { }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.author = txtAuthor.Text;
            Properties.Settings.Default.Save();
        }

        private void txtEntry_KeyDown(object sender, KeyEventArgs e)
        {
            // Simulate ctrl + a
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
                e.SuppressKeyPress = true;
            }
            // Simulate ctrl + backspace
            else if ((e.KeyCode == Keys.Back) && e.Control)
            {
                e.SuppressKeyPress = true;
                int selStart = txtEntry.SelectionStart;
                while (selStart > 0 && txtEntry.Text.Substring(selStart - 1, 1) == " ")
                {
                    selStart--;
                }
                int prevSpacePos = -1;
                if (selStart != 0)
                {
                    prevSpacePos = txtEntry.Text.LastIndexOf(' ', selStart - 1);
                }
                txtEntry.Select(prevSpacePos + 1, txtEntry.SelectionStart - prevSpacePos - 1);
                txtEntry.SelectedText = "";
            }
            else if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnCommit_Click(sender, e);
            }
        }

        private void txtSubject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnCommit_Click(sender, e);
            }
        }
    }

}