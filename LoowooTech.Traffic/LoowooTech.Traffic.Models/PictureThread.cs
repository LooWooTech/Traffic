using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    public class PictureThread
    {
        public string FilePath { get; set; }
        public IActiveView ActiveView { get; set; }
        public int DPI { get; set; }

        public PictureThread(string FilePath,IActiveView ActiveView)
        {
            this.ActiveView = ActiveView;
            this.FilePath = FilePath;
            this.DPI = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DPI"]);
        }
        private IExport ExportBase()
        {
            var ext = System.IO.Path.GetExtension(this.FilePath);
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

        public void ThreadMain()
        {
            IExport export = ExportBase();
            double IScreenResolution = ActiveView.ScreenDisplay.DisplayTransformation.Resolution;
            export.ExportFileName = this.FilePath;
            export.Resolution = IScreenResolution;
            ESRI.ArcGIS.esriSystem.tagRECT deviceRECT = ActiveView.ExportFrame;
            IEnvelope envelope = new EnvelopeClass();
            deviceRECT.right = deviceRECT.right * DPI;
            deviceRECT.bottom = deviceRECT.bottom * DPI;
            envelope.PutCoords(deviceRECT.left, deviceRECT.top, deviceRECT.right, deviceRECT.bottom);
            export.PixelBounds = envelope;
            ITrackCancel Cancel = new ESRI.ArcGIS.Display.CancelTrackerClass();
            ActiveView.Output(export.StartExporting(), (int)IScreenResolution * DPI, ref deviceRECT, ActiveView.Extent, Cancel);
            export.FinishExporting();
            export.Cleanup();
        }
    }
}
