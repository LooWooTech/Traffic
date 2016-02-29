using ESRI.ArcGIS;
using System;
using System.Windows.Forms;
using LoowooTech.Traffic.TForms;
using ESRI.ArcGIS.esriSystem;

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
            /*
            MessageBox.Show("当前CAD文件中不包含路线信息，请检查。", "注意");
            IAoInitialize ao = null;
            try
            {
                if (!RuntimeManager.Bind(ProductCode.Engine))
                {
                    if (!RuntimeManager.Bind(ProductCode.Desktop))
                    {
                        MessageBox.Show("unable to bind to arcgis runtime.application will be shut down");
                        return;
                    }
                }
                ao = new AoInitializeClass();

                ao.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngine);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "注意");
            }
            var form2 = LoowooTech.Traffic.TForms.TestSuite.TestCase2();
            Application.Run(form2);
            if(ao != null) ao.Shutdown();
            return;*/


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
