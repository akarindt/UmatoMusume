using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UmatoMusume.Data;
using UmatoMusume.Models;
using UmatoMusume.Utils;
using Timer = System.Windows.Forms.Timer;

namespace UmatoMusume
{
	public partial class FrmMain : Form
	{
		private readonly RectConfigData _rectConfigData;
		private IntPtr _processhWnd;
		private IntPtr _hWinEventHook;
		private Process? _targetProc = null;
		private Timer _attachTimer;
		private Timer _captureTimer;
		private List<Umamusume> _umaList = new List<Umamusume>();
		private List<SupportCard> _supportCardList = new List<SupportCard>();
		private List<Career> _careerList = new List<Career>();
		private List<Race> _raceList = new List<Race>();
		private List<string> _raceGrades = new List<string>();
		private List<string> _raceDistanceTypes = new List<string>();
		private List<string> _raceTerrains = new List<string>();
		private List<string> _filterGrades = new List<string>();
		private List<string> _filterDistanceTypes = new List<string>();
		private List<string> _filterTerrainTypes = new List<string>();
		private bool _firstTimeSetWindowState = false;
		private CancellationTokenSource _cts = new CancellationTokenSource();
		private DateTime _lastUpdateDate = DateTime.MinValue;
		private readonly TimeSpan _debounceInterval = TimeSpan.FromMilliseconds(200);


		private Font _boldFont = new Font(Control.DefaultFont, FontStyle.Bold);
		private Font _regularFont = new Font(Control.DefaultFont, FontStyle.Regular);

		protected Hook.WinEventDelegate _winEventDelegate;
		static GCHandle _gcSafetyHandle;

		private const string TARGET_PROCESS_NAME = "UmamusumePrettyDerby";
		private const string FORM_TITLE = "UmatoMusume - Process Window Capture";
		private const int ATTACH_INTERVAL = 500;
		private const int CAPTURE_INTERVAL = 1000;
		private const int OFFSET_HEIGHT = 100;
		private int _appHeight = 0;
		private int _appWidth = 0;


		// Paths for JSON data files
		private const string UMA_DATA_PATH = "Assets/uma_data.json";
		private const string SUPPORT_CARD_PATH = "Assets/support_card.json";
		private const string CAREER_DATA_PATH = "Assets/career.json";
		private const string RACE_DATA_PATH = "Assets/races.json";

		// Rectangles for storing captured areas
		private Rectangle? _eventOctRect = null;
		private Rectangle? _dateTimeRect = null;

		// Offsets for each capture area (relative to process window)
		private Rectangle? _eventOctOffset = null;
		private Rectangle? _dateTimeOffset = null;

		public FrmMain()
		{
			InitializeComponent();

			WindowState = FormWindowState.Minimized;
			Text = FORM_TITLE;

			_winEventDelegate = new Hook.WinEventDelegate(WinEventCallback);
			_gcSafetyHandle = GCHandle.Alloc(_winEventDelegate);
			_rectConfigData = new RectConfigData(new UmatoDBContext());

			_attachTimer = new Timer();
			_attachTimer.Interval = ATTACH_INTERVAL;
			_attachTimer.Tick += AttachTimer_Tick;
			_attachTimer.Start();

			_captureTimer = new Timer();
			_captureTimer.Interval = CAPTURE_INTERVAL;
			_captureTimer.Tick += EventTimer_Tick;
			_captureTimer.Start();

			_umaList = Helper.LoadFromJson<Umamusume>(UMA_DATA_PATH);
			_supportCardList = Helper.LoadFromJson<SupportCard>(SUPPORT_CARD_PATH);
			_careerList = Helper.LoadFromJson<Career>(CAREER_DATA_PATH);
			_raceList = Helper.LoadFromJson<Race>(RACE_DATA_PATH);

			var primaryScreen = Screen.PrimaryScreen;
			if (primaryScreen != null)
			{
				_appHeight = primaryScreen.WorkingArea.Height - OFFSET_HEIGHT;
			}

			_appWidth = Width;

			InitFilter();
			Updater.CleanupOldFiles();
		}

