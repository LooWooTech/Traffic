using ESRI.ArcGIS;
using LoowooTech.Traffic.Common;
using LoowooTech.Traffic.TForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
          //  ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
           // LicenseManager.StartUp();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
           // LicenseManager.ShutDown();
        }
    }
}
