using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace UmatoMusume.Utils
{
    public class Detector
    {
        private const float IMAGE_SCALE = 3.0f;
        private const int OCR_DPI = 300;

        private const double GRAYSCALE_WEIGHT_R = 0.299;
        private const double GRAYSCALE_WEIGHT_G = 0.587;
        private const double GRAYSCALE_WEIGHT_B = 0.114;

        private const int REMOVE_NOISE_THRESHOLD = 162;
        private const int REMOVE_SMALL_NOISE_PIXEL_THRESHOLD = 128;
        private const int REMOVE_SMALL_NOISE_MIN_NEIGHBORS = 2;
        private const int REMOVE_SMALL_NOISE_WHITE_NEIGHBORS = 1;
        private const int PREPROCESS_AVG = 3;
        private const int PREPROCESS_BRIGHTNESS_THRESHOLD = 200;
        private const int OTSU_HISTOGRAM_SIZE = 256;
        private const int SHARPEN_FILTER_SIZE = 3;
        private const int SHARPEN_FILTER_CENTER = 9;
        private const int SHARPEN_FILTER_EDGE = -1;
        private const double SHARPEN_FACTOR = 1.0;
        private const double SHARPEN_BIAS = 0.0;
        private const int COLOR_CHANNELS = 3;
        private const int PIXEL_MIN = 0;
        private const int PIXEL_MAX = 255;
        private const string OCR_DPI_STRING = "300";
        private const string OCR_CHAR_WHITELIST = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 .,-!?:()[]{}♪☆";
        private const string OCR_PAGE_SEG_MODE = "7";

        private static readonly float[][] COLOR_MATRIX = new float[][]
        {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        };

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

        public static Bitmap GrayScale(Bitmap _image)
        {
            Bitmap newBitmap = new Bitmap(_image.Width, _image.Height);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix(COLOR_MATRIX);

                using ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height), 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel, attributes);
            }
            return newBitmap;
        }

        public static Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap grayImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int gray = (int)(pixel.R * GRAYSCALE_WEIGHT_R + pixel.G * GRAYSCALE_WEIGHT_G + pixel.B * GRAYSCALE_WEIGHT_B);
                    Color grayColor = Color.FromArgb(gray, gray, gray);
                    grayImage.SetPixel(x, y, grayColor);
                }
            }

            return grayImage;
        }

        public static Bitmap SimpleThreshold(Bitmap image, int threshold)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int brightness = pixel.R;
                    if (brightness < threshold)
                    {
                        result.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.White);
                    }
                }
            }

            return result;
        }

        public static Bitmap InvertedSimpleThreshold(Bitmap image, int threshold)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int brightness = pixel.R;
                    if (brightness > threshold)
                    {
                        result.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.White);
                    }
                }
            }

            return result;
        }

        public static Bitmap RemoveSmallNoise(Bitmap image, int minNeighbors = REMOVE_SMALL_NOISE_MIN_NEIGHBORS)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = 1; y < image.Height - 1; y++)
            {
                for (int x = 1; x < image.Width - 1; x++)
                {
                    Color centerPixel = image.GetPixel(x, y);
                    if (centerPixel.R < REMOVE_SMALL_NOISE_PIXEL_THRESHOLD)
                    {
                        int blackNeighbors = 0;
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                if (dx == 0 && dy == 0) continue;

                                Color neighbor = image.GetPixel(x + dx, y + dy);
                                if (neighbor.R < REMOVE_SMALL_NOISE_PIXEL_THRESHOLD) blackNeighbors++;
                            }
                        }

                        if (blackNeighbors >= minNeighbors)
                        {
                            result.SetPixel(x, y, Color.Black);
                        }
                        else
                        {
                            result.SetPixel(x, y, Color.White);
                        }
                    }
                    else
                    {
                        result.SetPixel(x, y, Color.White);
                    }
                }
            }

            for (int x = 0; x < image.Width; x++)
            {
                result.SetPixel(x, 0, Color.White);
                result.SetPixel(x, image.Height - 1, Color.White);
            }
            for (int y = 0; y < image.Height; y++)
            {
                result.SetPixel(0, y, Color.White);
                result.SetPixel(image.Width - 1, y, Color.White);
            }

            return result;
        }

        public static Bitmap OtsuThreshold(Bitmap src)
        {
            int width = src.Width;
            int height = src.Height;

            int[,] gray = new int[width, height];
            int[] histogram = new int[OTSU_HISTOGRAM_SIZE];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = src.GetPixel(x, y);
                    int g = (c.R + c.G + c.B) / PREPROCESS_AVG;
                    gray[x, y] = g;
                    histogram[g]++;
                }
            }

            int total = width * height;

            float sum = 0;
            for (int t = 0; t < OTSU_HISTOGRAM_SIZE; t++) sum += t * histogram[t];

            float sumB = 0;
            int wB = 0;
            int wF = 0;
            float maxVariance = 0;
            int threshold = 0;

            for (int t = 0; t < OTSU_HISTOGRAM_SIZE; t++)
            {
                wB += histogram[t];
                if (wB == 0) continue;
                wF = total - wB;
                if (wF == 0) break;

                sumB += t * histogram[t];

                float mB = sumB / wB;
                float mF = (sum - sumB) / wF;

                float variance = (float)wB * (float)wF * (mB - mF) * (mB - mF);
                if (variance > maxVariance)
                {
                    maxVariance = variance;
                    threshold = t;
                }
            }

            Bitmap result = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int g = gray[x, y];
                    Color newColor = g >= threshold ? Color.White : Color.Black;
                    result.SetPixel(x, y, newColor);
                }
            }
            return result;
        }

        public static Bitmap Sharpen(Bitmap image)
        {
            Bitmap sharpenImage = (Bitmap)image.Clone();

            int filterWidth = SHARPEN_FILTER_SIZE;
            int filterHeight = SHARPEN_FILTER_SIZE;
            int width = image.Width;
            int height = image.Height;

            double[,] filter = new double[filterWidth, filterHeight];
            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = SHARPEN_FILTER_EDGE;
            filter[1, 1] = SHARPEN_FILTER_CENTER;

            double factor = SHARPEN_FACTOR;
            double bias = SHARPEN_BIAS;

            Color[,] result = new Color[image.Width, image.Height];

            BitmapData pbits = sharpenImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int bytes = pbits.Stride * height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(pbits.Scan0, rgbValues, 0, bytes);

            int rgb;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + width) % width;
                            int imageY = (y - filterHeight / 2 + filterY + height) % height;

                            rgb = imageY * pbits.Stride + COLOR_CHANNELS * imageX;

                            red += rgbValues[rgb + 2] * filter[filterX, filterY];
                            green += rgbValues[rgb + 1] * filter[filterX, filterY];
                            blue += rgbValues[rgb + 0] * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), PIXEL_MIN), PIXEL_MAX);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), PIXEL_MIN), PIXEL_MAX);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), PIXEL_MIN), PIXEL_MAX);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    rgb = y * pbits.Stride + COLOR_CHANNELS * x;

                    rgbValues[rgb + 2] = result[x, y].R;
                    rgbValues[rgb + 1] = result[x, y].G;
                    rgbValues[rgb + 0] = result[x, y].B;
                }
            }

            Marshal.Copy(rgbValues, 0, pbits.Scan0, bytes);
            sharpenImage.UnlockBits(pbits);
            return sharpenImage;
        }

        public static Bitmap RemoveNoise(Bitmap bmap)
        {
            int threshold = REMOVE_NOISE_THRESHOLD;

            for (int x = 1; x < bmap.Width - 1; x++)
            {
                for (int y = 1; y < bmap.Height - 1; y++)
                {
                    Color pixel = bmap.GetPixel(x, y);
                    bool isDark = pixel.R < threshold && pixel.G < threshold && pixel.B < threshold;

                    if (isDark)
                    {
                        int blackCount = 0;

                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if (i == 0 && j == 0) continue;

                                Color neighbor = bmap.GetPixel(x + i, y + j);
                                if (neighbor.R < threshold && neighbor.G < threshold && neighbor.B < threshold)
                                {
                                    blackCount++;
                                }
                            }
                        }

                        if (blackCount <= REMOVE_SMALL_NOISE_WHITE_NEIGHBORS)
                        {
                            bmap.SetPixel(x, y, Color.White);
                        }
                        else
                        {
                            bmap.SetPixel(x, y, Color.Black);
                        }
                    }
                    else
                    {
                        bmap.SetPixel(x, y, Color.White);
                    }
                }
            }

            return bmap;
        }

        public static Bitmap PreprocessImage(Bitmap _image)
        {
            Bitmap processed = new Bitmap(_image.Width, _image.Height);
            for (int y = 0; y < _image.Height; y++)
            {
                for (int x = 0; x < _image.Width; x++)
                {
                    Color pixel = _image.GetPixel(x, y);

                    int brightness = (pixel.R + pixel.G + pixel.B) / PREPROCESS_AVG;

                    if (brightness > PREPROCESS_BRIGHTNESS_THRESHOLD)
                    {
                        processed.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        processed.SetPixel(x, y, Color.White);
                    }
                }
            }

            return processed;
        }

        private static string RunOCR(Bitmap image)
        {
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                engine.SetVariable("tessedit_char_whitelist", OCR_CHAR_WHITELIST);
                engine.SetVariable("tessedit_pageseg_mode", OCR_PAGE_SEG_MODE);
                engine.SetVariable("user_defined_dpi", OCR_DPI_STRING);

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    using (var img = Pix.LoadFromMemory(ms.ToArray()))
                    {
                        using (var page = engine.Process(img))
                        {
                            string text = page.GetText().Trim();
                            return text;
                        }
                    }
                }
            }
        }

        public static string EnhancedDetect(Rectangle _captureArea)
        {
            using (Bitmap screenshot = CaptureScreen(_captureArea))
            {
                using (Bitmap scaledImage = ScaleImage(screenshot))
                {
                    string result = string.Empty;
                    string bestApproach = "";

                    var approaches = new[]
                    {
                        new { Name = "Simple140", Threshold = 140, Inverted = false },
                        new { Name = "Simple120", Threshold = 120, Inverted = false },
                        new { Name = "Simple160", Threshold = 160, Inverted = false },
                        new { Name = "Inverted140", Threshold = 140, Inverted = true },
                        new { Name = "Inverted120", Threshold = 120, Inverted = true },
                        new { Name = "Otsu", Threshold = 0, Inverted = false }
                    };

                    foreach (var approach in approaches)
                    {
                        using (Bitmap grayImage = ConvertToGrayscale(scaledImage))
                        {
                            Bitmap thresholdImage;

                            if (approach.Name == "Otsu")
                            {
                                thresholdImage = OtsuThreshold(grayImage);
                            }
                            else
                            {
                                thresholdImage = approach.Inverted ? InvertedSimpleThreshold(grayImage, approach.Threshold) : SimpleThreshold(grayImage, approach.Threshold);
                            }

                            using (thresholdImage)
                            {
                                using (Bitmap cleanedImage = RemoveSmallNoise(thresholdImage))
                                {
                                    try
                                    {
                                        string currentResult = RunOCR(cleanedImage);
                                        if (!string.IsNullOrWhiteSpace(currentResult))
                                        {
                                            if (string.IsNullOrWhiteSpace(result) || currentResult.Length > result.Length || (currentResult.Length == result.Length && currentResult.Any(char.IsLetter)))
                                            {
                                                result = currentResult;
                                                bestApproach = approach.Name;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error with approach {approach.Name}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    return result;
                }
            }
        }

        public static string NormalDetect(Rectangle _captureArea)
        {
            using Bitmap screenshot = CaptureScreen(_captureArea);
            using Bitmap scaledImage = ScaleImage(screenshot);
            using Bitmap thresholdImage = PreprocessImage(scaledImage);
            return RunOCR(thresholdImage);
        }

        public static string DetectText(Rectangle _captureArea, bool _isEnhanced = false)
        {
            if (_isEnhanced)
            {
                return EnhancedDetect(_captureArea);
            }
            else
            {
                return NormalDetect(_captureArea);
            }
        }
    }
}