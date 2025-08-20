using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
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

        protected Hook.WinEventDelegate _winEventDelegate;
        static GCHandle _gcSafetyHandle;

        private const string TARGET_PROCESS_NAME = "UmamusumePrettyDerby";
        private const string FORM_TITLE = "UmatoMusume - Process Window Capture";
        private const int ATTACH_INTERVAL = 500;
        private const int CAPTURE_INTERVAL = 1000;
        private const int OFFSET_HEIGHT = 100;

        // Paths for JSON data files
        private const string UMA_DATA_PATH = "Assets/uma_data.json";
        private const string SUPPORT_CARD_PATH = "Assets/support_card.json";
        private const string CAREER_DATA_PATH = "Assets/career.json";
        private const string RACE_DATA_PATH = "Assets/races.json";

        // Rectangles for storing captured areas
        private Rectangle? _eventOctRect = null;
        private Rectangle? _dateTimeRect = null;

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
        }

        private async void EventTimer_Tick(object? sender, EventArgs e)
        {
            if (_processhWnd != IntPtr.Zero)
            {
                if (_eventOctRect != null)
                {
                    var rect = (Rectangle)_eventOctRect;
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        lblEventName.Text = await Task.Run(() => Detector.DetectText(rect));
                    }
                }

                if (_dateTimeRect != null)
                {
                    var rect = (Rectangle)_dateTimeRect;
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        lblDate.Text = await Task.Run(() => Detector.DetectText(rect));
                    }
                }
            }
        }

        private void AttachTimer_Tick(object? sender, EventArgs e)
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
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => Close()));
                        return;
                    }

                    Close();
                    return;
                };
            }

            _processhWnd = _targetProc.MainWindowHandle;
            if (_processhWnd != IntPtr.Zero)
            {
                _attachTimer.Stop();
                uint targetThreadId = Hook.GetWindowThread(_processhWnd);
                _hWinEventHook = Hook.WinEventHookOne(NativeMethods.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE, _winEventDelegate, (uint)_targetProc.Id, targetThreadId);
                var rect = Hook.GetWindowRectangle(_processhWnd);
                Location = new Point(rect.Right, rect.Top);

                var primaryScreen = Screen.PrimaryScreen;
                if (primaryScreen != null)
                {
                    Height = primaryScreen.WorkingArea.Height - OFFSET_HEIGHT;
                }

                WindowState = FormWindowState.Normal;
            }
        }

        protected async void WinEventCallback(
            IntPtr hWinEventHook,
            NativeMethods.SWEH_Events eventType,
            IntPtr hWnd,
            NativeMethods.SWEH_ObjectId idObject,
            long idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (hWnd == _processhWnd &&
                eventType == NativeMethods.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE &&
                idObject == (NativeMethods.SWEH_ObjectId)NativeMethods.SWEH_CHILDID_SELF)
            {
                var rect = Hook.GetWindowRectangle(hWnd).ToRectangle();
                Location = new Point(rect.Right, rect.Top);

                var primaryScreen = Screen.PrimaryScreen;
                if (primaryScreen != null)
                {
                    Height = primaryScreen.WorkingArea.Height - OFFSET_HEIGHT;
                }

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

                if (NativeMethods.IsIconic(hWnd))
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
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel)
            {
                if (_gcSafetyHandle.IsAllocated)
                {
                    _gcSafetyHandle.Free();
                }
                Hook.WinEventUnhook(_hWinEventHook);
            }
        }

        private async void btnCaptureEvent_Click(object sender, EventArgs e)
        {
            _eventOctRect = Detector.CaptureArea(_processhWnd);

            if (_eventOctRect == null)
            {
                MessageBox.Show("Please select an area to capture.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Store the absolute coordinates (not relative to window)
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

        private async Task InitConfig()
        {
            var eventRect = await _rectConfigData.Get("EVENT_RECT");
            var dateTimeRect = await _rectConfigData.Get("DATETIME_RECT");

            _eventOctRect = eventRect?.ToRectangle() ?? null;
            _dateTimeRect = dateTimeRect?.ToRectangle() ?? null;

            var primaryScreen = Screen.PrimaryScreen;
            if (primaryScreen != null)
            {
                Height = primaryScreen.WorkingArea.Height - OFFSET_HEIGHT;
            }


            foreach (var uma in _umaList)
            {
                cboCharacterName.Items.Add(uma.UmaName);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            _ = InitConfig();
        }

        private void SetData()
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
                        rtbObjectives.SelectionFont = new Font(rtbObjectives.Font, FontStyle.Bold);
                        rtbObjectives.AppendText(objective.ObjectiveName + ":\n");
                        rtbObjectives.SelectionFont = new Font(rtbObjectives.Font, FontStyle.Regular);
                        rtbObjectives.AppendText($"Turn: {objective.Turn} \nTime: {objective.Time} \nCondition: {objective.ObjectiveCondition}\n");
                    }
                }
            }

            if (!string.IsNullOrEmpty(selectedUma) && !string.IsNullOrEmpty(lblEventName.Text))
            {
                var options = _umaList.GetUmaEventOptions(selectedUma, lblEventName.Text);
                if (options.Any())
                {
                    foreach (var option in options.SelectMany(x => x))
                    {
                        if (!string.IsNullOrEmpty(option.Key))
                        {
                            rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Bold);
                            rtbOptions.AppendText(option.Key + ":\n");
                        }

                        var isContainHtml = option.Value.ConvertXHTMLEntities().ContainsXHTML();
                        if (isContainHtml)
                        {
                            rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Regular);
                            rtbOptions.AppendText(HtmlToText.ConvertHtml(option.Value) + "\n---------------\n");
                        }
                        else
                        {
                            rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Regular);
                            rtbOptions.AppendText(option.Value + "\n---------------\n");
                        }
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
                                rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Bold);
                                rtbOptions.AppendText(option.Key + ":\n");
                            }

                            var isContainHtml = option.Value.ConvertXHTMLEntities().ContainsXHTML();
                            if (isContainHtml)
                            {
                                rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Regular);
                                rtbOptions.AppendText(HtmlToText.ConvertHtml(option.Value) + "\n---------------\n");
                            }
                            else
                            {
                                rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Regular);
                                rtbOptions.AppendText(option.Value + "\n---------------\n");
                            }
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
                                    rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Bold);
                                    rtbOptions.AppendText(option.Key + ":\n");
                                }

                                var isContainHtml = option.Value.ConvertXHTMLEntities().ContainsXHTML();
                                if (isContainHtml)
                                {
                                    rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Regular);
                                    rtbOptions.AppendText(HtmlToText.ConvertHtml(option.Value) + "\n---------------\n");
                                }
                                else
                                {
                                    rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Regular);
                                    rtbOptions.AppendText(option.Value + "\n---------------\n");
                                }
                            }
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
                                    rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Bold);
                                    rtbOptions.AppendText(option.Key + ":\n");
                                }
                                rtbOptions.SelectionFont = new Font(rtbOptions.Font, FontStyle.Regular);
                                rtbOptions.AppendText(HtmlToText.ConvertHtml(option.Value) + "\n---------------\n");
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(lblDate.Text))
            {
                var races = _raceList.GetRaces(lblDate.Text);
                if (races.Any())
                {
                    foreach (var race in races)
                    {
                        rtbRaces.SelectionFont = new Font(rtbRaces.Font, FontStyle.Bold);
                        rtbRaces.AppendText($"{race.RaceName}({race.Grade}) - {race.Terrain} - {race.DistanceType} - {race.DistanceMeter}\n");

                        rtbRaces.SelectionFont = new Font(rtbRaces.Font, FontStyle.Regular);
                        rtbRaces.AppendText($"Fans required: {race.FansRequired} - Gained: {race.FansGained}" + "\n---------------\n");
                    }
                }
            }
        }

        private void SetRaceData()
        {
            rtbRaces.Clear();
            if (!string.IsNullOrEmpty(lblDate.Text))
            {
                var races = _raceList.GetRaces(lblDate.Text);
                if (races.Any())
                {
                    foreach (var race in races)
                    {
                        rtbRaces.SelectionFont = new Font(rtbRaces.Font, FontStyle.Bold);
                        rtbRaces.AppendText($"{race.RaceName}({race.Grade}) - {race.Terrain} - {race.DistanceType} - {race.DistanceMeter}\n");

                        rtbRaces.SelectionFont = new Font(rtbRaces.Font, FontStyle.Regular);
                        rtbRaces.AppendText($"Fans required: {race.FansRequired} - Gained: {race.FansGained}" + "\n---------------\n");
                    }
                }
            }
        }

        private void SetRaceData()
        {
            rtbRaces.Clear();
            if (!string.IsNullOrEmpty(lblDate.Text))
            {
                var races = _raceList.GetRaces(lblDate.Text);
                if (races.Any())
                {
                    foreach (var race in races)
                    {
                        rtbRaces.SelectionFont = new Font(rtbRaces.Font, FontStyle.Bold);
                        rtbRaces.AppendText($"{race.RaceName}({race.Grade}) - {race.Terrain} - {race.DistanceType} - {race.DistanceMeter}\n");

                        rtbRaces.SelectionFont = new Font(rtbRaces.Font, FontStyle.Regular);
                        rtbRaces.AppendText($"Fans required: {race.FansRequired} - Gained: {race.FansGained}" + "\n---------------\n");
                    }
                }
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
            _dateTimeRect = Detector.CaptureArea(_processhWnd);

            if (_dateTimeRect == null)
            {
                MessageBox.Show("Please select an area to capture.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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

        private void lblDate_TextChanged(object sender, EventArgs e) => SetRaceData();
    }
}