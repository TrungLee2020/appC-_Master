namespace MainWindow.MultiCamera
{
    partial class multicam
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(multicam));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUseNum = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbDevNum = new System.Windows.Forms.TextBox();
            this.bnOpen = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.bnSetParam = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbGain = new System.Windows.Forms.TextBox();
            this.tbExposure = new System.Windows.Forms.TextBox();
            this.bnClose = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbHardTrigger = new System.Windows.Forms.CheckBox();
            this.bnTriggerExec = new System.Windows.Forms.Button();
            this.cbSoftTrigger = new System.Windows.Forms.CheckBox();
            this.bnSaveBmp = new System.Windows.Forms.Button();
            this.bnStopGrab = new System.Windows.Forms.Button();
            this.bnStartGrab = new System.Windows.Forms.Button();
            this.bnTriggerMode = new System.Windows.Forms.RadioButton();
            this.bnContinuesMode = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.tbGrabFrame1 = new System.Windows.Forms.TextBox();
            this.tbGrabFrame2 = new System.Windows.Forms.TextBox();
            this.tbLostFrame1 = new System.Windows.Forms.TextBox();
            this.tbLostFrame2 = new System.Windows.Forms.TextBox();
            this.tbLostFrame4 = new System.Windows.Forms.TextBox();
            this.tbGrabFrame3 = new System.Windows.Forms.TextBox();
            this.tbGrabFrame4 = new System.Windows.Forms.TextBox();
            this.tbLostFrame3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.bnAuto = new System.Windows.Forms.Button();
            this.bnManual = new System.Windows.Forms.Button();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox1.Location = new System.Drawing.Point(17, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(361, 356);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbUseNum);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbDevNum);
            this.groupBox1.Controls.Add(this.bnOpen);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Location = new System.Drawing.Point(759, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 254);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Initialization";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 14);
            this.label5.TabIndex = 12;
            this.label5.Text = "Used Device";
            // 
            // tbUseNum
            // 
            this.tbUseNum.Location = new System.Drawing.Point(106, 80);
            this.tbUseNum.Name = "tbUseNum";
            this.tbUseNum.Size = new System.Drawing.Size(36, 22);
            this.tbUseNum.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 14);
            this.label4.TabIndex = 10;
            this.label4.Text = "Online Device";
            // 
            // tbDevNum
            // 
            this.tbDevNum.Location = new System.Drawing.Point(106, 34);
            this.tbDevNum.Name = "tbDevNum";
            this.tbDevNum.Size = new System.Drawing.Size(36, 22);
            this.tbDevNum.TabIndex = 9;
            // 
            // bnOpen
            // 
            this.bnOpen.Location = new System.Drawing.Point(148, 45);
            this.bnOpen.Name = "bnOpen";
            this.bnOpen.Size = new System.Drawing.Size(87, 54);
            this.bnOpen.TabIndex = 1;
            this.bnOpen.Text = "Camera Initialization";
            this.bnOpen.UseVisualStyleBackColor = true;
            this.bnOpen.Click += new System.EventHandler(this.bnOpen_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.bnSetParam);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.tbGain);
            this.groupBox4.Controls.Add(this.tbExposure);
            this.groupBox4.Location = new System.Drawing.Point(6, 134);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(239, 110);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Parameters";
            // 
            // bnSetParam
            // 
            this.bnSetParam.Enabled = false;
            this.bnSetParam.Location = new System.Drawing.Point(154, 30);
            this.bnSetParam.Name = "bnSetParam";
            this.bnSetParam.Size = new System.Drawing.Size(75, 57);
            this.bnSetParam.TabIndex = 7;
            this.bnSetParam.Text = "Set Parameter";
            this.bnSetParam.UseVisualStyleBackColor = true;
            this.bnSetParam.Click += new System.EventHandler(this.bnSetParam_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "Gain";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "Exposure Time";
            // 
            // tbGain
            // 
            this.tbGain.Enabled = false;
            this.tbGain.Location = new System.Drawing.Point(93, 66);
            this.tbGain.Name = "tbGain";
            this.tbGain.Size = new System.Drawing.Size(48, 22);
            this.tbGain.TabIndex = 1;
            // 
            // tbExposure
            // 
            this.tbExposure.Enabled = false;
            this.tbExposure.Location = new System.Drawing.Point(94, 30);
            this.tbExposure.Name = "tbExposure";
            this.tbExposure.Size = new System.Drawing.Size(48, 22);
            this.tbExposure.TabIndex = 0;
            // 
            // bnClose
            // 
            this.bnClose.Enabled = false;
            this.bnClose.Location = new System.Drawing.Point(143, 167);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(93, 27);
            this.bnClose.TabIndex = 2;
            this.bnClose.Text = "Close Device";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbHardTrigger);
            this.groupBox2.Controls.Add(this.bnTriggerExec);
            this.groupBox2.Controls.Add(this.cbSoftTrigger);
            this.groupBox2.Controls.Add(this.bnSaveBmp);
            this.groupBox2.Controls.Add(this.bnStopGrab);
            this.groupBox2.Controls.Add(this.bnStartGrab);
            this.groupBox2.Controls.Add(this.bnTriggerMode);
            this.groupBox2.Controls.Add(this.bnContinuesMode);
            this.groupBox2.Controls.Add(this.bnClose);
            this.groupBox2.Location = new System.Drawing.Point(758, 308);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(254, 213);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image Acquisition";
            // 
            // cbHardTrigger
            // 
            this.cbHardTrigger.AutoSize = true;
            this.cbHardTrigger.Location = new System.Drawing.Point(25, 113);
            this.cbHardTrigger.Name = "cbHardTrigger";
            this.cbHardTrigger.Size = new System.Drawing.Size(118, 18);
            this.cbHardTrigger.TabIndex = 26;
            this.cbHardTrigger.Text = "Hardware Trigger";
            this.cbHardTrigger.UseVisualStyleBackColor = true;
            this.cbHardTrigger.CheckedChanged += new System.EventHandler(this.cbHardTrigger_CheckedChanged);
            // 
            // bnTriggerExec
            // 
            this.bnTriggerExec.Enabled = false;
            this.bnTriggerExec.Location = new System.Drawing.Point(143, 113);
            this.bnTriggerExec.Name = "bnTriggerExec";
            this.bnTriggerExec.Size = new System.Drawing.Size(93, 41);
            this.bnTriggerExec.TabIndex = 5;
            this.bnTriggerExec.Text = "Trigger by Software Once";
            this.bnTriggerExec.UseVisualStyleBackColor = true;
            this.bnTriggerExec.Click += new System.EventHandler(this.bnTriggerExec_Click);
            // 
            // cbSoftTrigger
            // 
            this.cbSoftTrigger.AutoSize = true;
            this.cbSoftTrigger.Enabled = false;
            this.cbSoftTrigger.Location = new System.Drawing.Point(25, 143);
            this.cbSoftTrigger.Name = "cbSoftTrigger";
            this.cbSoftTrigger.Size = new System.Drawing.Size(112, 18);
            this.cbSoftTrigger.TabIndex = 4;
            this.cbSoftTrigger.Text = "Software Trigger";
            this.cbSoftTrigger.UseVisualStyleBackColor = true;
            this.cbSoftTrigger.CheckedChanged += new System.EventHandler(this.cbSoftTrigger_CheckedChanged);
            // 
            // bnSaveBmp
            // 
            this.bnSaveBmp.Enabled = false;
            this.bnSaveBmp.Location = new System.Drawing.Point(33, 167);
            this.bnSaveBmp.Name = "bnSaveBmp";
            this.bnSaveBmp.Size = new System.Drawing.Size(86, 27);
            this.bnSaveBmp.TabIndex = 0;
            this.bnSaveBmp.Text = "Save Image";
            this.bnSaveBmp.UseVisualStyleBackColor = true;
            this.bnSaveBmp.Click += new System.EventHandler(this.bnSaveBmp_Click);
            // 
            // bnStopGrab
            // 
            this.bnStopGrab.Enabled = false;
            this.bnStopGrab.Location = new System.Drawing.Point(143, 77);
            this.bnStopGrab.Name = "bnStopGrab";
            this.bnStopGrab.Size = new System.Drawing.Size(93, 27);
            this.bnStopGrab.TabIndex = 3;
            this.bnStopGrab.Text = "Stop";
            this.bnStopGrab.UseVisualStyleBackColor = true;
            this.bnStopGrab.Click += new System.EventHandler(this.bnStopGrab_Click);
            // 
            // bnStartGrab
            // 
            this.bnStartGrab.Enabled = false;
            this.bnStartGrab.Location = new System.Drawing.Point(33, 77);
            this.bnStartGrab.Name = "bnStartGrab";
            this.bnStartGrab.Size = new System.Drawing.Size(86, 27);
            this.bnStartGrab.TabIndex = 2;
            this.bnStartGrab.Text = "Start";
            this.bnStartGrab.UseVisualStyleBackColor = true;
            this.bnStartGrab.Click += new System.EventHandler(this.bnStartGrab_Click);
            // 
            // bnTriggerMode
            // 
            this.bnTriggerMode.AutoSize = true;
            this.bnTriggerMode.Enabled = false;
            this.bnTriggerMode.Location = new System.Drawing.Point(147, 35);
            this.bnTriggerMode.Name = "bnTriggerMode";
            this.bnTriggerMode.Size = new System.Drawing.Size(95, 18);
            this.bnTriggerMode.TabIndex = 1;
            this.bnTriggerMode.TabStop = true;
            this.bnTriggerMode.Text = "Trigger Mode";
            this.bnTriggerMode.UseVisualStyleBackColor = true;
            this.bnTriggerMode.CheckedChanged += new System.EventHandler(this.bnTriggerMode_CheckedChanged);
            // 
            // bnContinuesMode
            // 
            this.bnContinuesMode.AutoSize = true;
            this.bnContinuesMode.Enabled = false;
            this.bnContinuesMode.Location = new System.Drawing.Point(33, 35);
            this.bnContinuesMode.Name = "bnContinuesMode";
            this.bnContinuesMode.Size = new System.Drawing.Size(86, 18);
            this.bnContinuesMode.TabIndex = 0;
            this.bnContinuesMode.TabStop = true;
            this.bnContinuesMode.Text = "Continuous";
            this.bnContinuesMode.UseVisualStyleBackColor = true;
            this.bnContinuesMode.CheckedChanged += new System.EventHandler(this.bnContinuesMode_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox2.Location = new System.Drawing.Point(384, 28);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(361, 356);
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox3.Location = new System.Drawing.Point(17, 395);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(361, 356);
            this.pictureBox3.TabIndex = 7;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox4.Location = new System.Drawing.Point(384, 395);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(361, 356);
            this.pictureBox4.TabIndex = 8;
            this.pictureBox4.TabStop = false;
            // 
            // tbGrabFrame1
            // 
            this.tbGrabFrame1.Location = new System.Drawing.Point(1101, 78);
            this.tbGrabFrame1.Name = "tbGrabFrame1";
            this.tbGrabFrame1.Size = new System.Drawing.Size(91, 22);
            this.tbGrabFrame1.TabIndex = 9;
            // 
            // tbGrabFrame2
            // 
            this.tbGrabFrame2.Location = new System.Drawing.Point(1101, 127);
            this.tbGrabFrame2.Name = "tbGrabFrame2";
            this.tbGrabFrame2.Size = new System.Drawing.Size(91, 22);
            this.tbGrabFrame2.TabIndex = 10;
            // 
            // tbLostFrame1
            // 
            this.tbLostFrame1.Location = new System.Drawing.Point(1210, 78);
            this.tbLostFrame1.Name = "tbLostFrame1";
            this.tbLostFrame1.Size = new System.Drawing.Size(64, 22);
            this.tbLostFrame1.TabIndex = 11;
            // 
            // tbLostFrame2
            // 
            this.tbLostFrame2.Location = new System.Drawing.Point(1210, 127);
            this.tbLostFrame2.Name = "tbLostFrame2";
            this.tbLostFrame2.Size = new System.Drawing.Size(64, 22);
            this.tbLostFrame2.TabIndex = 12;
            // 
            // tbLostFrame4
            // 
            this.tbLostFrame4.Location = new System.Drawing.Point(1210, 223);
            this.tbLostFrame4.Name = "tbLostFrame4";
            this.tbLostFrame4.Size = new System.Drawing.Size(64, 22);
            this.tbLostFrame4.TabIndex = 13;
            // 
            // tbGrabFrame3
            // 
            this.tbGrabFrame3.Location = new System.Drawing.Point(1101, 175);
            this.tbGrabFrame3.Name = "tbGrabFrame3";
            this.tbGrabFrame3.Size = new System.Drawing.Size(91, 22);
            this.tbGrabFrame3.TabIndex = 14;
            // 
            // tbGrabFrame4
            // 
            this.tbGrabFrame4.Location = new System.Drawing.Point(1101, 223);
            this.tbGrabFrame4.Name = "tbGrabFrame4";
            this.tbGrabFrame4.Size = new System.Drawing.Size(91, 22);
            this.tbGrabFrame4.TabIndex = 15;
            // 
            // tbLostFrame3
            // 
            this.tbLostFrame3.Location = new System.Drawing.Point(1210, 175);
            this.tbLostFrame3.Name = "tbLostFrame3";
            this.tbLostFrame3.Size = new System.Drawing.Size(64, 22);
            this.tbLostFrame3.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1094, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 14);
            this.label6.TabIndex = 17;
            this.label6.Text = "Acquired Frames";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1207, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 14);
            this.label7.TabIndex = 18;
            this.label7.Text = "Lost Frames";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1038, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 14);
            this.label3.TabIndex = 19;
            this.label3.Text = "Camera 1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1038, 130);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 14);
            this.label8.TabIndex = 20;
            this.label8.Text = "Camera 2";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1038, 178);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 14);
            this.label9.TabIndex = 21;
            this.label9.Text = "Camera 3";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1038, 226);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 14);
            this.label10.TabIndex = 22;
            this.label10.Text = "Camera 4";
            // 
            // bnAuto
            // 
            this.bnAuto.Enabled = false;
            this.bnAuto.Location = new System.Drawing.Point(1038, 334);
            this.bnAuto.Name = "bnAuto";
            this.bnAuto.Size = new System.Drawing.Size(86, 27);
            this.bnAuto.TabIndex = 23;
            this.bnAuto.Text = "Auto";
            this.bnAuto.UseVisualStyleBackColor = true;
            this.bnAuto.Click += new System.EventHandler(this.bnAuto_Click);
            // 
            // bnManual
            // 
            this.bnManual.Enabled = false;
            this.bnManual.Location = new System.Drawing.Point(1038, 385);
            this.bnManual.Name = "bnManual";
            this.bnManual.Size = new System.Drawing.Size(86, 27);
            this.bnManual.TabIndex = 24;
            this.bnManual.Text = "Manual";
            this.bnManual.UseVisualStyleBackColor = true;
            this.bnManual.Click += new System.EventHandler(this.bnManual_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Location = new System.Drawing.Point(759, 540);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(515, 211);
            this.richTextBox.TabIndex = 25;
            this.richTextBox.Text = "";
            this.richTextBox.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // multicam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1325, 788);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.bnManual);
            this.Controls.Add(this.bnAuto);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbLostFrame3);
            this.Controls.Add(this.tbGrabFrame4);
            this.Controls.Add(this.tbGrabFrame3);
            this.Controls.Add(this.tbLostFrame4);
            this.Controls.Add(this.tbLostFrame2);
            this.Controls.Add(this.tbLostFrame1);
            this.Controls.Add(this.tbGrabFrame2);
            this.Controls.Add(this.tbGrabFrame1);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "multicam";
            this.Text = "Vision System";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bnClose;
        private System.Windows.Forms.Button bnOpen;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton bnTriggerMode;
        private System.Windows.Forms.RadioButton bnContinuesMode;
        private System.Windows.Forms.CheckBox cbHardTrigger;
        private System.Windows.Forms.CheckBox cbSoftTrigger;
        private System.Windows.Forms.Button bnStopGrab;
        private System.Windows.Forms.Button bnStartGrab;
        private System.Windows.Forms.Button bnTriggerExec;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbGain;
        private System.Windows.Forms.TextBox tbExposure;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bnSetParam;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button bnSaveBmp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbDevNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbUseNum;
        private System.Windows.Forms.TextBox tbGrabFrame1;
        private System.Windows.Forms.TextBox tbGrabFrame2;
        private System.Windows.Forms.TextBox tbLostFrame1;
        private System.Windows.Forms.TextBox tbLostFrame2;
        private System.Windows.Forms.TextBox tbLostFrame4;
        private System.Windows.Forms.TextBox tbGrabFrame3;
        private System.Windows.Forms.TextBox tbGrabFrame4;
        private System.Windows.Forms.TextBox tbLostFrame3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button bnAuto;
        private System.Windows.Forms.Button bnManual;
        private System.Windows.Forms.RichTextBox richTextBox;
        
    }
}

