using System.Windows.Forms;
using DevExpress.XtraDiagram;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Native;

namespace DXScreenCapture {
    public class CustomDiagramControl : DiagramControl, IDiagramControl {
        protected override void OnMouseWheel(MouseEventArgs e) {
            ((DiagramControlHandlerEx)this.DiagramHandler).PublicDoZoom(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button == MouseButtons.Middle) {
                this.OptionsBehavior.ActiveTool = this.OptionsBehavior.PanTool;
                base.OnMouseDown(new MouseEventArgs(MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta));
                return;
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            if (e.Button == MouseButtons.Middle) {
                this.OptionsBehavior.ActiveTool = this.OptionsBehavior.PointerTool;
            }
            base.OnMouseUp(e);
        }
        protected override DiagramControlHandler CreateDiagramHandler() {
            return new DiagramControlHandlerEx(this);
        }
    }

    public class DiagramControlHandlerEx : DiagramControlHandler {
        public DiagramControlHandlerEx(DiagramControl diagram) : base(diagram) {

        }
        public void PublicDoZoom(MouseEventArgs e) {
            this.DoZoom(e);
        }
    }
}