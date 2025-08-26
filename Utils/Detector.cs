using PaddleOCRJson;
using System.Drawing.Drawing2D;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
    public class Detector
    {
        private const float IMAGE_SCALE = 3.0f;
        private const int OCR_DPI = 300;
        private const string ENGINE_PATH = "Extras/RapidOCR/RapidOCR-json.exe";

        private static readonly OcrEngine _engine;
        private static readonly OcrClient _client;

        static Detector()
        {
            var startupArgs = OcrEngineStartupArgs
                .WithPipeMode(ENGINE_PATH)
                .EnableMkldnn(true);

            _engine = new OcrEngine(startupArgs);
            _client = _engine.CreateClient();
        }

        public static Rectangle? CaptureArea(IntPtr _processhWnd)
        {
            if (_processhWnd == IntPtr.Zero)
            {
                MessageBox.Show("Process window not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var rect = Hook.GetWindowRectangle(_processhWnd);
            var height = rect.Bottom - rect.Top;
            var width = rect.Right - rect.Left;

            Rectangle processRect = new Rectangle(rect.Left, rect.Top, width, height);
            using (var overlay = new FrmScreenSelectionOverlay(processRect))
            {
                if (overlay.ShowDialog() == DialogResult.OK)
                {
                    Rectangle selectedRect = overlay.SelectedRectangle;
                    if (selectedRect.Width > 0 && selectedRect.Height > 0)
                    {
                        return selectedRect;
                    }
                }

                return null;
            }
        }

        public static Bitmap CaptureScreen(Rectangle region)
        {
            Bitmap bitmap = new Bitmap(region.Width, region.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(region.X, region.Y, 0, 0, region.Size);
            }
            return bitmap;
        }

        public static Bitmap ScaleImage(Bitmap _image)
        {
            int newWidth = (int)(_image.Width * IMAGE_SCALE);
            int newHeight = (int)(_image.Height * IMAGE_SCALE);

            Bitmap resized = new Bitmap(newWidth, newHeight);
            resized.SetResolution(OCR_DPI, OCR_DPI);

            using (Graphics g = Graphics.FromImage(resized))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var destRect = new Rectangle(0, 0, newWidth, newHeight);
                g.DrawImage(_image, destRect, 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel);
            }

            return resized;
        }

        public static OcrResult? RunOCR(Bitmap _bitmap)
        {
            byte[] imageBytes = _bitmap.ToByteArray();
            var ret = _client.FromImageBytes(imageBytes).JsonToData<OcrResult>();
            return ret;
        }

        public static string DetectText(Rectangle _captureArea)
        {
            using var capturedImage = CaptureScreen(_captureArea);
            using var scaledImage = ScaleImage(capturedImage);
            var result = RunOCR(scaledImage);
            return result?.OcrData?.OrderByDescending(x => x.Score).FirstOrDefault()?.Text ?? string.Empty;
        }

        public static void Dispose()
        {
            _client.Dispose();
            _engine.Dispose();
        }
    }
}
