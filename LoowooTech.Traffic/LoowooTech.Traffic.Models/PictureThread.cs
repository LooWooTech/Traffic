using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    public class PictureThread
    {
        public string FilePath { get; set; }
        public string Temp { get; set; }
        public IActiveView ActiveView { get; set; }
        public int DPI { get; set; }
        private string Lenged { get; set; }
        private string Ext { get; set; }

        public PictureThread(string FilePath,IActiveView ActiveView,DataType dataType)
        {
            this.ActiveView = ActiveView;
            this.FilePath = FilePath;
            this.DPI = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DPI"]);
            this.Lenged = GetLegend(dataType);
        }
        public string GetLegend(DataType dataType)
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/" + dataType.ToString() + ".png");
        }
        private IExport ExportBase()
        {
            this.Ext = System.IO.Path.GetExtension(this.FilePath);
            IExport export = null;
            switch (this.Ext)
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
            this.Temp = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp/"+System.IO.Path.GetFileName(this.FilePath));
            IExport export = ExportBase();
            double IScreenResolution = ActiveView.ScreenDisplay.DisplayTransformation.Resolution;
            export.ExportFileName = this.Temp;
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
            Merge();
        }

        public void Merge()
        {
            System.Drawing.Image ImageBackGround = System.Drawing.Image.FromFile(this.Temp);
            System.Drawing.Image ImageLenged = System.Drawing.Image.FromFile(this.Lenged);
            Graphics g = Graphics.FromImage(ImageBackGround);
            //g.DrawImage(ImageBackGround, 0, 0, ImageBackGround.Width, ImageBackGround.Height);
            g.DrawImage(ImageLenged, ImageBackGround.Width-ImageLenged.Width, ImageBackGround.Height-ImageLenged.Height, ImageLenged.Width, ImageLenged.Height);
            g.Dispose();
            switch (this.Ext)
            {
                case ".jpeg":
                    ImageBackGround.Save(this.FilePath, ImageFormat.Jpeg);
                    break;
                case ".bmp":
                    ImageBackGround.Save(this.FilePath, ImageFormat.Bmp);
                    break;
                case ".png":
                    ImageBackGround.Save(this.FilePath, ImageFormat.Png);
                    break;
                case ".gif":
                    ImageBackGround.Save(this.FilePath, ImageFormat.Gif);
                    break;
            }
            
        }
    }
}
