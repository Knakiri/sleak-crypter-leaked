using System;
using System.Text;
using System.Windows.Forms;

namespace FuckAV
{
    class Program
    {

        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
             Application.Run(new Main());
      
        }
    }
}