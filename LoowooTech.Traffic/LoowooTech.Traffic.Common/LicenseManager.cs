using ESRI.ArcGIS.esriSystem;
using System;

namespace LoowooTech.Traffic.Common
{
    public static class LicenseManager
    {
        private static IAoInitialize aoinitialize = new AoInitializeClass();

        public static bool StartUp()
        {
            try
            {
                if (aoinitialize == null)
                {
                    Console.WriteLine("没有安装ARCEngine，系统无法进行");
                    return false;
                }
                ESRI.ArcGIS.esriSystem.esriLicenseStatus licensesStatus = (ESRI.ArcGIS.esriSystem.esriLicenseStatus)aoinitialize.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
                if (licensesStatus == esriLicenseStatus.esriLicenseAvailable)
                {
                    licensesStatus = (esriLicenseStatus)aoinitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
                    if (licensesStatus != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        Console.WriteLine("没有ARCEngine中的GDBEdit许可！");
                        return false;
                    }

                }
                else
                {
                    Console.WriteLine("没有ARCEngine中的GDBEdit许可！");
                    return false;
                }

                licensesStatus = aoinitialize.IsExtensionCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeEngine, esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork);
                licensesStatus = (ESRI.ArcGIS.esriSystem.esriLicenseStatus)aoinitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork);
                if (licensesStatus == esriLicenseStatus.esriLicenseCheckedOut)
                {

                }
                else
                {
                    Console.WriteLine("没有ARCEngine中的NetWork许可！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ArcEngine的许可错误" + ex.Message);
                return false;
            }

            return true;
        }

        public static void ShutDown()
        {
            if (aoinitialize != null)
            {
                aoinitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork);
                aoinitialize.Shutdown();
            }
        }
    }
}
