using OpenCvSharp;
using OpenCvSharp.Extensions;
using PaddleOCRJson;
using System.Diagnostics;
using System.Drawing.Imaging;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
	public class Detector
	{
		private const float IMAGE_SCALE = 3.0f;
		private const int DELAY_MS = 1000;
		private const string ENGINE_PATH_PADDLE = "Extras/PaddleOCR/PaddleOCR-json.exe";
		private const string ENGINE_PATH_RAPID = "Extras/RapidOCR/RapidOCR-json.exe";

		private static readonly OcrEngine _engine;
		private static readonly OcrClient _client;
		private static readonly string _currentEnginePath;

		static Detector()
		{
			var isRapid = bool.Parse(Helper.GetConfigValue("UseRapidOCR", "False"));
			_currentEnginePath = isRapid ? ENGINE_PATH_RAPID : ENGINE_PATH_PADDLE;

			var startupArgs = OcrEngineStartupArgs
				.WithPipeMode(_currentEnginePath)
				.CpuThreads(1)
				.EnableMkldnn(true);

			_engine = new OcrEngine(startupArgs);
			_client = _engine.CreateClient();

			Application.ApplicationExit += Application_ApplicationExit;
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
		}

		private static void Application_ApplicationExit(object? sender, EventArgs e) => Dispose();

		private static void CurrentDomain_ProcessExit(object? sender, EventArgs e) => Dispose();

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

		public static OcrResult? RunOCR(Bitmap _bmp)
		{
			byte[] imageBytes = _bmp.ToByteArray();
			var ret = _client.FromImageBytes(imageBytes).JsonToData<OcrResult>();
			return ret;
		}

		public static async Task<string> DetectText(Rectangle _captureArea)
		{
			return await Task.Run(() =>
			{
				using var capturedImage = CaptureScreen(_captureArea);
				using var scaled = Smoother(capturedImage);
				using var norm = Normalization(scaled);
				using var gray = ToGray(norm);
				using var denoise = Denoise(gray);
				using var binary = Threshold(denoise);

				// For debugging purposes, uncomment to save intermediate images
				//SaveBitmap(binary, Path.Combine(Path.GetTempPath(), "debug_binary.png"));

				var result = RunOCR(binary);
				return string.Join(" ", result?.OcrData?.Select(x => x.Text) ?? []);
			});
		}

		public static void Dispose()
		{
			try
			{
				_client?.Dispose();
				_engine?.Dispose();

				var ocrProcessNames = new[] { "PaddleOCR-json", "RapidOCR-json" };
				foreach (string processName in ocrProcessNames)
				{
					var processes = Process.GetProcessesByName(processName);
					foreach (var process in processes)
					{
						try
						{
							if (!process.HasExited)
							{
								process.CloseMainWindow();
								if (!process.WaitForExit(DELAY_MS))
								{
									process.Kill();
									process.WaitForExit(DELAY_MS);
								}
							}
						}
						catch
						{

						}
						finally
						{
							process.Dispose();
						}
					}
				}
			}
			catch
			{

			}
		}

		public static Mat BitmapToMat(Bitmap _bmp) => BitmapConverter.ToMat(_bmp);

		public static Bitmap MatToBitmap(Mat _bmp) => BitmapConverter.ToBitmap(_bmp);

		public static Bitmap Normalization(Bitmap _bmp)
		{
			using var mat = BitmapToMat(_bmp);
			using var normalized = new Mat();
			Cv2.Normalize(mat, normalized, 0, 255, NormTypes.MinMax);
			return MatToBitmap(normalized);
		}

		public static Bitmap ToGray(Bitmap _bmp)
		{
			using var mat = BitmapToMat(_bmp);
			if (mat.Channels() == 1)
			{
				return MatToBitmap(mat);
			}

			using var gray = new Mat();
			Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
			return MatToBitmap(gray);
		}

		public static Bitmap Threshold(Bitmap _bmp)
		{
			using var mat = BitmapToMat(_bmp);
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
		public static Bitmap Denoise(Bitmap _bmp)
		{
			using var mat = BitmapToMat(_bmp);
			using var dst = new Mat();

			if (mat.Channels() == 1)
				Cv2.FastNlMeansDenoising(mat, dst);
			else
				Cv2.FastNlMeansDenoisingColored(mat, dst);

			return MatToBitmap(dst);
		}



		private static void SaveBitmap(Bitmap bmp, string path)
		{
			bmp.Save(path, ImageFormat.Png);
		}

		public static Bitmap Smoother(Bitmap _bmp, double _scale = IMAGE_SCALE)
		{
			int width = (int)(_bmp.Width * _scale);
			int height = (int)(_bmp.Height * _scale);

			using var mat = BitmapToMat(_bmp);
			using var upscaled = new Mat();
			Cv2.Resize(mat, upscaled, new OpenCvSharp.Size(width, height), 0, 0, InterpolationFlags.Cubic);

			using var blurred = new Mat();
			Cv2.GaussianBlur(upscaled, blurred, new OpenCvSharp.Size(3, 3), 0);

			Mat gray = blurred;
			if (blurred.Channels() > 1)
			{
				gray = new Mat();
				Cv2.CvtColor(blurred, gray, ColorConversionCodes.BGRA2GRAY);
			}

			using var binarized = new Mat();
			Cv2.Threshold(gray, binarized, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

			return MatToBitmap(binarized);
		}

	}
}
