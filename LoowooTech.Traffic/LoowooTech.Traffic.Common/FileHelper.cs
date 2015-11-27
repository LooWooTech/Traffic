using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Traffic.Common
{
    public static class FileHelper
    {
        public static string Open(string Title, string Filter)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Title = Title;
            openfileDialog.Filter = Filter;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                return openfileDialog.FileName;
            }

            return string.Empty;
        }
        public static string Save(string Title, string Filter)
        {
            SaveFileDialog savefileDialog = new SaveFileDialog();
            savefileDialog.Title = Title;
            savefileDialog.Filter = Filter;
            if (savefileDialog.ShowDialog() == DialogResult.OK)
            {
                return savefileDialog.FileName;
            }
            return string.Empty;
        }

        public static IExport ExportBase(string FilePath)
        {
            var ext = System.IO.Path.GetExtension(FilePath);
            IExport export = null;
            switch (ext)
            {
                case ".jpeg":
                    export = new ExportJPEGClass();
                    break;
                case ".bmp":
                    export = new ExportBMPClass();
                    break;
                case ".png":
                    export = new ExportPNGClass();
                    break;
                case ".gif":
                    export = new ExportGIFClass();
                    break;
            }
            return export;
        }
        /// <summary>
        /// 保存当前ActiveView为图片
        /// </summary>
        /// <param name="SaveFilePath">图片路径</param>
        /// <param name="ActiveView">Map ActiveView</param>
        public static void ExportMap(string SaveFilePath,IActiveView ActiveView)
        {
            IExport export = ExportBase(SaveFilePath);
            double IScreenResolution = ActiveView.ScreenDisplay.DisplayTransformation.Resolution;
            export.ExportFileName = SaveFilePath;
            export.Resolution = IScreenResolution;
            ESRI.ArcGIS.esriSystem.tagRECT deviceRECT = ActiveView.ExportFrame;
            IEnvelope envelope = new EnvelopeClass();
            deviceRECT.right = deviceRECT.right * 10;
            deviceRECT.bottom = deviceRECT.bottom * 10;
            envelope.PutCoords(deviceRECT.left, deviceRECT.top, deviceRECT.right, deviceRECT.bottom);
            export.PixelBounds = envelope;
            ITrackCancel Cancel=new  ESRI.ArcGIS.Display.CancelTrackerClass();
            ActiveView.Output(export.StartExporting(), (int)IScreenResolution*10, ref deviceRECT, ActiveView.Extent, Cancel);
            export.FinishExporting();
            export.Cleanup();
            //MessageBox.Show("OK");
        }
    }
}
