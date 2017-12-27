namespace ChatServer
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.lstOnlineUsers = new System.Windows.Forms.ListBox();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSendText = new System.Windows.Forms.TextBox();
            this.btnBoardcast = new System.Windows.Forms.Button();
            this.btnClearChatRoom = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtChatRoom = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "伺服器 IP";
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(440, 25);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(75, 23);
            this.btnStartServer.TabIndex = 1;
            this.btnStartServer.Text = "啟動伺服器";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(90, 27);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.Size = new System.Drawing.Size(100, 22);
            this.txtServerIP.TabIndex = 2;
            // 
            // lstOnlineUsers
            // 
            this.lstOnlineUsers.BackColor = System.Drawing.Color.LightGreen;
            this.lstOnlineUsers.FormattingEnabled = true;
            this.lstOnlineUsers.ItemHeight = 12;
            this.lstOnlineUsers.Location = new System.Drawing.Point(364, 79);
            this.lstOnlineUsers.Name = "lstOnlineUsers";
            this.lstOnlineUsers.Size = new System.Drawing.Size(151, 124);
            this.lstOnlineUsers.TabIndex = 3;
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(265, 27);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(91, 22);
            this.txtServerPort.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "伺服器 Port";
            // 
            // btnStopServer
            // 
            this.btnStopServer.Location = new System.Drawing.Point(440, 54);
            this.btnStopServer.Name = "btnStopServer";
            this.btnStopServer.Size = new System.Drawing.Size(75, 23);
            this.btnStopServer.TabIndex = 6;
            this.btnStopServer.Text = "關閉伺服器";
            this.btnStopServer.UseVisualStyleBackColor = true;
            this.btnStopServer.Click += new System.EventHandler(this.btnStopServer_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(362, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "線上使用者";
            // 
            // txtSendText
            // 
            this.txtSendText.BackColor = System.Drawing.Color.Wheat;
            this.txtSendText.Location = new System.Drawing.Point(33, 236);
            this.txtSendText.Name = "txtSendText";
            this.txtSendText.Size = new System.Drawing.Size(323, 22);
            this.txtSendText.TabIndex = 8;
            // 
            // btnBoardcast
            // 
            this.btnBoardcast.Location = new System.Drawing.Point(364, 236);
            this.btnBoardcast.Name = "btnBoardcast";
            this.btnBoardcast.Size = new System.Drawing.Size(151, 23);
            this.btnBoardcast.TabIndex = 9;
            this.btnBoardcast.Text = "廣播至所有使用者";
            this.btnBoardcast.UseVisualStyleBackColor = true;
            this.btnBoardcast.Click += new System.EventHandler(this.btnBoardcast_Click);
            // 
            // btnClearChatRoom
            // 
            this.btnClearChatRoom.Location = new System.Drawing.Point(364, 207);
            this.btnClearChatRoom.Name = "btnClearChatRoom";
            this.btnClearChatRoom.Size = new System.Drawing.Size(151, 23);
            this.btnClearChatRoom.TabIndex = 10;
            this.btnClearChatRoom.Text = "清除聊天紀錄";
            this.btnClearChatRoom.UseVisualStyleBackColor = true;
            this.btnClearChatRoom.Click += new System.EventHandler(this.btnClearChatRoom_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "聊天紀錄";
            // 
            // txtChatRoom
            // 
            this.txtChatRoom.BackColor = System.Drawing.Color.LightBlue;
            this.txtChatRoom.Location = new System.Drawing.Point(31, 79);
            this.txtChatRoom.Multiline = true;
            this.txtChatRoom.Name = "txtChatRoom";
            this.txtChatRoom.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtChatRoom.Size = new System.Drawing.Size(325, 151);
            this.txtChatRoom.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 273);
            this.Controls.Add(this.txtChatRoom);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnClearChatRoom);
            this.Controls.Add(this.btnBoardcast);
            this.Controls.Add(this.txtSendText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnStopServer);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstOnlineUsers);
            this.Controls.Add(this.txtServerIP);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TCP Socket 伺服器";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.ListBox lstOnlineUsers;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSendText;
        private System.Windows.Forms.Button btnBoardcast;
        private System.Windows.Forms.Button btnClearChatRoom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtChatRoom;
    }
}

