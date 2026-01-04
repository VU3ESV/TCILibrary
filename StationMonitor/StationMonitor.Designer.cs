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
        protected override async void Dispose(bool disposing)
        {
            if (tciClient?.ConnectionStatus == ExpertElectronics.Tci.Interfaces.ConnectionStatus.Connected)
            {
                await tciClient?.DisConnectAsync();
            }

            tciClient?.Dispose();

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
            tciServerIP = new TextBox();
            tciServerPort = new TextBox();
            ConnectButton = new Button();
            DisconnectButton = new Button();
            Receiver1VfoA = new Label();
            Receiver1VfoB = new Label();
            label1 = new Label();
            label2 = new Label();
            groupBox1 = new GroupBox();
            R1Split = new Button();
            R1BtoA = new Button();
            R1AtoB = new Button();
            Tr1ModulationValue = new Label();
            groupBox2 = new GroupBox();
            R2Split = new Button();
            R2BtoA = new Button();
            Tr2ModulationValue = new Label();
            R2AtoB = new Button();
            Receiver2VfoB = new Label();
            Receiver2VfoA = new Label();
            Tx = new Label();
            Drive = new Label();
            Tune = new Label();
            StartButton = new Button();
            StopButton = new Button();
            Volume = new Label();
            VolumeControl = new TrackBar();
            DriveControl = new TrackBar();
            TuneControl = new TrackBar();
            groupBox3 = new GroupBox();
            ConnectionStatus = new Label();
            TuneButton = new Button();
            MuteButton = new Button();
            Device = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)VolumeControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DriveControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TuneControl).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // tciServerIP
            // 
            tciServerIP.Location = new Point(82, 31);
            tciServerIP.Margin = new Padding(4, 3, 4, 3);
            tciServerIP.Name = "tciServerIP";
            tciServerIP.Size = new Size(266, 23);
            tciServerIP.TabIndex = 0;
            tciServerIP.Text = "192.168.86.26";
            // 
            // tciServerPort
            // 
            tciServerPort.Location = new Point(377, 33);
            tciServerPort.Margin = new Padding(4, 3, 4, 3);
            tciServerPort.Name = "tciServerPort";
            tciServerPort.Size = new Size(151, 23);
            tciServerPort.TabIndex = 1;
            tciServerPort.Text = "50001";
            // 
            // ConnectButton
            // 
            ConnectButton.Location = new Point(536, 33);
            ConnectButton.Margin = new Padding(4, 3, 4, 3);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new Size(91, 29);
            ConnectButton.TabIndex = 2;
            ConnectButton.Text = "Connect";
            ConnectButton.UseVisualStyleBackColor = true;
            ConnectButton.Click += ConnectButton_Click;
            // 
            // DisconnectButton
            // 
            DisconnectButton.Location = new Point(634, 33);
            DisconnectButton.Margin = new Padding(4, 3, 4, 3);
            DisconnectButton.Name = "DisconnectButton";
            DisconnectButton.Size = new Size(83, 27);
            DisconnectButton.TabIndex = 3;
            DisconnectButton.Text = "Disconnect";
            DisconnectButton.UseVisualStyleBackColor = true;
            DisconnectButton.Click += DisconnectButton_Click;
            // 
            // Receiver1VfoA
            // 
            Receiver1VfoA.AutoSize = true;
            Receiver1VfoA.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            Receiver1VfoA.Location = new Point(20, 23);
            Receiver1VfoA.Margin = new Padding(4, 0, 4, 0);
            Receiver1VfoA.Name = "Receiver1VfoA";
            Receiver1VfoA.Size = new Size(128, 25);
            Receiver1VfoA.TabIndex = 4;
            Receiver1VfoA.Text = "R - 1 Vfo-A";
            // 
            // Receiver1VfoB
            // 
            Receiver1VfoB.AutoSize = true;
            Receiver1VfoB.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            Receiver1VfoB.Location = new Point(20, 85);
            Receiver1VfoB.Margin = new Padding(4, 0, 4, 0);
            Receiver1VfoB.Name = "Receiver1VfoB";
            Receiver1VfoB.Size = new Size(128, 25);
            Receiver1VfoB.TabIndex = 5;
            Receiver1VfoB.Text = "R - 1 Vfo-B";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(602, 90);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 7;
            label1.Text = "Receiver- 2 Vfo-B";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(426, 90);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(99, 15);
            label2.TabIndex = 6;
            label2.Text = "Receiver- 2 Vfo-A";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(R1Split);
            groupBox1.Controls.Add(R1BtoA);
            groupBox1.Controls.Add(R1AtoB);
            groupBox1.Controls.Add(Receiver1VfoB);
            groupBox1.Controls.Add(Tr1ModulationValue);
            groupBox1.Controls.Add(Receiver1VfoA);
            groupBox1.Location = new Point(68, 73);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(337, 595);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Transceiver -1";
            // 
            // R1Split
            // 
            R1Split.Location = new Point(187, 186);
            R1Split.Margin = new Padding(4, 3, 4, 3);
            R1Split.Name = "R1Split";
            R1Split.Size = new Size(71, 36);
            R1Split.TabIndex = 19;
            R1Split.Text = "Split";
            R1Split.UseVisualStyleBackColor = true;
            R1Split.Click += R1Split_Click;
            // 
            // R1BtoA
            // 
            R1BtoA.Location = new Point(106, 186);
            R1BtoA.Margin = new Padding(4, 3, 4, 3);
            R1BtoA.Name = "R1BtoA";
            R1BtoA.Size = new Size(71, 36);
            R1BtoA.TabIndex = 18;
            R1BtoA.Text = "B > A";
            R1BtoA.UseVisualStyleBackColor = true;
            R1BtoA.Click += R1BtoA_Click;
            // 
            // R1AtoB
            // 
            R1AtoB.Location = new Point(26, 186);
            R1AtoB.Margin = new Padding(4, 3, 4, 3);
            R1AtoB.Name = "R1AtoB";
            R1AtoB.Size = new Size(71, 36);
            R1AtoB.TabIndex = 17;
            R1AtoB.Text = "A > B";
            R1AtoB.UseVisualStyleBackColor = true;
            R1AtoB.Click += R1AtoB_Click;
            // 
            // Tr1ModulationValue
            // 
            Tr1ModulationValue.AutoSize = true;
            Tr1ModulationValue.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Tr1ModulationValue.Location = new Point(20, 135);
            Tr1ModulationValue.Margin = new Padding(4, 0, 4, 0);
            Tr1ModulationValue.Name = "Tr1ModulationValue";
            Tr1ModulationValue.Size = new Size(123, 16);
            Tr1ModulationValue.TabIndex = 16;
            Tr1ModulationValue.Text = "ModulationValue";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(R2Split);
            groupBox2.Controls.Add(R2BtoA);
            groupBox2.Controls.Add(Tr2ModulationValue);
            groupBox2.Controls.Add(R2AtoB);
            groupBox2.Controls.Add(Receiver2VfoB);
            groupBox2.Controls.Add(Receiver2VfoA);
            groupBox2.Location = new Point(429, 73);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(331, 595);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Transceiver -2";
            // 
            // R2Split
            // 
            R2Split.Location = new Point(160, 186);
            R2Split.Margin = new Padding(4, 3, 4, 3);
            R2Split.Name = "R2Split";
            R2Split.Size = new Size(68, 36);
            R2Split.TabIndex = 21;
            R2Split.Text = "Split";
            R2Split.UseVisualStyleBackColor = true;
            R2Split.Click += R2Split_Click;
            // 
            // R2BtoA
            // 
            R2BtoA.Location = new Point(85, 186);
            R2BtoA.Margin = new Padding(4, 3, 4, 3);
            R2BtoA.Name = "R2BtoA";
            R2BtoA.Size = new Size(68, 36);
            R2BtoA.TabIndex = 20;
            R2BtoA.Text = "B > A";
            R2BtoA.UseVisualStyleBackColor = true;
            R2BtoA.Click += R2BtoA_Click;
            // 
            // Tr2ModulationValue
            // 
            Tr2ModulationValue.AutoSize = true;
            Tr2ModulationValue.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Tr2ModulationValue.Location = new Point(7, 125);
            Tr2ModulationValue.Margin = new Padding(4, 0, 4, 0);
            Tr2ModulationValue.Name = "Tr2ModulationValue";
            Tr2ModulationValue.Size = new Size(123, 16);
            Tr2ModulationValue.TabIndex = 17;
            Tr2ModulationValue.Text = "ModulationValue";
            // 
            // R2AtoB
            // 
            R2AtoB.Location = new Point(10, 186);
            R2AtoB.Margin = new Padding(4, 3, 4, 3);
            R2AtoB.Name = "R2AtoB";
            R2AtoB.Size = new Size(68, 36);
            R2AtoB.TabIndex = 19;
            R2AtoB.Text = "A > B";
            R2AtoB.UseVisualStyleBackColor = true;
            R2AtoB.Click += R2AtoB_Click;
            // 
            // Receiver2VfoB
            // 
            Receiver2VfoB.AutoSize = true;
            Receiver2VfoB.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            Receiver2VfoB.Location = new Point(18, 85);
            Receiver2VfoB.Margin = new Padding(4, 0, 4, 0);
            Receiver2VfoB.Name = "Receiver2VfoB";
            Receiver2VfoB.Size = new Size(128, 25);
            Receiver2VfoB.TabIndex = 5;
            Receiver2VfoB.Text = "R - 2 Vfo-B";
            // 
            // Receiver2VfoA
            // 
            Receiver2VfoA.AutoSize = true;
            Receiver2VfoA.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            Receiver2VfoA.Location = new Point(18, 17);
            Receiver2VfoA.Margin = new Padding(4, 0, 4, 0);
            Receiver2VfoA.Name = "Receiver2VfoA";
            Receiver2VfoA.Size = new Size(128, 25);
            Receiver2VfoA.TabIndex = 4;
            Receiver2VfoA.Text = "R - 2 Vfo-A";
            // 
            // Tx
            // 
            Tx.AutoSize = true;
            Tx.BackColor = Color.Green;
            Tx.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            Tx.Location = new Point(780, 33);
            Tx.Margin = new Padding(4, 0, 4, 0);
            Tx.Name = "Tx";
            Tx.Size = new Size(73, 25);
            Tx.TabIndex = 10;
            Tx.Text = "Tx/Rx";
            // 
            // Drive
            // 
            Drive.AutoSize = true;
            Drive.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Drive.Location = new Point(1, 90);
            Drive.Margin = new Padding(4, 0, 4, 0);
            Drive.Name = "Drive";
            Drive.Size = new Size(44, 16);
            Drive.TabIndex = 11;
            Drive.Text = "Drive";
            // 
            // Tune
            // 
            Tune.AutoSize = true;
            Tune.BackColor = Color.Green;
            Tune.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Tune.Location = new Point(4, 135);
            Tune.Margin = new Padding(4, 0, 4, 0);
            Tune.Name = "Tune";
            Tune.Size = new Size(42, 16);
            Tune.TabIndex = 13;
            Tune.Text = "Tune";
            // 
            // StartButton
            // 
            StartButton.Location = new Point(891, 30);
            StartButton.Margin = new Padding(4, 3, 4, 3);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(88, 43);
            StartButton.TabIndex = 18;
            StartButton.Text = "Start";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // StopButton
            // 
            StopButton.Location = new Point(986, 31);
            StopButton.Margin = new Padding(4, 3, 4, 3);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(88, 43);
            StopButton.TabIndex = 19;
            StopButton.Text = "Stop";
            StopButton.UseVisualStyleBackColor = true;
            StopButton.Click += StopButton_Click;
            // 
            // Volume
            // 
            Volume.AutoSize = true;
            Volume.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Volume.Location = new Point(0, 30);
            Volume.Margin = new Padding(4, 0, 4, 0);
            Volume.Name = "Volume";
            Volume.Size = new Size(69, 20);
            Volume.TabIndex = 20;
            Volume.Text = "Volume";
            // 
            // VolumeControl
            // 
            VolumeControl.Location = new Point(986, 121);
            VolumeControl.Margin = new Padding(4, 3, 4, 3);
            VolumeControl.Maximum = 0;
            VolumeControl.Minimum = -60;
            VolumeControl.Name = "VolumeControl";
            VolumeControl.Size = new Size(215, 45);
            VolumeControl.TabIndex = 21;
            VolumeControl.Scroll += VolumeControl_Scroll;
            // 
            // DriveControl
            // 
            DriveControl.Location = new Point(986, 164);
            DriveControl.Margin = new Padding(4, 3, 4, 3);
            DriveControl.Maximum = 100;
            DriveControl.Name = "DriveControl";
            DriveControl.Size = new Size(215, 45);
            DriveControl.TabIndex = 22;
            DriveControl.Scroll += DriveControl_Scroll;
            // 
            // TuneControl
            // 
            TuneControl.Location = new Point(986, 208);
            TuneControl.Margin = new Padding(4, 3, 4, 3);
            TuneControl.Maximum = 100;
            TuneControl.Name = "TuneControl";
            TuneControl.Size = new Size(215, 45);
            TuneControl.TabIndex = 23;
            TuneControl.Scroll += TuneControl_Scroll;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(Volume);
            groupBox3.Controls.Add(Tune);
            groupBox3.Controls.Add(Drive);
            groupBox3.Location = new Point(852, 91);
            groupBox3.Margin = new Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 3, 4, 3);
            groupBox3.Size = new Size(348, 185);
            groupBox3.TabIndex = 24;
            groupBox3.TabStop = false;
            // 
            // ConnectionStatus
            // 
            ConnectionStatus.AutoSize = true;
            ConnectionStatus.Location = new Point(729, 39);
            ConnectionStatus.Margin = new Padding(4, 0, 4, 0);
            ConnectionStatus.Name = "ConnectionStatus";
            ConnectionStatus.Size = new Size(28, 15);
            ConnectionStatus.TabIndex = 26;
            ConnectionStatus.Text = "C/D";
            // 
            // TuneButton
            // 
            TuneButton.Location = new Point(1224, 219);
            TuneButton.Margin = new Padding(4, 3, 4, 3);
            TuneButton.Name = "TuneButton";
            TuneButton.Size = new Size(77, 39);
            TuneButton.TabIndex = 27;
            TuneButton.Text = "Tune";
            TuneButton.UseVisualStyleBackColor = true;
            TuneButton.Click += TuneButton_Click;
            // 
            // MuteButton
            // 
            MuteButton.Location = new Point(1224, 105);
            MuteButton.Margin = new Padding(4, 3, 4, 3);
            MuteButton.Name = "MuteButton";
            MuteButton.Size = new Size(77, 39);
            MuteButton.TabIndex = 28;
            MuteButton.Text = "Mute";
            MuteButton.UseVisualStyleBackColor = true;
            MuteButton.Click += MuteButton_Click;
            // 
            // Device
            // 
            Device.AutoSize = true;
            Device.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Device.Location = new Point(1111, 47);
            Device.Margin = new Padding(4, 0, 4, 0);
            Device.Name = "Device";
            Device.Size = new Size(0, 16);
            Device.TabIndex = 29;
            // 
            // StationMonitor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(1314, 789);
            Controls.Add(Device);
            Controls.Add(MuteButton);
            Controls.Add(TuneButton);
            Controls.Add(ConnectionStatus);
            Controls.Add(TuneControl);
            Controls.Add(DriveControl);
            Controls.Add(VolumeControl);
            Controls.Add(StopButton);
            Controls.Add(StartButton);
            Controls.Add(Tx);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(DisconnectButton);
            Controls.Add(ConnectButton);
            Controls.Add(tciServerPort);
            Controls.Add(tciServerIP);
            Controls.Add(groupBox3);
            Margin = new Padding(4, 3, 4, 3);
            Name = "StationMonitor";
            Text = "StationMonitor";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)VolumeControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)DriveControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)TuneControl).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

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
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Label Volume;
        private System.Windows.Forms.TrackBar VolumeControl;
        private System.Windows.Forms.TrackBar DriveControl;
        private System.Windows.Forms.TrackBar TuneControl;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label Tr1ModulationValue;
        private System.Windows.Forms.Label Tr2ModulationValue;
        private System.Windows.Forms.Label ConnectionStatus;
        private System.Windows.Forms.Button R1BtoA;
        private System.Windows.Forms.Button R1AtoB;
        private System.Windows.Forms.Button R2BtoA;
        private System.Windows.Forms.Button R2AtoB;
        private System.Windows.Forms.Button TuneButton;
        private System.Windows.Forms.Button MuteButton;
        private System.Windows.Forms.Button R1Split;
        private System.Windows.Forms.Button R2Split;
        private System.Windows.Forms.Label Device;
    }
}

