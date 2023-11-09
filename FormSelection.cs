using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXScreenCapture {
    public partial class FormSelection : Form {
        private Point startPoint;
        private Rectangle selectionRect;
        private bool isSelecting = false;
        private Bitmap entireScreen;

        private static SolidBrush brush = new SolidBrush(Color.FromArgb(128, Color.Blue));
        private static Pen pen = new Pen(Color.Red, 2) { DashStyle = DashStyle.Dash };

        public event EventHandler<RegionCapturedEventArgs> RegionCaptured;

        public FormSelection() {
            InitializeComponent();

            TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            Cursor = Cursors.Cross;

            var doubleBufferedProp = typeof(System.Windows.Forms.Control).GetProperty(
                "DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance);

            doubleBufferedProp.SetValue(this, true, null);
        }

        private void FormSelection_MouseDown(object sender, MouseEventArgs e) {
            // Start the selection
            isSelecting = true;
            startPoint = e.Location;
        }

        private void FormSelection_MouseMove(object sender, MouseEventArgs e) {
            if (isSelecting) {
                // Update the selection rectangle
                selectionRect = new Rectangle(
                    Math.Min(startPoint.X, e.Location.X),
                    Math.Min(startPoint.Y, e.Location.Y),
                    Math.Abs(startPoint.X - e.Location.X),
                    Math.Abs(startPoint.Y - e.Location.Y));

                // Redraw the form with the updated selection
                Invalidate();
            }
        }

        private void FormSelection_MouseUp(object sender, MouseEventArgs e) {
            // Stop the selection
            isSelecting = false;

            if (selectionRect.Width > 10 && selectionRect.Height > 10) {
                // Capture the selected region from the screenshot
                using (Bitmap selectedRegion = new Bitmap(selectionRect.Width, selectionRect.Height))
                using (Graphics selectedGraphics = Graphics.FromImage(selectedRegion)) {
                    selectedGraphics.DrawImage(entireScreen, new Rectangle(0, 0, selectionRect.Width, selectionRect.Height),
                        selectionRect, GraphicsUnit.Pixel);

                    selectedGraphics.DrawRectangle(new Pen(Brushes.Black, 10), new Rectangle(0, 0, selectedRegion.Width, selectedRegion.Height));

                    RegionCaptured?.Invoke(this, new RegionCapturedEventArgs() { 
                        SelectedRegion = selectionRect,
                        SelectedImage = (Bitmap)selectedRegion.Clone() // selectedRegion is disposed!
                    });
                }
            }

            // Close the selection form
            Close();
        }

        private void FormSelection_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void FormSelection_Paint(object sender, PaintEventArgs e) {
            // Draw the screenshot on the form
            e.Graphics.DrawImage(entireScreen, Point.Empty);

            // Highlight the selected region
            e.Graphics.FillRectangle(brush, selectionRect);
            e.Graphics.DrawRectangle(pen, selectionRect);
        }

        public void CaptureRegion() {
            selectionRect = Rectangle.Empty;

            entireScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            
            using (Graphics graphics = Graphics.FromImage(entireScreen)) {
                // Capture the entire screen
                graphics.CopyFromScreen(0, 0, 0, 0, entireScreen.Size);
            }

            ShowDialog();
            entireScreen.Dispose();
        }
    }

    public class RegionCapturedEventArgs : EventArgs {
        public Rectangle SelectedRegion { get; set; }
        public Bitmap SelectedImage { get; set; }
    }
}