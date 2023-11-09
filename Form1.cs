using System.Threading;
using System.Diagnostics;
using System.IO;
using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using DevExpress.Utils;
using Accord.Video.FFMPEG;

namespace DXScreenCapture {
    public partial class Form1 : Form {
        private WinAPI.LowLevelKeyboardProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;
        private Image capturedImage = null;

        private VideoFileWriter videoWriter;
        private System.Windows.Forms.Timer videoWriterTimer;

        private IntPtr desktopDC;
        private Graphics desktopGraphics;

        private int quality = 20;

        public Form1() {
            InitializeComponent();

            KeyPreview = true;
            ShowInTaskbar = false;

            _proc = HookCallback;

            _hookID = SetHook(_proc);

            videoWriterTimer = new System.Windows.Forms.Timer();
            videoWriterTimer.Interval = 50;

            videoWriterTimer.Tick += VideoWriterTimer_Tick;
            
            this.FormClosed += Form1_FormClosed;

            diagramControl1.OptionsView.AllowShapeShadows = false;
            diagramControl1.OptionsView.ShowRulers = false;
            diagramControl1.OptionsView.ShowGrid = false;
            diagramControl1.OptionsBehavior.SnapToGrid = false;
            diagramControl1.OptionsBehavior.SnapToItems = false;
            diagramControl1.OptionsBehavior.ScrollMode = DiagramScrollMode.Content;

            diagramControl1.OptionsBehavior.ActiveTool = new FactoryConnectorTool("CustomArrow", () => "CustomArrow", d => {
                var result = new DiagramConnector() { Type = ConnectorType.Straight, BeginArrow = null, EndArrow = ArrowDescriptions.IndentedFilledArrow };

                result.Appearance.ContentBackground = Color.Red;
                result.Appearance.BorderColor = Color.Red;
                result.Appearance.BorderSize = 3;

                result.EndArrowSize = new SizeF(20, 20);

                return result;
            });

            diagramControl1.OptionsView.PageMargin = new Padding(0);

            cbQuality.SelectedIndex = 0;

            notifyIcon1.ShowBalloonTip(500);

            toolTip1.SetToolTip(btnSave, "Click to copy screenshot to clipboard.\r\nCTRL+Click to save screenshot file.");
            toolTip1.SetToolTip(cbQuality, "Quality of the created content (affects the resulting file size)");
        }

