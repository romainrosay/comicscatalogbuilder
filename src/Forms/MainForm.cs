using ComicsCatalog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComicsCatalogWin
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            txtInput.Text = Properties.Settings.Default.inputFolder;
            txtOutput.Text = Properties.Settings.Default.outputFile;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtInput.Text == null || txtOutput.Text == null)
                {
                    MessageBox.Show("Please fill input and outpout fields !");
                    return;
                }

                lockInterface();
                Task.Run(() => GenerateCatalog(txtInput.Text, txtOutput.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally {
            }
        }

        private void GenerateCatalog(string input, string output) {
            Catalog catalog = new Catalog();
            catalog.NewStatusSent += Catalog_NewStatusSent;
            catalog.BuildCatalog(input, output);
            MessageBox.Show("Catalog created successfully");
            if (InvokeRequired) Invoke(new MethodInvoker(unlockInterface));
            else unlockInterface();
        }

        private void lockInterface() {
            btnGenerate.Enabled = false;
            btnInput.Enabled = false;
            btnOutput.Enabled = false;
            txtInput.Enabled = false;
            txtOutput.Enabled = false;
        }

        private void unlockInterface()
        {
            btnGenerate.Text = "Generate";
            btnGenerate.Enabled = true;
            btnInput.Enabled = true;
            btnOutput.Enabled = true;
            txtInput.Enabled = true;
            txtOutput.Enabled = true;
        }

        private void Catalog_NewStatusSent(object sender, NewStatusEventArgs e)
        {
            Invoke(new MethodInvoker(()=> {
                btnGenerate.Text = e.Message;
            }));
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                Properties.Settings.Default.inputFolder = folderBrowserDialog2.SelectedPath;
                txtInput.Text = folderBrowserDialog2.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            saveFileDialog1.Filter = "cbz files (*.cbz)|*.cbz";
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                if (!filePath.ToLower().EndsWith(".cbz")) filePath += ".cbz";
                Properties.Settings.Default.outputFile = filePath;
                txtOutput.Text = filePath;
                Properties.Settings.Default.Save();
            }
        }

    }
}
