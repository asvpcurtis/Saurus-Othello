﻿namespace Saurus
{
    partial class SaurusForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.boardPbx = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.boardPbx)).BeginInit();
            this.SuspendLayout();
            // 
            // boardPbx
            // 
            
            this.boardPbx.Location = new System.Drawing.Point(159, 140);
            this.boardPbx.Name = "boardPbx";
            this.boardPbx.Size = new System.Drawing.Size(600, 600);
            this.boardPbx.TabIndex = 0;
            this.boardPbx.TabStop = false;
            this.boardPbx.Click += new System.EventHandler(this.boardPbx_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 929);
            this.Controls.Add(this.boardPbx);
            this.Name = "Saurus";
            this.Text = "Saurus";
            ((System.ComponentModel.ISupportInitialize)(this.boardPbx)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox boardPbx;
    }
}

