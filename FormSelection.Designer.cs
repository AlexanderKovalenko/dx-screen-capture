﻿namespace DXScreenCapture {
    partial class FormSelection {
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
            this.SuspendLayout();
            // 
            // FormSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "FormSelection";
            this.Text = "FormSelection";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormSelection_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormSelection_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormSelection_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormSelection_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormSelection_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}