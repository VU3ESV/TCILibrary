namespace StationMonitor
{
    partial class StationMonitor
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
            this.tciServerIP = new System.Windows.Forms.TextBox();
            this.tciServerPort = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.Receiver1VfoA = new System.Windows.Forms.Label();
            this.Receiver1VfoB = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Receiver2VfoB = new System.Windows.Forms.Label();
            this.Receiver2VfoA = new System.Windows.Forms.Label();
            this.Tx = new System.Windows.Forms.Label();
            this.Drive = new System.Windows.Forms.Label();
            this.Tune = new System.Windows.Forms.Label();
            this.Modulation = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.Volume = new System.Windows.Forms.Label();
            this.VolumeControl = new System.Windows.Forms.TrackBar();
            this.DriveControl = new System.Windows.Forms.TrackBar();
            this.TuneControl = new System.Windows.Forms.TrackBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Tr1ModulationValue = new System.Windows.Forms.Label();
            this.Tr2ModulationValue = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DriveControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TuneControl)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tciServerIP
            // 
            this.tciServerIP.Location = new System.Drawing.Point(70, 27);
            this.tciServerIP.Name = "tciServerIP";
            this.tciServerIP.Size = new System.Drawing.Size(229, 20);
            this.tciServerIP.TabIndex = 0;
            this.tciServerIP.Text = "localhost";
            // 
            // tciServerPort
            // 
            this.tciServerPort.Location = new System.Drawing.Point(323, 29);
            this.tciServerPort.Name = "tciServerPort";
            this.tciServerPort.Size = new System.Drawing.Size(130, 20);
            this.tciServerPort.TabIndex = 1;
            this.tciServerPort.Text = "40001";
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(498, 30);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(88, 25);
            this.ConnectButton.TabIndex = 2;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(599, 31);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(85, 23);
            this.DisconnectButton.TabIndex = 3;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // Receiver1VfoA
            // 
            this.Receiver1VfoA.AutoSize = true;
            this.Receiver1VfoA.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Receiver1VfoA.Location = new System.Drawing.Point(15, 15);
            this.Receiver1VfoA.Name = "Receiver1VfoA";
            this.Receiver1VfoA.Size = new System.Drawing.Size(198, 25);
            this.Receiver1VfoA.TabIndex = 4;
            this.Receiver1VfoA.Text = "Receiver- 1 Vfo-A";
            // 
            // Receiver1VfoB
            // 
            this.Receiver1VfoB.AutoSize = true;
            this.Receiver1VfoB.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Receiver1VfoB.Location = new System.Drawing.Point(17, 74);
            this.Receiver1VfoB.Name = "Receiver1VfoB";
            this.Receiver1VfoB.Size = new System.Drawing.Size(198, 25);
            this.Receiver1VfoB.TabIndex = 5;
            this.Receiver1VfoB.Text = "Receiver- 1 Vfo-B";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(516, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Receiver- 2 Vfo-B";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(365, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Receiver- 2 Vfo-A";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Receiver1VfoB);
            this.groupBox1.Controls.Add(this.Receiver1VfoA);
            this.groupBox1.Location = new System.Drawing.Point(58, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 125);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transceiver -1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Receiver2VfoB);
            this.groupBox2.Controls.Add(this.Receiver2VfoA);
            this.groupBox2.Location = new System.Drawing.Point(368, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(284, 125);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Transceiver -2";
            // 
            // Receiver2VfoB
            // 
            this.Receiver2VfoB.AutoSize = true;
            this.Receiver2VfoB.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Receiver2VfoB.Location = new System.Drawing.Point(15, 74);
            this.Receiver2VfoB.Name = "Receiver2VfoB";
            this.Receiver2VfoB.Size = new System.Drawing.Size(198, 25);
            this.Receiver2VfoB.TabIndex = 5;
            this.Receiver2VfoB.Text = "Receiver- 2 Vfo-B";
            // 
            // Receiver2VfoA
            // 
            this.Receiver2VfoA.AutoSize = true;
            this.Receiver2VfoA.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Receiver2VfoA.Location = new System.Drawing.Point(15, 15);
            this.Receiver2VfoA.Name = "Receiver2VfoA";
            this.Receiver2VfoA.Size = new System.Drawing.Size(198, 25);
            this.Receiver2VfoA.TabIndex = 4;
            this.Receiver2VfoA.Text = "Receiver- 2 Vfo-A";
            // 
            // Tx
            // 
            this.Tx.AutoSize = true;
            this.Tx.BackColor = System.Drawing.Color.Green;
            this.Tx.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tx.Location = new System.Drawing.Point(709, 31);
            this.Tx.Name = "Tx";
            this.Tx.Size = new System.Drawing.Size(73, 25);
            this.Tx.TabIndex = 10;
            this.Tx.Text = "Tx/Rx";
            // 
            // Drive
            // 
            this.Drive.AutoSize = true;
            this.Drive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Drive.Location = new System.Drawing.Point(1, 78);
            this.Drive.Name = "Drive";
            this.Drive.Size = new System.Drawing.Size(45, 16);
            this.Drive.TabIndex = 11;
            this.Drive.Text = "Drive";
            // 
            // Tune
            // 
            this.Tune.AutoSize = true;
            this.Tune.BackColor = System.Drawing.Color.Green;
            this.Tune.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tune.Location = new System.Drawing.Point(3, 117);
            this.Tune.Name = "Tune";
            this.Tune.Size = new System.Drawing.Size(43, 16);
            this.Tune.TabIndex = 13;
            this.Tune.Text = "Tune";
            // 
            // Modulation
            // 
            this.Modulation.AutoSize = true;
            this.Modulation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Modulation.Location = new System.Drawing.Point(12, 37);
            this.Modulation.Name = "Modulation";
            this.Modulation.Size = new System.Drawing.Size(84, 16);
            this.Modulation.TabIndex = 15;
            this.Modulation.Text = "Modulation";
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(798, 26);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 37);
            this.StartButton.TabIndex = 18;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(900, 26);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(75, 37);
            this.StopButton.TabIndex = 19;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // Volume
            // 
            this.Volume.AutoSize = true;
            this.Volume.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Volume.Location = new System.Drawing.Point(0, 26);
            this.Volume.Name = "Volume";
            this.Volume.Size = new System.Drawing.Size(69, 20);
            this.Volume.TabIndex = 20;
            this.Volume.Text = "Volume";
            // 
            // VolumeControl
            // 
            this.VolumeControl.Location = new System.Drawing.Point(169, 226);
            this.VolumeControl.Maximum = 0;
            this.VolumeControl.Minimum = -60;
            this.VolumeControl.Name = "VolumeControl";
            this.VolumeControl.Size = new System.Drawing.Size(184, 45);
            this.VolumeControl.TabIndex = 21;
            this.VolumeControl.Scroll += new System.EventHandler(this.VolumeControl_Scroll);
            // 
            // DriveControl
            // 
            this.DriveControl.Location = new System.Drawing.Point(169, 263);
            this.DriveControl.Maximum = 100;
            this.DriveControl.Name = "DriveControl";
            this.DriveControl.Size = new System.Drawing.Size(184, 45);
            this.DriveControl.TabIndex = 22;
            this.DriveControl.Scroll += new System.EventHandler(this.DriveControl_Scroll);
            // 
            // TuneControl
            // 
            this.TuneControl.Location = new System.Drawing.Point(169, 301);
            this.TuneControl.Maximum = 100;
            this.TuneControl.Name = "TuneControl";
            this.TuneControl.Size = new System.Drawing.Size(184, 45);
            this.TuneControl.TabIndex = 23;
            this.TuneControl.Scroll += new System.EventHandler(this.TuneControl_Scroll);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Volume);
            this.groupBox3.Controls.Add(this.Tune);
            this.groupBox3.Controls.Add(this.Drive);
            this.groupBox3.Location = new System.Drawing.Point(54, 200);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(298, 160);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            // 
            // Tr1ModulationValue
            // 
            this.Tr1ModulationValue.AutoSize = true;
            this.Tr1ModulationValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tr1ModulationValue.Location = new System.Drawing.Point(120, 18);
            this.Tr1ModulationValue.Name = "Tr1ModulationValue";
            this.Tr1ModulationValue.Size = new System.Drawing.Size(124, 16);
            this.Tr1ModulationValue.TabIndex = 16;
            this.Tr1ModulationValue.Text = "ModulationValue";
            // 
            // Tr2ModulationValue
            // 
            this.Tr2ModulationValue.AutoSize = true;
            this.Tr2ModulationValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tr2ModulationValue.Location = new System.Drawing.Point(120, 57);
            this.Tr2ModulationValue.Name = "Tr2ModulationValue";
            this.Tr2ModulationValue.Size = new System.Drawing.Size(124, 16);
            this.Tr2ModulationValue.TabIndex = 17;
            this.Tr2ModulationValue.Text = "ModulationValue";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Tr2ModulationValue);
            this.groupBox4.Controls.Add(this.Tr1ModulationValue);
            this.groupBox4.Controls.Add(this.Modulation);
            this.groupBox4.Location = new System.Drawing.Point(659, 69);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(254, 109);
            this.groupBox4.TabIndex = 25;
            this.groupBox4.TabStop = false;
            // 
            // StationMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 456);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.TuneControl);
            this.Controls.Add(this.DriveControl);
            this.Controls.Add(this.VolumeControl);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.Tx);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.tciServerPort);
            this.Controls.Add(this.tciServerIP);
            this.Name = "StationMonitor";
            this.Text = "StationMonitor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DriveControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TuneControl)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tciServerIP;
        private System.Windows.Forms.TextBox tciServerPort;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Label Receiver1VfoA;
        private System.Windows.Forms.Label Receiver1VfoB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label Receiver2VfoB;
        private System.Windows.Forms.Label Receiver2VfoA;
        private System.Windows.Forms.Label Tx;
        private System.Windows.Forms.Label Drive;
        private System.Windows.Forms.Label Tune;
        private System.Windows.Forms.Label Modulation;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Label Volume;
        private System.Windows.Forms.TrackBar VolumeControl;
        private System.Windows.Forms.TrackBar DriveControl;
        private System.Windows.Forms.TrackBar TuneControl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label Tr1ModulationValue;
        private System.Windows.Forms.Label Tr2ModulationValue;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}

