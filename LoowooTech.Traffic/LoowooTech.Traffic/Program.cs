using ESRI.ArcGIS;
using System;
using System.Windows.Forms;
using LoowooTech.Traffic.TForms;

namespace LoowooTech.Traffic
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            if (!RuntimeManager.Bind(ProductCode.Engine))
            {
                if (!RuntimeManager.Bind(ProductCode.Desktop))
                {
                    MessageBox.Show("unable to bind to arcgis runtime.application will be shut down");
                    return;
                }
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var splash = new SplashForm();
            splash.Show();
            
            var form = new MainForm();
            splash.Form = form;
            form.Splash = splash;
            Application.Run(form);
        }
    }
}
