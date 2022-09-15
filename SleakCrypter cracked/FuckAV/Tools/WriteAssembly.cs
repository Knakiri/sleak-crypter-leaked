using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vestris.ResourceLib;

namespace FuckAV.Tools
{
    internal class WriteAssembly
    {
        public static bool Write(string filename)
        {
            try
            {
                VersionResource versionResource = new VersionResource();
                versionResource.LoadFrom(filename);

                versionResource.FileVersion = Properties.Settings.Default.FileVersion;
                versionResource.ProductVersion = Properties.Settings.Default.ProductVersion;
                versionResource.Language = 0;

                StringFileInfo stringFileInfo = (StringFileInfo)versionResource["StringFileInfo"];

                stringFileInfo["ProductName"] = Properties.Settings.Default.Product;
                stringFileInfo["FileDescription"] = Properties.Settings.Default.Description;
                stringFileInfo["CompanyName"] = Properties.Settings.Default.Company;
                stringFileInfo["LegalCopyright"] = Properties.Settings.Default.Copyright;
                stringFileInfo["LegalTrademarks"] = Properties.Settings.Default.Trademarks;
                stringFileInfo["Assembly Version"] = versionResource.ProductVersion;
                stringFileInfo["InternalName"] = Properties.Settings.Default.OriginalFilename;
                stringFileInfo["OriginalFilename"] = Properties.Settings.Default.OriginalFilename;
                stringFileInfo["ProductVersion"] = versionResource.ProductVersion;
                stringFileInfo["FileVersion"] = versionResource.FileVersion;

                versionResource.SaveTo(filename);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Assembly: " + ex.Message);
                return false;
            }
        }
    }
}

