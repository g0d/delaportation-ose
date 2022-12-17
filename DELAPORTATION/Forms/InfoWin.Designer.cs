namespace DELAPORTATION
{
    partial class InfoWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoWin));
            this.NotificationMsg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CancelTeleportButton = new System.Windows.Forms.Button();
            this.NoNotificationsCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // NotificationMsg
            // 
            this.NotificationMsg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.NotificationMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotificationMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NotificationMsg.Location = new System.Drawing.Point(12, 44);
            this.NotificationMsg.Name = "NotificationMsg";
            this.NotificationMsg.Size = new System.Drawing.Size(376, 83);
            this.NotificationMsg.TabIndex = 0;
            this.NotificationMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.NotificationMsg.Click += new System.EventHandler(this.NotificationMsg_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(376, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = ":: DELAPORTATION ::";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // CancelTeleportButton
            // 
            this.CancelTeleportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.CancelTeleportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelTeleportButton.Location = new System.Drawing.Point(336, 137);
            this.CancelTeleportButton.Name = "CancelTeleportButton";
            this.CancelTeleportButton.Size = new System.Drawing.Size(52, 23);
            this.CancelTeleportButton.TabIndex = 3;
            this.CancelTeleportButton.Text = "Cancel";
            this.CancelTeleportButton.UseVisualStyleBackColor = false;
            this.CancelTeleportButton.Click += new System.EventHandler(this.CancelTeleportButton_Click);
            // 
            // NoNotificationsCheckBox
            // 
            this.NoNotificationsCheckBox.AutoSize = true;
            this.NoNotificationsCheckBox.BackColor = System.Drawing.SystemColors.Control;
            this.NoNotificationsCheckBox.Location = new System.Drawing.Point(12, 143);
            this.NoNotificationsCheckBox.Name = "NoNotificationsCheckBox";
            this.NoNotificationsCheckBox.Size = new System.Drawing.Size(210, 17);
            this.NoNotificationsCheckBox.TabIndex = 4;
            this.NoNotificationsCheckBox.Text = "Don\'t show progress notifications again";
            this.NoNotificationsCheckBox.UseVisualStyleBackColor = false;
            this.NoNotificationsCheckBox.CheckedChanged += new System.EventHandler(this.NoNotificationsCheckBox_CheckedChanged);
            // 
            // InfoWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 167);
            this.ControlBox = false;
            this.Controls.Add(this.NoNotificationsCheckBox);
            this.Controls.Add(this.CancelTeleportButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NotificationMsg);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoWin";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DELAPORTATION - Teleport your files anywhere!";
            this.TopMost = true;
            this.Click += new System.EventHandler(this.InfoWin_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NotificationMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CancelTeleportButton;
        private System.Windows.Forms.CheckBox NoNotificationsCheckBox;
    }
}

