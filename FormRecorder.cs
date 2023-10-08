using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXScreenCapture {
    public partial class FormRecorder : Form {
        public event EventHandler<EventArgs> StopRecording;
        public event EventHandler<EventArgs> CancelRecording;

        public FormRecorder() {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, EventArgs e) {
            StopRecording?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            CancelRecording?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}