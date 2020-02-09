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

namespace ProjectRenamer
{
    public partial class Form1 : Form
    {
        private string _folderPath;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            ofdSlnFiles.Filter = @"Sln Files (*.sln) | *.sln;";
            var dialogResult = ofdSlnFiles.ShowDialog();

            if (dialogResult != DialogResult.OK)
                return;

            var fileName = ofdSlnFiles.FileNames[0];
            var indexer = fileName.LastIndexOf("\\", StringComparison.Ordinal);
            _folderPath = fileName.Substring(0, indexer);
            lblFileLocation.Text = _folderPath;

            btnReplace.Enabled = true;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(_folderPath);
            try
            {
                ChangeFileName(directory, txtFindKey.Text, txtReplaceKey.Text);
                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = @"Done";
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = Color.DarkRed;
                lblMessage.Text = @"Fail : " + ex.Message;
            }
        }

        private void ChangeFileName(DirectoryInfo directory, string currentName, string replacementName)
        {
            if (directory.FullName != directory.FullName.Replace(currentName, replacementName))
            {
                Directory.Move(directory.FullName, directory.FullName.Replace(currentName, replacementName));
                directory = new DirectoryInfo(directory.FullName.Replace(currentName, replacementName));
            }

            if (directory.Name == "bin" || directory.Name == "obj")
            {
                DeleteDirectory(directory.FullName);
                return;
            }
                


            FileInfo[] files = directory.GetFiles();

            ChangeFileNameSpace(files, currentName, replacementName);


            var directories = directory.GetDirectories();

            if (directories.Length == 0) return;

            for (int i = 0; i < directories.Length; i++)
            {
                ChangeFileName(directories[i], currentName, replacementName);
            }
        }

        private void ChangeFileNameSpace(FileInfo[] files, string currentName, string replacementName)
        {
            foreach (FileInfo file in files)
            {
                if (file.Extension == ".cs" || file.Extension == ".sln" || file.Extension == ".csproj" || file.Extension == ".config" || file.Extension == ".txt" || file.Extension == ".user" || file.Extension == ".cshtml" || file.Extension == ".asax")
                {
                    string text = File.ReadAllText(file.DirectoryName + "\\" + file.Name);
                    text = text.Replace(currentName, replacementName);
                    File.WriteAllText(file.DirectoryName + "\\" + file.Name, text, Encoding.UTF8);
                }

                if (file.Extension == ".cs" || file.Extension == ".sln" || file.Extension == ".csproj" || file.Extension == ".config" || file.Extension == ".txt" || file.Extension == ".user" || file.Extension == ".cshtml" || file.Extension == ".asax")
                {
                    File.Move(file.DirectoryName + "\\" + file.Name, file.DirectoryName + "\\" + file.Name.Replace(currentName, replacementName));
                }
            }
        }

        public void DeleteDirectory(string targetDir)
        {
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }
    }
}