		#region Functions
		private void InitFilter()
		{
			_raceGrades = _raceList.GetRaceGrades();
			_raceDistanceTypes = _raceList.GetRaceDistanceTypes();
			_raceTerrains = _raceList.GetRaceTerrains();

			pGradeCheckboxes.FlowDirection = FlowDirection.LeftToRight;
			pGradeCheckboxes.WrapContents = true;
			pGradeCheckboxes.AutoScroll = true;

			foreach (var grade in _raceGrades)
			{
				var checkbox = new CheckBox
				{
					Text = grade,
					AutoSize = true,
					Checked = false,
				};

				checkbox.CheckedChanged += (s, e) =>
				{
					if (checkbox.Checked)
					{
						_filterGrades.Add(checkbox.Text);
					}
					else
					{
						_filterGrades.Remove(checkbox.Text);
					}
					SetRaceData(_filterGrades, _filterDistanceTypes, _filterTerrainTypes);
				};

				pGradeCheckboxes.Controls.Add(checkbox);
			}

			foreach (var distanceType in _raceDistanceTypes)
			{
				var checkbox = new CheckBox
				{
					Text = distanceType,
					AutoSize = true,
					Checked = false,
				};
				checkbox.CheckedChanged += (s, e) =>
				{
					if (checkbox.Checked)
					{
						_filterDistanceTypes.Add(checkbox.Text);
					}
					else
					{
						_filterDistanceTypes.Remove(checkbox.Text);
					}
					SetRaceData(_filterGrades, _filterDistanceTypes, _filterTerrainTypes);
				};
				pGradeCheckboxes.Controls.Add(checkbox);
			}

			foreach (var terrain in _raceTerrains)
			{
				var checkbox = new CheckBox
				{
					Text = terrain,
					AutoSize = true,
					Checked = false,
				};
				checkbox.CheckedChanged += (s, e) =>
				{
					if (checkbox.Checked)
					{
						_filterTerrainTypes.Add(checkbox.Text);
					}
					else
					{
						_filterTerrainTypes.Remove(checkbox.Text);
					}
					SetRaceData(_filterGrades, _filterDistanceTypes, _filterTerrainTypes);
				};
				pGradeCheckboxes.Controls.Add(checkbox);
			}
		}

