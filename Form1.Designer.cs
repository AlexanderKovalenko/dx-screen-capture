
namespace DXScreenCapture {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.diagramControl1 = new DXScreenCapture.CustomDiagramControl();
            this.btnSave = new System.Windows.Forms.Button();
            this.rgSelectedTool = new DevExpress.XtraEditors.RadioGroup();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbQuality = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgSelectedTool.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbQuality.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // diagramControl1
            // 
            this.diagramControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.diagramControl1.Location = new System.Drawing.Point(127, 12);
            this.diagramControl1.Margin = new System.Windows.Forms.Padding(15);
            this.diagramControl1.Name = "diagramControl1";
            this.diagramControl1.OptionsBehavior.SelectedStencils = new DevExpress.Diagram.Core.StencilCollection(new string[] {
            "BasicShapes",
            "BasicFlowchartShapes"});
            this.diagramControl1.OptionsView.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            this.diagramControl1.Size = new System.Drawing.Size(882, 534);
            this.diagramControl1.TabIndex = 3;
            this.diagramControl1.Text = "diagramControl1";
            this.diagramControl1.AddingNewItem += new System.EventHandler<DevExpress.XtraDiagram.DiagramAddingNewItemEventArgs>(this.diagramControl1_AddingNewItem);
            this.diagramControl1.CustomDrawBackground += new System.EventHandler<DevExpress.XtraDiagram.CustomDrawBackgroundEventArgs>(this.DiagramControl1_CustomDrawBackground);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(13, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(98, 25);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Copy/Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // rgSelectedTool
            // 
            this.rgSelectedTool.EditValue = "Arrow";
            this.rgSelectedTool.Location = new System.Drawing.Point(13, 52);
            this.rgSelectedTool.Margin = new System.Windows.Forms.Padding(12);
            this.rgSelectedTool.Name = "rgSelectedTool";
            this.rgSelectedTool.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem("Move", "<Move>"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("Arrow", "Arrow"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("Rect", "Rect"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem("Label", "Label")});
            this.rgSelectedTool.Size = new System.Drawing.Size(98, 136);
            this.rgSelectedTool.TabIndex = 5;
            this.rgSelectedTool.SelectedIndexChanged += new System.EventHandler(this.rgSelectedTool_SelectedIndexChanged);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "Use PrtScn or CTRL + PrtScn buttons to capture screenshots or screencasts.\r\nHold Shift button to capture from the active window.";
            this.notifyIcon1.BalloonTipTitle = "DXScreenCapture";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // cbQuality
            // 
            this.cbQuality.Location = new System.Drawing.Point(13, 203);
            this.cbQuality.Name = "cbQuality";
            this.cbQuality.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbQuality.Properties.Items.AddRange(new object[] {
            "Low",
            "Medium",
            "High"});
            this.cbQuality.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbQuality.Size = new System.Drawing.Size(98, 22);
            this.cbQuality.TabIndex = 7;
            this.cbQuality.SelectedValueChanged += new System.EventHandler(this.cbQuality_SelectedValueChanged);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1033, 570);
            this.Controls.Add(this.cbQuality);
            this.Controls.Add(this.rgSelectedTool);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.diagramControl1);
            this.Name = "Form1";
            this.Text = "DXScreenCapture";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.diagramControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgSelectedTool.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbQuality.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private CustomDiagramControl diagramControl1;
        private System.Windows.Forms.Button btnSave;
        private DevExpress.XtraEditors.RadioGroup rgSelectedTool;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolTip toolTip1;
        private DevExpress.XtraEditors.ComboBoxEdit cbQuality;
    }
}