        private void rgSelectedTool_SelectedIndexChanged(object sender, EventArgs e) {
            switch (rgSelectedTool.SelectedIndex) {
                case 0:
                    diagramControl1.OptionsBehavior.ActiveTool = new PanTool();
                    break;
                case 1:
                    diagramControl1.OptionsBehavior.ActiveTool = new FactoryConnectorTool("CustomArrow", () => "CustomArrow", d => {
                        var result = new DiagramConnector() { Type = ConnectorType.Straight, BeginArrow = null, EndArrow = ArrowDescriptions.IndentedFilledArrow };

                        result.Appearance.ContentBackground = Color.Red;
                        result.Appearance.BorderColor = Color.Red;
                        result.Appearance.BorderSize = 3;

                        result.EndArrowSize = new SizeF(20, 20);

                        return result;
                    });
                    break;
                case 2:
                    diagramControl1.OptionsBehavior.ActiveTool = new FactoryItemTool("CustomRect", () => "CustomRect", d => {
                        var result = new DiagramShape() {
                            Shape = BasicShapes.Frame,
                            ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5F, 0.5F) }),
                        };

                        result.Appearance.BackColor = Color.FromArgb(150, Color.Blue);
                        result.Appearance.BorderColor = Color.FromArgb(150, Color.Red);
                        result.Appearance.BorderSize = 2;

                        //result.CanSelect = false;

                        return result;
                    }, BasicShapes.Frame.DefaultSize, BasicShapes.Frame.IsQuick);
                    break;
                case 3:
                    diagramControl1.OptionsBehavior.ActiveTool = new FactoryItemTool("CustomLabel", () => "CustomLabel", d => {
                        var result = new DiagramShape() {
                            Shape = BasicShapes.Rectangle,
                            ConnectionPoints = new PointCollection(new List<PointFloat>() { new PointFloat(0.5F, 0.5F) }),
                        };

                        result.Appearance.BackColor = Color.FromArgb(200, Color.Red);
                        result.Appearance.BorderColor = Color.FromArgb(200, Color.Blue);
                        result.Appearance.BorderSize = 2;
                        result.Appearance.ForeColor = Color.Black;
                        result.Appearance.Font = new Font("Consolas", 20, FontStyle.Regular);

                        return result;
                    }, BasicShapes.Rectangle.DefaultSize, BasicShapes.Rectangle.IsQuick);
                    break;
                default:
                    break;
            }
        }

        private void cbQuality_SelectedValueChanged(object sender, EventArgs e) {
            switch (cbQuality.SelectedIndex) {
                case 0:
                    quality = 20;
                    break;
                case 1:
                    quality = 50;
                    break;
                case 2:
                    quality = 80;
                    break;
                default:
                    break;
            }
        }

        private void diagramControl1_AddingNewItem(object sender, DiagramAddingNewItemEventArgs e) {
            if (e.Item is DiagramShape shp && shp.Shape.Id == BasicShapes.Rectangle.Id) {
                setTimeout<int>(param => {
                    diagramControl1.ShowEditor();
                }, 1, this, 500);
            }
        }

        private void DiagramControl1_CustomDrawBackground(object sender, CustomDrawBackgroundEventArgs e) {
            e.Graphics.FillRectangle(Brushes.LightBlue, e.TotalBounds);

            if (capturedImage != null)
                e.GraphicsCache.DrawImage(capturedImage, 0, 0, capturedImage.Width, capturedImage.Height);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            WinAPI.UnhookWindowsHookEx(_hookID);
            Application.Exit();
        }

        private static IntPtr SetHook(WinAPI.LowLevelKeyboardProc proc) {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule) {
                return WinAPI.SetWindowsHookEx(WinAPI.WH_KEYBOARD_LL, proc,
                    WinAPI.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == (IntPtr)WinAPI.WM_KEYDOWN) {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine((Keys)vkCode);

                if ((Keys)vkCode == Keys.PrintScreen) {
                    if (Application.OpenForms.OfType<FormSelection>().Count() == 0) {
                        bool ctrl = (Form.ModifierKeys & Keys.Control) == Keys.Control;
                        bool shift = (Form.ModifierKeys & Keys.Shift) == Keys.Shift;

                        if (ctrl)
                            CaptureScreencast(!shift);
                        else { 
                            if (shift)
                                CaptureActiveWindow();
                            else
                                CaptureRegion();
                        }
                    }
                }
            }

            return WinAPI.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        /*private void button1_Click(object sender, EventArgs e) {
            // Create a Bitmap to hold the captured screen
            var screenshot = new Bitmap(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);

            // Create a Graphics object from the Bitmap
            using (Graphics graphics = Graphics.FromImage(screenshot)) {
                // Capture the screen and copy it to the Bitmap
                graphics.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
            }

            capturedImage = screenshot;
            diagramControl1.OptionsView.PageSize = screenshot.Size;
            diagramControl1.Refresh();
        }*/

        private void CaptureActiveWindow() {
            // Get the handle of the active window
            IntPtr hWnd = WinAPI.GetForegroundWindow();

            // Get the dimensions of the active window
            WinAPI.RECT windowRect;
            if (!WinAPI.GetWindowRect(hWnd, out windowRect)) {
                MessageBox.Show("Failed to get the window dimensions.");
                return;
            }
            windowRect.Left += 5;

            int windowWidth = windowRect.Right - windowRect.Left - 7;
            int windowHeight = windowRect.Bottom - windowRect.Top - 7;

            // Create a Bitmap to hold the captured window
            Bitmap screenshot = new Bitmap(windowWidth, windowHeight);

            // Create a Graphics object from the Bitmap
            using (Graphics graphics = Graphics.FromImage(screenshot)) {
                graphics.CopyFromScreen(windowRect.Left, windowRect.Top, 0, 0, screenshot.Size);
                graphics.DrawRectangle(new Pen(Brushes.Black, 10), new Rectangle(0, 0, screenshot.Width, screenshot.Height));
            }
  
            this.Size = new Size(screenshot.Size.Width + 450, screenshot.Size.Height + 250);

            capturedImage = screenshot;
            diagramControl1.OptionsView.PageSize = screenshot.Size;
            diagramControl1.Refresh();
            
            diagramControl1.ScrollToPoint(
                new PointFloat(diagramControl1.OptionsView.PageSize.Width / 2, diagramControl1.OptionsView.PageSize.Height / 2), 
                HorzAlignment.Center, VertAlignment.Center);

            diagramControl1.OptionsView.ZoomFactor = 0.8f;

            Show();
            Activate();
            WindowState = FormWindowState.Normal;
        }

        private void CaptureRegion() {
            Hide();

            using (var selectionForm = new FormSelection()) {
                selectionForm.RegionCaptured += (s, args) => {
                    this.Size = new Size(args.SelectedRegion.Width + 450, args.SelectedRegion.Height + 250);

                    diagramControl1.Items.Clear();

                    capturedImage = (Image)args.SelectedImage; 
                    diagramControl1.OptionsView.PageSize = args.SelectedRegion.Size;
                    diagramControl1.Refresh();

                    diagramControl1.ScrollToPoint(
                        new PointFloat(diagramControl1.OptionsView.PageSize.Width / 2, diagramControl1.OptionsView.PageSize.Height / 2),
                        HorzAlignment.Center, VertAlignment.Center);

                    diagramControl1.OptionsView.ZoomFactor = 1f;

                    Show();
                    Activate();
                    WindowState = FormWindowState.Normal;
                };

                selectionForm.CaptureRegion();
            }
        }

        private void CaptureScreencast(bool useRegionSelector) {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), GetFileNamePrefix() + "screencast.mp4");

            if (File.Exists(path)) {
                MessageBox.Show(string.Format("The '{0}' file already exists.", path), "DXScreenCapture");
                return;
            }

            this.Hide();

            desktopDC = WinAPI.GetDC(IntPtr.Zero);
            desktopGraphics  = Graphics.FromHdc(desktopDC);

            if (useRegionSelector) {
                using (var selectionForm = new FormSelection()) {
                    selectionForm.RegionCaptured += (s, args) => {
                        InitScreencastRecording(path, args.SelectedRegion);
                    };

                    selectionForm.CaptureRegion();
                }
            }
            else {
                IntPtr hWnd = WinAPI.GetForegroundWindow();
                // Get the dimensions of the active window
                WinAPI.RECT windowRect;
                if (!WinAPI.GetWindowRect(hWnd, out windowRect)) {
                    MessageBox.Show("Failed to get the window dimensions.");
                    return;
                }
                windowRect.Left += 5;

                InitScreencastRecording(path,
                    new Rectangle(windowRect.Left, windowRect.Top,
                    windowRect.Right - windowRect.Left - 7,
                    windowRect.Bottom - windowRect.Top - 7));
            }
        }

        private void InitScreencastRecording(string path, Rectangle rect) {
            videoWriter = new VideoFileWriter();
            videoWriter.Open(path, RoundTo2(rect.Width), RoundTo2(rect.Height), 25, VideoCodec.H264, 1000000);
            videoWriterTimer.Tag = rect;
            videoWriterTimer.Start();
            var frmRecorder = new FormRecorder();
            frmRecorder.Location = new Point(rect.Left, rect.Bottom);
            frmRecorder.StopRecording += (a, b) => {
                videoWriterTimer.Stop();
                videoWriter.Close();
                desktopGraphics.Dispose();
                WinAPI.ReleaseDC(IntPtr.Zero, desktopDC);
                WinAPI.InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);
                Clipboard.SetText(path);
            };
            frmRecorder.CancelRecording += (a, b) => {
                videoWriterTimer.Stop();
                videoWriter.Close();
                desktopGraphics.Dispose();
                WinAPI.ReleaseDC(IntPtr.Zero, desktopDC);
                WinAPI.InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);
                File.Delete(path);
            };
            frmRecorder.Show();
        }

        private int RoundTo2(int value) {
            return value % 2 == 1 ? value + 1 : value;
        }

        private void VideoWriterTimer_Tick(object sender, EventArgs e) {
            var rect = (Rectangle)videoWriterTimer.Tag;
            var bitmap = new Bitmap(RoundTo2(rect.Width), RoundTo2(rect.Height));
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(bitmap.Width, bitmap.Height));

            Cursors.Arrow.Draw(graphics, new Rectangle((int)(Cursor.Position.X - rect.Left), (int)(Cursor.Position.Y - rect.Top), Cursors.Arrow.Size.Width, Cursors.Arrow.Size.Height));
            
            videoWriter.WriteVideoFrame((Bitmap)CompressImage(bitmap, quality));

            WinAPI.InvalidateRect(IntPtr.Zero, IntPtr.Zero, true);

            desktopGraphics.DrawRectangle(new Pen(Color.Red, 2), rect);

            /*desktopGraphics.DrawLines(new Pen(Color.Red, 2), new Point[] {
                rect.Location, 
                new Point(rect.Left + rect.Width, rect.Top),
                new Point(rect.Left + rect.Width, rect.Top + rect.Height),
                new Point(rect.Left, rect.Top + rect.Height)
            });*/
        }

        private void btnSave_Click(object sender, EventArgs e) {
            var memoryStream = new MemoryStream();
            diagramControl1.ExportToImage(memoryStream,  DiagramImageExportFormat.JPEG);
            var compressed = CompressImage(Image.FromStream(memoryStream), quality);

            // Code for old version:
            //var image = new Bitmap(diagramControl1.Width, diagramControl1.Height);
            //diagramControl1.DrawToBitmap(image, diagramControl1.Bounds);
            //var compressed = CompressImage(image, quality);

            Clipboard.SetImage(compressed);

            if ((Form.ModifierKeys & Keys.Control) == Keys.Control) {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), GetFileNamePrefix() + "screenshot.jpg");

                if (File.Exists(path)) {
                    MessageBox.Show(string.Format("The '{0}' file already exists.", path), "DXScreenCapture");
                    compressed.Dispose();
                    return;
                }
                else {
                    compressed.Save(path);
                }
            }

            compressed.Dispose();

            this.Hide();
        }

        public static Image CompressImage(Image image, int quality) {
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            var encoder = GetEncoder(ImageFormat.Jpeg);
            var memoryStream = new MemoryStream();

            image.Save(memoryStream, encoder, encoderParameters);

            return Image.FromStream(memoryStream);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format) {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var codec in codecs) {
                if (codec.FormatID == format.Guid)
                    return codec;
            }

            return null;
        }

        public void setTimeout<T>(Action<T> action, T param, Control control, int timeout) {
            var thread = new Thread(() => {
                Thread.Sleep(timeout);
                control.BeginInvoke(action, param);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                if (Visible)
                    Hide();
                else {
                    Show();
                    Activate();
                    WindowState = FormWindowState.Normal;
                };
            }
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private string GetFileNamePrefix() {
            var prefix = string.Format("{0:yyyy-MM-dd}_", DateTime.Today);

            return prefix;
        }
    }
}