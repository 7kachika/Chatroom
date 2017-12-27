using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        Thread threadTickData;
        public Socket clientSocket;
        public AsyncCallback pfnCallBack;
        IAsyncResult result;

        public class SocketPacket
        {
            public Socket thisSocket;
            public byte[] dataBuffer = new byte[1024];
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string myIpAddress = GetMyIP();
            string myMacAddress = GetMyMAC();
            txtServerIP.Text = "127.0.0.1";
            txtServerIP.Text = myIpAddress;
            txtServerPort.Text = "8888";
            txtMyIp.Text = myIpAddress;
            txtMyMac.Text = myMacAddress;
            txtClientName.Text = "anonymous";
            txtMyIp.ReadOnly = true;
            txtMyMac.ReadOnly = true;
            txtChatRoom.ReadOnly = true;
            UpdateButtonEnabledControls(false);
            txtClientName.Select();
        }

        private void btnConnectServer_Click(object sender, EventArgs e)
        {
            ConnectServer();
        }

        private void btnDisconnectServer_Click(object sender, EventArgs e)
        {
            if (threadTickData == null)
            {

            }
            else
            {
                try
                {
                    if (threadTickData.IsAlive == true)
                    {
                        threadTickData.Abort();
                        threadTickData.Join();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            CloseSocket();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string message = txtClientName.Text + "說 : " + txtClientTalk.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException)
            {
                MessageBox.Show("伺服器已關閉。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearSelect_Click(object sender, EventArgs e)
        {
            lstOnlineUsers.ClearSelected();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadTickData == null)
            {
            }
            else
            {
                try
                {
                    if (threadTickData.IsAlive == true)
                    {
                        threadTickData.Abort();
                        threadTickData.Join();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void UpdateButtonEnabledControls(bool p)
        {
            txtChatRoom.Clear();
            txtClientTalk.Clear();
            lstOnlineUsers.Items.Clear();
            btnConnectServer.Enabled = !p;
            btnDisconnectServer.Enabled = p;
            btnSend.Enabled = p;
            txtClientName.ReadOnly = p;
            txtClientTalk.ReadOnly = !p;
            txtChatRoom.BackColor = Color.DarkGray;
            txtClientTalk.BackColor = Color.DarkGray;
            lstOnlineUsers.BackColor = Color.DarkGray;
            txtClientName.Select();
            if (p)
            {
                txtChatRoom.Text = "歡迎使用聊天室~";
                txtClientTalk.Text = "請輸入聊天訊息...";
                txtChatRoom.BackColor = Color.LightBlue;
                txtClientTalk.BackColor = Color.Wheat;
                lstOnlineUsers.BackColor = Color.LightGreen;
                txtClientTalk.Select();
            }
        }

        private string GetMyIP()
        {
            string hostname = Dns.GetHostName();
            IPAddress[] ip = Dns.GetHostEntry(hostname).AddressList;
            foreach (IPAddress it in ip)
            {
                if (it.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return it.ToString();
                }
            }
            return "";
        }

        private string GetMyMAC()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in nics)
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    return nic.GetPhysicalAddress().ToString();
                }
            }
            return "";
        }

        private void ConnectServer()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(txtServerIP.Text);
                int port = System.Convert.ToInt16(txtServerPort.Text);
                clientSocket.BeginConnect(new IPEndPoint(ip, port), new AsyncCallback(ConnectCallback), null);

            }
            catch (SocketException se)
            {
                MessageBox.Show("\n連線失敗。\n" + se.Message, "Application.ProductName", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateButtonEnabledControls(true);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            CheckForIllegalCrossThreadCalls = false;
            try
            {
                clientSocket.EndConnect(ar);
                btnSend.Enabled = true;
                btnConnectServer.Enabled = false;
                userInfoSend();
                WaitForDataFromServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WaitForDataFromServer()
        {
            try
            {
                if (pfnCallBack == null)
                {
                    pfnCallBack = new AsyncCallback(ReceiveCallback);
                }
                SocketPacket theSocPkt = new SocketPacket();
                theSocPkt.thisSocket = clientSocket;
                result = clientSocket.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnCallBack, theSocPkt);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            CheckForIllegalCrossThreadCalls = false;
            int inLength = 0;
            string message;
            string cmd;
            string strUserName;
            try
            {
                SocketPacket theSockId = (SocketPacket)ar.AsyncState;
                inLength = theSockId.thisSocket.EndReceive(ar);
                message = Encoding.UTF8.GetString(theSockId.dataBuffer, 0, inLength);
                cmd = message.Substring(2, 2);
                if (cmd == "##")
                {
                    lstOnlineUsers.Items.Clear();
                    strUserName = message.Substring(4);
                    string[] nameList = strUserName.Split(';');
                    for (int i = 0; i < nameList.Length; i++)
                    {
                        lstOnlineUsers.Items.Add(nameList[i]);
                    }
                    WaitForDataFromServer();
                }
                else
                {
                    Debug.WriteLine(message);
                    AppendToChatRoom(message);
                    WaitForDataFromServer();
                }
            }
            catch (ObjectDisposedException)
            {
                Debugger.Log(0, "1", "\nReceiveCallback: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendToChatRoom(string text)
        {
            MethodInvoker invoker = new MethodInvoker(delegate {
                txtChatRoom.Text += text;
                txtChatRoom.SelectionStart = txtChatRoom.Text.Length;
                txtChatRoom.ScrollToCaret();
            });
            this.Invoke(invoker);
        }

        public void userInfoSend()
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes("0" + txtClientName.Text);
                clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            }
            catch (SocketException)
            {
                MessageBox.Show("伺服器已關閉。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sendTickData(object obj)
        {
            int i = 0;
            string msg = "測試 ...  ";
            while (true)
            {
                i++;
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(msg + Convert.ToString(i) + "\r\n");
                    clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
                }
                catch (SocketException)
                {
                    MessageBox.Show("伺服器已關閉。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Thread.Sleep(500);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sendToServer(string strMsg)
        {
        }

        private void sendTick()
        {
            int i = 0;
            while (true)
            {
                sendToServer("1" + " test " + Convert.ToString(i));
                Thread.Sleep(500);
                i++;
            }
        }

        private void CloseSocket()
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
                clientSocket = null;
                UpdateButtonEnabledControls(false);
            }
        }
    }
}