		private void StartCapture()
		{
			_captureTimer.Stop();

			if (_processhWnd == IntPtr.Zero)
			{
				_captureTimer.Start();
				return;
			}

			if (_eventOctRect is Rectangle eventRect && eventRect.Width > 0 && eventRect.Height > 0)
			{
				_ = Task.Run(async () =>
				{
					try
					{
						var text = await Detector.DetectText(eventRect);
						BeginInvoke(new Action(() => lblEventName.Text = text));
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Event OCR error: {ex.Message}");
					}
				});
			}

			if (_dateTimeRect is Rectangle dateRect && dateRect.Width > 0 && dateRect.Height > 0)
			{
				_ = Task.Run(async () =>
				{
					try
					{
						var text = await Detector.DetectText(dateRect);
						var completed = Helper.CompleteText(text);

						if (!string.IsNullOrWhiteSpace(completed) && (DateTime.Now - _lastUpdateDate > _debounceInterval))
						{
							_lastUpdateDate = DateTime.Now;
							BeginInvoke(new Action(() => lblDate.Text = completed));
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Date OCR error: {ex.Message}");
					}
				});
			}

			_captureTimer.Start();
		}


		private void WinEventCallback(IntPtr _, NativeMethods.SWEH_Events _eventType, IntPtr _hWnd, NativeMethods.SWEH_ObjectId _idObject, long _idChild, uint _dwEventThread, uint _dwmsEventTime)
		{
			if (!IsHandleCreated && IsDisposed) return;
			if (_hWnd == IntPtr.Zero || _hWnd != _processhWnd) return;
			var isFullScreen = bool.Parse(Helper.GetConfigValue("FullScreen", "False"));

			switch (_eventType)
			{
				case NativeMethods.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE:
					if (!isFullScreen && _idObject == NativeMethods.SWEH_ObjectId.OBJID_WINDOW)
					{
						var rectEnd = Hook.GetWindowRectangle(_hWnd).ToRectangle();
						BeginInvoke(new Action(() => UpdateUI(rectEnd)));
					}
					break;
				case NativeMethods.SWEH_Events.EVENT_SYSTEM_FOREGROUND:
					var fg = NativeMethods.GetForegroundWindow();
					if (fg == _processhWnd)
					{
						BeginInvoke(new Action(() =>
						{
							if (WindowState == FormWindowState.Minimized)
							{
								Show();
								WindowState = FormWindowState.Normal;
								NativeMethods.ShowWindow(this.Handle, NativeMethods.SW_RESTORE);
							}
							NativeMethods.BringWindowToTop(this.Handle);
							TopMost = true;
							TopMost = false;
						}));
					}
					break;

				case NativeMethods.SWEH_Events.EVENT_SYSTEM_MINIMIZESTART:
					BeginInvoke(new Action(() => WindowState = FormWindowState.Minimized));
					break;

				case NativeMethods.SWEH_Events.EVENT_SYSTEM_MINIMIZEEND:
					BeginInvoke(new Action(() =>
					{
						if (WindowState == FormWindowState.Minimized)
						{
							Show();
							WindowState = FormWindowState.Normal;
						}
					}));
					break;
			}
		}

		private void UpdateUI(Rectangle _rect)
		{
			var isFullScreen = bool.Parse(Helper.GetConfigValue("FullScreen", "False"));
			TopMost = isFullScreen;

			var isRightMenu = bool.Parse(Helper.GetConfigValue("RightMenu", "False"));
			if (isRightMenu)
			{
				Location = new Point(_rect.Right, _rect.Top);
			}
			else
			{
				Location = new Point(_rect.Left - _appWidth, _rect.Top);
			}

			Height = _appHeight;
			Width = _appWidth;

			UpdateCaptureAreasWithWindow(_rect);

			if (NativeMethods.IsIconic(_processhWnd))
			{
				if (WindowState != FormWindowState.Minimized)
				{
					WindowState = FormWindowState.Minimized;
				}
			}
			else
			{
				if (WindowState == FormWindowState.Minimized)
				{
					WindowState = FormWindowState.Normal;
				}
			}

			_ = SaveRectsAsync();
		}

		private async Task SaveRectsAsync()
		{
			if (_eventOctRect != null)
			{
				await _rectConfigData.Upsert(new RectConfig
				{
					RectName = "EVENT_RECT",
					X = _eventOctRect.Value.X,
					Y = _eventOctRect.Value.Y,
					Width = _eventOctRect.Value.Width,
					Height = _eventOctRect.Value.Height
				});
			}

			if (_dateTimeRect != null)
			{
				await _rectConfigData.Upsert(new RectConfig
				{
					RectName = "DATETIME_RECT",
					X = _dateTimeRect.Value.X,
					Y = _dateTimeRect.Value.Y,
					Width = _dateTimeRect.Value.Width,
					Height = _dateTimeRect.Value.Height
				});
			}
		}

		private async Task CaptureEvent()
		{
			_eventOctRect = Detector.CaptureArea(_processhWnd);

			if (_eventOctRect == null)
			{
				MessageBox.Show("Please select an area to capture.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var windowRect = Hook.GetWindowRectangle(_processhWnd).ToRectangle();

			_eventOctOffset = new Rectangle(
				_eventOctRect.Value.X - windowRect.Left,
				_eventOctRect.Value.Y - windowRect.Top,
				_eventOctRect.Value.Width,
				_eventOctRect.Value.Height
			);

			await _rectConfigData.Upsert(new RectConfig
			{
				RectName = "EVENT_RECT",
				X = _eventOctRect.Value.X,
				Y = _eventOctRect.Value.Y,
				Width = _eventOctRect.Value.Width,
				Height = _eventOctRect.Value.Height
			});
			return;
		}

		private void UpdateCaptureAreasWithWindow(Rectangle _windowRect)
		{
			if (_eventOctOffset != null)
			{
				_eventOctRect = new Rectangle(
					_windowRect.Left + _eventOctOffset.Value.X,
					_windowRect.Top + _eventOctOffset.Value.Y,
					_eventOctOffset.Value.Width,
					_eventOctOffset.Value.Height);
			}

			if (_dateTimeOffset != null)
			{
				_dateTimeRect = new Rectangle(
					_windowRect.Left + _dateTimeOffset.Value.X,
					_windowRect.Top + _dateTimeOffset.Value.Y,
					_dateTimeOffset.Value.Width,
					_dateTimeOffset.Value.Height);
			}
		}

		private async Task InitConfig()
		{
			var configTasks = new[]
			{
				_rectConfigData.Get("EVENT_RECT"),
				_rectConfigData.Get("DATETIME_RECT"),
				_rectConfigData.Get("WINDOW_RECT"),
			};
			Game8Resolver_Config.EnsureDirs();
			var results = await Task.WhenAll(configTasks);
			var eventRect = results[0];
			var dateTimeRect = results[1];
			var appSize = results[2];
			var checkForUpdates = bool.Parse(Helper.GetConfigValue("AutoUpdate", "False"));

			_eventOctRect = eventRect?.ToRectangle() ?? null;
			_dateTimeRect = dateTimeRect?.ToRectangle() ?? null;

			_appHeight = appSize?.Height ?? _appHeight;
			_appWidth = appSize?.Width ?? _appWidth;

			BeginInvoke(new Action(() => UpdateUIAfterConfigAsync()));

			if (checkForUpdates)
			{
				var check = await Updater.CheckForUpdates();
				if (check)
				{
					var dialogResult = MessageBox
						.Show(
						"An update is available. Do you want to download and install it now?",
						"Update Available",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question);

					if (dialogResult == DialogResult.Yes)
					{
						FrmUpdater frmUpdater = new FrmUpdater();
						frmUpdater._shouldUpdate = true;
						frmUpdater.ShowDialog();
					}
				}
			}
		}

		private async Task UpdateUIAfterConfigAsync()
		{
			Height = _appHeight;
			Width = _appWidth;
			cboCharacterName.BeginUpdate();
			try
			{
				cboCharacterName.Items.Clear();

				// using Game8
				var resolver = new Game8Resolver_Config.Game8Resolver();
				var umaList = await resolver.FetchAllUmaNameAsync();
				umaList.Sort();
				foreach (var uma in umaList)
				{
					cboCharacterName.Items.Add(uma);
				}
				/*
				// using GameTora
				foreach (var uma in _umaList)
				{
					cboCharacterName.Items.Add(uma.UmaName);
				}
				*/
			}
			finally
			{
				cboCharacterName.EndUpdate();
			}
		}
		private void SetData()
		{
			SetDataGame8();
		}
		private async void SetDataGame8()
		{
			try
			{
				rtbOptions.Clear();
				var selectedUma = cboCharacterName.GetSelectedValue<string>();
				if (selectedUma != null)
					selectedUma = selectedUma.Substring(0, selectedUma.IndexOf('(')-2);
				if (!string.IsNullOrEmpty(lblEventName.Text))
				{
					var resolver = new Game8Resolver_Config.Game8Resolver();
					List<Task<JObject>> tasks = new();
					List<JObject> resultList = new();

					JObject results = await resolver.ResolveAsync(lblEventName.Text, selectedUma);
					if (results["status"].ToString() == "not_found")
						return;

					if (results["status"].ToString() == "ambiguous")
					{
						foreach (var eventResult in results["events"])
						{
							tasks.Add(resolver.ResolveAsync(eventResult["name"].ToString()));
						}
					}
					else
					{
						resultList.Add(results);
					}
					if (tasks.Count > 0)
					{
						var taskResult = await Task.WhenAll(tasks);
						foreach (var task in taskResult)
						{
							resultList.Add(task);
						}
					}


					foreach (JObject result in resultList)
					{
						foreach (var events in result["events"])
						{
							var matching = events["matching"];
							rtbOptions.SelectionFont = _boldFont;
							rtbOptions.AppendText(matching["normalized_query"] + ":\n");
							foreach (var choices in events["choices"])
							{
								string choiceText = choices["label"].ToString();
								string cleanChoiceText = choiceText.Replace("\n", " ");
								cleanChoiceText = System.Text.RegularExpressions.Regex.Replace(cleanChoiceText, @"\s+", " ");
								rtbOptions.SelectionFont = _regularFont;
								rtbOptions.AppendText($"{cleanChoiceText} {(bool.Parse(choices["random"].ToString()) ? "\n(* Random)" : "")}\n");
								foreach (var effects in choices["effects"])
								{
									rtbOptions.SelectionFont = _regularFont;
									rtbOptions.AppendText($"  {effects["display_text"]}\n");
								}
								rtbOptions.AppendText($"{(string.IsNullOrEmpty(result["message"].ToString()) ? $"{result["message"]}" : "")}---------------\n");

							}
						}
					}
					tasks.Clear();
					resultList.Clear();
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Fatal: " + ex);
				return;
			}
		}
		private void SetDataGameTora()
		{
			rtbOptions.Clear();

			var selectedUma = cboCharacterName.GetSelectedValue<string>();
			if (!string.IsNullOrEmpty(selectedUma))
			{
				var objectives = _umaList.GetUmaObjectives(selectedUma);
				if (objectives.Count > 0)
				{
					rtbObjectives.Clear();
					foreach (var objective in objectives)
					{
						rtbObjectives.SelectionFont = _boldFont;
						rtbObjectives.AppendText(objective.ObjectiveName + ":\n");
						rtbObjectives.SelectionFont = _regularFont;
						rtbObjectives.AppendText($"Turn: {objective.Turn} \nTime: {objective.Time} \nCondition: {objective.ObjectiveCondition}\n");
					}
				}
			}

			if (!string.IsNullOrEmpty(selectedUma) && !string.IsNullOrEmpty(lblEventName.Text))
			{
				var options = _umaList.GetUmaEventOptions(selectedUma, lblEventName.Text, _raceGrades);
				if (options.Any())
				{
					foreach (var option in options.SelectMany(x => x))
					{
						if (!string.IsNullOrEmpty(option.Key))
						{
							rtbOptions.SelectionFont = _boldFont;
							rtbOptions.AppendText(option.Key + ":\n");
						}

						rtbOptions.SelectionFont = _regularFont;
						rtbOptions.AppendText(option.Value + "\n---------------\n");
					}
				}
				else
				{
					var cardOptions = _supportCardList.GetSupportCardEventOptions(lblEventName.Text);
					if (cardOptions.Any())
					{

						foreach (var option in cardOptions.SelectMany(x => x))
						{
							if (!string.IsNullOrEmpty(option.Key))
							{
								rtbOptions.SelectionFont = _boldFont;
								rtbOptions.AppendText(option.Key + ":\n");
							}

							rtbOptions.SelectionFont = _regularFont;
							rtbOptions.AppendText(option.Value + "\n---------------\n");
						}
					}
					else
					{
						var careerOptions = _careerList.GetCareerEvents(lblEventName.Text);
						if (careerOptions.Any())
						{

							foreach (var option in careerOptions.SelectMany(x => x))
							{
								if (!string.IsNullOrEmpty(option.Key))
								{
									rtbOptions.SelectionFont = _boldFont;
									rtbOptions.AppendText(option.Key + ":\n");
								}

								rtbOptions.SelectionFont = _regularFont;
								rtbOptions.AppendText(option.Value + "\n---------------\n");
							}
						}
					}
				}
			}
		}

		private void SetRaceData(List<string> _grades, List<string> _distanceTypes, List<string> _terrainTypes)
		{
			rtbRaces.Clear();
			if (!string.IsNullOrEmpty(lblDate.Text))
			{
				var races = _raceList.GetRaces(lblDate.Text, _grades, _distanceTypes, _terrainTypes);
				if (races.Any())
				{
					foreach (var race in races)
					{
						rtbRaces.SelectionFont = _boldFont;
						rtbRaces.AppendText($"{race.RaceName}({race.Grade}) - {race.Terrain} - {race.DistanceType} - {race.DistanceMeter}\n");

						rtbRaces.SelectionFont = _regularFont;
						rtbRaces.AppendText($"Fans required: {race.FansRequired} - Fans gained: {race.FansGained}" + "\n---------------\n");
					}
				}
			}
		}

		private async Task CaptureDateTime()
		{
			_dateTimeRect = Detector.CaptureArea(_processhWnd);

			if (_dateTimeRect == null)
			{
				MessageBox.Show("Please select an area to capture.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var windowRect = Hook.GetWindowRectangle(_processhWnd).ToRectangle();

			_dateTimeOffset = new Rectangle(
				_dateTimeRect.Value.X - windowRect.Left,
				_dateTimeRect.Value.Y - windowRect.Top,
				_dateTimeRect.Value.Width,
				_dateTimeRect.Value.Height
			);

			await _rectConfigData.Upsert(new RectConfig
			{
				RectName = "DATETIME_RECT",
				X = _dateTimeRect.Value.X,
				Y = _dateTimeRect.Value.Y,
				Width = _dateTimeRect.Value.Width,
				Height = _dateTimeRect.Value.Height
			});

			return;
		}
		#endregion

		#region Event Handlers
		private void EventTimer_Tick(object? sender, EventArgs e) => StartCapture();
		readonly object timerLock = new();
		private void AttachTimer_Tick(object? sender, EventArgs e)
		{
			try
			{
				if (_targetProc == null || _targetProc.HasExited)
				{
					_targetProc = Process.GetProcessesByName(TARGET_PROCESS_NAME).FirstOrDefault(p => p != null);
					if (_targetProc == null)
					{
						return;
					}
					_targetProc.EnableRaisingEvents = true;
					_targetProc.Exited += (s, ev) =>
					{
						BeginInvoke(new Action(() => Close()));
						return;
					};
				}

				_processhWnd = _targetProc.MainWindowHandle;
				if (_processhWnd != IntPtr.Zero)
				{
					_attachTimer.Stop();
					uint targetThreadId = Hook.GetWindowThread(_processhWnd);
					_hWinEventHook = Hook.WinEventHookRange(NativeMethods.SWEH_Events.EVENT_SYSTEM_FOREGROUND, NativeMethods.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE, _winEventDelegate, (uint)_targetProc.Id, targetThreadId);
					var rect = Hook.GetWindowRectangle(_processhWnd);

					Height = _appHeight;
					Width = _appWidth;

					var isFullScreen = bool.Parse(Helper.GetConfigValue("FullScreen", "False"));
					if (isFullScreen)
					{
						TopMost = true;
						if (Location.X == 0 && Location.Y == 0)
						{
							StartPosition = FormStartPosition.CenterScreen;
						}
					}
					else
					{
						TopMost = false;
						var isRightMenu = bool.Parse(Helper.GetConfigValue("RightMenu", "False"));
						if (isRightMenu)
						{
							Location = new Point(rect.Right, rect.Top);
						}
						else
						{
							Location = new Point(rect.Left - _appWidth, rect.Top);
						}
					}
				}

				if (!_firstTimeSetWindowState)
				{
					if (NativeMethods.IsIconic(_processhWnd))
					{
						if (WindowState != FormWindowState.Minimized)
						{
							WindowState = FormWindowState.Minimized;
						}
					}
					else
					{
						if (WindowState == FormWindowState.Minimized)
						{
							this.Show();
							WindowState = FormWindowState.Normal;
						}
					}

					_firstTimeSetWindowState = true;
				}
			}
			catch { }
		}

		private async void btnCaptureEvent_Click(object sender, EventArgs e)
		{
			await CaptureEvent();
		}

		private async void FrmMain_Load(object sender, EventArgs e)
		{
			try
			{
				await InitConfig();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error during initialization: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void lblEventName_TextChanged(object sender, EventArgs e) => SetData();

		private void btnDownloadUmaData_Click(object sender, EventArgs e)
		{
			FrmDownload frmDownload = new FrmDownload();
			frmDownload.ShowDialog();
		}

		private void cboCharacterName_SelectedIndexChanged(object sender, EventArgs e) => SetData();

		private async void btnCaptureDateTime_Click(object sender, EventArgs e)
		{
			await CaptureDateTime();
		}

		private void lblDate_TextChanged(object sender, EventArgs e) => SetRaceData(_filterGrades, _filterDistanceTypes, _filterTerrainTypes);

		private async void FrmMain_ResizeEnd(object sender, EventArgs e)
		{
			_attachTimer.Stop();

			await _rectConfigData.Upsert(new RectConfig
			{
				RectName = "WINDOW_RECT",
				X = Location.X,
				Y = Location.Y,
				Width = Width,
				Height = Height
			});

			_appHeight = Height;
			_appWidth = Width;

			_attachTimer.Start();
		}

		private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_gcSafetyHandle.IsAllocated)
			{
				_gcSafetyHandle.Free();
			}

			Hook.WinEventUnhook(_hWinEventHook);

			Detector.Dispose();

			_attachTimer?.Stop();
			_attachTimer?.Dispose();

			_captureTimer?.Stop();
			_captureTimer?.Dispose();

			_rectConfigData?.Dispose();
			GameTora.DisposeResources();
		}

		private void btnOpenConfig_Click(object sender, EventArgs e)
		{
			var form = new FrmSetting();
			form.ShowDialog();
		}
		#endregion
	}
}
