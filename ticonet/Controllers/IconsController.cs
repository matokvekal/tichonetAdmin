using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using log4net;

namespace ticonet.Controllers
{
    public class IconsController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(IconsController));
        // GET: Icons
        public ActionResult Index()
        {
            return null;
        }

        [HttpGet]
        public FileResult StudentIcon(string text)
        {
            int h = 30;
            int w = (int)((double)h * 0.6666666666666666666);
            int mp = 0; //offset marker from top 
            var colFonts = new PrivateFontCollection();
            colFonts.AddFontFile(Server.MapPath("~/fonts/glyphicons-halflings-regular.ttf"));
            colFonts.AddFontFile(Server.MapPath("~/fonts/JournalDingbats2.ttf"));

            var myCustomFont = new Font((FontFamily)colFonts.Families[0], 10f);
            var textFont = new Font(FontFamily.GenericSerif, 8, FontStyle.Bold);
            var codePoint = "E008";

            var code = int.Parse(codePoint, NumberStyles.HexNumber);
            var unicodeString = char.ConvertFromUtf32(code);

            var bm = new Bitmap(w + 1, h + 1, PixelFormat.Format32bppArgb);
            var gr = Graphics.FromImage(bm);
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.InterpolationMode = InterpolationMode.High;
            // gr.DrawRectangle(Pens.DarkGray, 0, 0, w, h);
            gr.TextRenderingHint = TextRenderingHint.AntiAlias;
            //gr.FillEllipse(Brushes.Red, 0, 0, 40, 40);
            //  
            //gr.FillPie(Brushes.Red,0,0,20,20,180,180);
            var points = new List<Point>
            {
                new Point(w/2, h + mp),
                //new Point((int)(w*0.25), (int)(h * 0.66666666666666) + mp),
                new Point(0, (h/3) + mp),
                new Point(w/2, mp),
                new Point(w, (h/3) + mp),
                //new Point((int)(w*0.75), (int)(h * 0.666666666666) + mp),
                new Point(w/2, h + mp)
            };
            gr.FillClosedCurve(Brushes.Red, points.ToArray());
            gr.DrawCurve(Pens.Gray, points.ToArray());
            var n = "LS";
            var sz = gr.MeasureString(n, textFont);
            gr.DrawString(n, textFont, Brushes.Black, (int)((w - sz.Width) / 2), 3);

            var codecs = ImageCodecInfo.GetImageEncoders();
            var j = 0;
            for (j = 0; j < codecs.Length; j++)
            {
                if (codecs[j].MimeType == "image/png") break;
            }
            var ratio = new EncoderParameter(Encoder.Quality, 100L);
            var codecParams = new EncoderParameters(1);
            codecParams.Param[0] = ratio;

            var ms = new MemoryStream();
            bm.Save(ms, codecs[j], codecParams);
            ms.Position = 0;
            Response.ContentType = "image/png";
            return new FileStreamResult(ms, "image/png");
        }

        /// <summary>
        /// Icon for station marker
        /// </summary>
        /// <param name="color">Station color</param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult StationIcon(string color, int type)
        {
            //parsing color
            var clr = color.Replace("#", "");
            var code = int.Parse("FF" + clr, NumberStyles.HexNumber);
            var markerColor = Color.FromArgb(code);
            var maskColorCode = int.Parse("FF222222", NumberStyles.HexNumber);
            //preparing canvas   
            int h = 40;
            int w = (int)((double)h * 0.6666666666666666666);
            int mp = 0; //offset marker from top 
            var bm = new Bitmap(w + 1, h + 1, PixelFormat.Format32bppArgb);
            var gr = Graphics.FromImage(bm);
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.InterpolationMode = InterpolationMode.High;
            gr.TextRenderingHint = TextRenderingHint.AntiAlias;

            //Draw marker
            var path = "~/Content/img/bus-marker-icon.png";
            if (type == 1) path = "~/Content/img/school-marker-icon.png";
            var mask = Image.FromFile(Server.MapPath(path));
            gr.DrawImage(mask, 0, 0, w, h);
            for (int i = 0; i < h; i++)
            {
                for (int k = 0; k < w; k++)
                {
                    var c = bm.GetPixel(k, i);
                    Color  cl =c;
                    if (c.A >200)
                    {
                        cl = Color.FromArgb(c.A,
                            markerColor.R + (((255 - markerColor.R)*c.R)/255),
                            markerColor.G + (((255 - markerColor.G)*c.G)/255),
                            markerColor.B + (((255 - markerColor.B)*c.B)/255));
                    }
                    bm.SetPixel(k, i, cl);
                }
            }
            //Saving image to output stream
            var codecs = ImageCodecInfo.GetImageEncoders();
            var j = 0;
            for (j = 0; j < codecs.Length; j++)
            {
                if (codecs[j].MimeType == "image/png") break;
            }
            var ratio = new EncoderParameter(Encoder.Quality, 100L);
            var codecParams = new EncoderParameters(1);
            codecParams.Param[0] = ratio;

            var ms = new MemoryStream();
            bm.Save(ms, codecs[j], codecParams);
            ms.Position = 0;
            Response.ContentType = "image/png";
            return new FileStreamResult(ms, "image/png");
        }
    }
}