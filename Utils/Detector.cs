using OpenCvSharp;
using OpenCvSharp.Extensions;
using PaddleOCRJson;
using System.Drawing.Drawing2D;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
    public class Detector
    {
        private const float IMAGE_SCALE = 3.0f;
        private const int OCR_DPI = 300;
        private const string ENGINE_PATH_PADDLE = "Extras/PaddleOCR/PaddleOCR-json.exe";
        private const string ENGINE_PATH_RAPID = "Extras/RapidOCR/RapidOCR-json.exe";

        private static readonly OcrEngine _engine;
        private static readonly OcrClient _client;

        static Detector()
        {
            var isRapid = bool.Parse(Helper.GetConfigValue("UseRapidOCR", "False"));
            string enginePath = ENGINE_PATH_PADDLE;

            if (isRapid)
            {
                enginePath = ENGINE_PATH_RAPID;
            }

            var startupArgs = OcrEngineStartupArgs
                .WithPipeMode(enginePath)
                .CpuThreads(1)
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

        public static OcrResult? RunOCR(Bitmap _bitmap)
        {
            byte[] imageBytes = _bitmap.ToByteArray();
            var ret = _client.FromImageBytes(imageBytes).JsonToData<OcrResult>();
            return ret;
        }

        public static string DetectText(Rectangle _captureArea)
        {
            using var capturedImage = CaptureScreen(_captureArea);
            using var scaled = Resize(capturedImage);
            using var norm = Normalization(scaled);
            using var gray = ToGray(norm);
            using var denoise = Denoise(gray);
            using var binary = Threshold(denoise);

            var result = RunOCR(scaled);
            return string.Join(" ", result?.OcrData?.OrderByDescending(x => x.Score).Select(x => x.Text) ?? []);
        }

        public static void Dispose()
        {
            _client.Dispose();
            _engine.Dispose();
        }

        public static Mat BitmapToMat(Bitmap bmp) => BitmapConverter.ToMat(bmp);

        public static Bitmap MatToBitmap(Mat mat) => BitmapConverter.ToBitmap(mat);

        public static Bitmap Normalization(Bitmap bmp)
        {
            using var mat = BitmapToMat(bmp);
            using var normalized = new Mat();
            Cv2.Normalize(mat, normalized, 0, 255, NormTypes.MinMax);
            return MatToBitmap(normalized);
        }

        public static Bitmap ToGray(Bitmap bmp)
        {
            using var mat = BitmapToMat(bmp);
            if (mat.Channels() == 1)
                return MatToBitmap(mat);

            using var gray = new Mat();
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
            return MatToBitmap(gray);
        }

        public static Bitmap Threshold(Bitmap bmp)
        {
            using var mat = BitmapToMat(bmp);
            Mat gray;
            if (mat.Channels() == 1)
                gray = mat.Clone();
            else
            {
                gray = new Mat();
                Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
            }

            using (gray)
            {
                using var bin = new Mat();
                Cv2.Threshold(gray, bin, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                return MatToBitmap(bin);
            }
        }

        public static Bitmap Denoise(Bitmap bmp)
        {
            using var mat = BitmapToMat(bmp);
            using var dst = new Mat();

            if (mat.Channels() == 1)
                Cv2.FastNlMeansDenoising(mat, dst);
            else
                Cv2.FastNlMeansDenoisingColored(mat, dst);

            return MatToBitmap(dst);
        }

        public static Bitmap Resize(Bitmap bmp, double scale = 1.5)
        {
            int width = (int)(bmp.Width * scale);
            int height = (int)(bmp.Height * scale);

            using var mat = BitmapToMat(bmp);
            using var resized = new Mat();
            Cv2.Resize(mat, resized, new OpenCvSharp.Size(width, height), 0, 0, InterpolationFlags.Area);
            return MatToBitmap(resized);
        }

        public static Bitmap Thinning(Bitmap bmp)
        {
            using var mat = BitmapToMat(bmp);
            Mat gray;
            if (mat.Channels() == 1)
                gray = mat.Clone();
            else
            {
                gray = new Mat();
                Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
            }

            using (gray)
            {
                using var bin = new Mat();
                Cv2.Threshold(gray, bin, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

                var skeleton = new Mat(bin.Size(), MatType.CV_8UC1, Scalar.Black);
                var element = Cv2.GetStructuringElement(MorphShapes.Cross, new OpenCvSharp.Size(3, 3));

                var temp = new Mat();
                var eroded = new Mat();
                var working = bin.Clone();

                bool done;
                do
                {
                    Cv2.Erode(working, eroded, element);
                    Cv2.Dilate(eroded, temp, element);
                    Cv2.Subtract(working, temp, temp);
                    Cv2.BitwiseOr(skeleton, temp, skeleton);
                    eroded.CopyTo(working);
                    done = (Cv2.CountNonZero(working) == 0);
                } while (!done);

                return MatToBitmap(skeleton);
            }
        }

        public static Bitmap Deskew(Bitmap bmp)
        {
            using var mat = BitmapToMat(bmp);
            Mat gray;
            if (mat.Channels() == 1)
                gray = mat.Clone();
            else
            {
                gray = new Mat();
                Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
            }

            using (gray)
            {
                using var bin = gray.Threshold(0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                using var edges = bin.Canny(50, 200);

                var lines = Cv2.HoughLines(edges, 1, Math.PI / 180, 100);
                double angle = 0;
                int count = 0;
                foreach (var line in lines)
                {
                    angle += line.Theta;
                    count++;
                }

                if (count > 0)
                    angle /= count;

                double angleDeg = (angle * 180 / Math.PI) - 90;
                var center = new Point2f(mat.Width / 2f, mat.Height / 2f);
                var rotMat = Cv2.GetRotationMatrix2D(center, angleDeg, 1.0);

                var rotated = new Mat();
                Cv2.WarpAffine(mat, rotated, rotMat, mat.Size(),
                    InterpolationFlags.Linear, BorderTypes.Constant, Scalar.White);

                return MatToBitmap(rotated);
            }
        }

        private static void SaveBitmap(Bitmap bmp, string path)
        {
            bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }

    }
}
