using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        private Socket serverSocket, clientSocket;
        private byte[] buffer;
        private int clientCount = 0;
        private ArrayList clientSocketList = ArrayList.Synchronized(new System.Collections.ArrayList());
        public delegate void UpdateClientListCallback();
        public AsyncCallback pfnClientCallBack;
        Hashtable hashTable = new Hashtable();

        public class SocketPacket
        {
            public SocketPacket(Socket socket, int clientNumber)
            {
                m_currentSocket = socket;
                m_clientNumber = clientNumber;
            }
            public Socket m_currentSocket;
            public int m_clientNumber;
            public byte[] dataBuffer = new byte[1024];
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtServerIP.Text = GetMyIP();
            txtServerPort.Text = "8888";
            txtChatRoom.ReadOnly = true;
            StartServer();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            StartServer();
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            CloseSockets();
            UpdateButtonEnabledControls(false);
        }

        private void btnBoardcast_Click(object sender, EventArgs e)
        {
            string msg = "[" + DateTime.Now.ToString("HH:mm") + "] 伺服器公告 : " + txtSendText.Text;
            AppendToChatRoom(msg);
            sendAllUsers(msg);
        }

        private void btnClearChatRoom_Click(object sender, EventArgs e)
        {
            txtChatRoom.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void UpdateButtonEnabledControls(bool p)
        {
            txtChatRoom.Clear();
            txtSendText.Clear();
            lstOnlineUsers.Items.Clear();
            btnStartServer.Enabled = !p;
            btnStopServer.Enabled = p;
            btnBoardcast.Enabled = p;
            btnClearChatRoom.Enabled = p;
            txtServerIP.ReadOnly = p;
            txtServerPort.ReadOnly = p;
            txtSendText.ReadOnly = !p;
            txtChatRoom.BackColor = Color.DarkGray;
            txtSendText.BackColor = Color.DarkGray;
            lstOnlineUsers.BackColor = Color.DarkGray;
            if (p)
            {
                txtChatRoom.BackColor = Color.LightBlue;
                txtSendText.BackColor = Color.Wheat;
                lstOnlineUsers.BackColor = Color.LightGreen;
            }
        }

        private string GetMyIP()
        {
            string hostname = Dns.GetHostName();

            IPAddress[] ip = Dns.GetHostEntry(hostname).AddressList;
            foreach (IPAddress it in ip)
            {
                if (it.AddressFamily == AddressFamily.InterNetwork)
                {
                    return it.ToString();
                }
            }
            return "";
        }

        private void StartServer()
        {
            int port = System.Convert.ToInt32(txtServerPort.Text);
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(10);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateButtonEnabledControls(true);
            txtSendText.Select();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket = serverSocket.EndAccept(ar);
                Interlocked.Increment(ref clientCount);
                clientSocketList.Add(clientSocket);
                UpdateClientListControl();
                WaitForDataFromClient(clientSocket, clientCount);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            catch (ObjectDisposedException)
            {
                Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
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

        public void WaitForDataFromClient(Socket soc, int clientNumber)
        {
            try
            {
                if (pfnClientCallBack == null)
                {
                    pfnClientCallBack = new AsyncCallback(ReceiveCallback);
                }
                SocketPacket socketPacket = new SocketPacket(soc, clientNumber);
                soc.BeginReceive(socketPacket.dataBuffer, 0, socketPacket.dataBuffer.Length, SocketFlags.None, pfnClientCallBack, socketPacket);
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

        private void UpdateClientListControl()
        {
            if (InvokeRequired)
            {
                lstOnlineUsers.BeginInvoke(new UpdateClientListCallback(UpdateClientList), null);
            }
            else
            {
                UpdateClientList();
            }
        }

        private void UpdateClientList()
        {
            lstOnlineUsers.Items.Clear();
            string namelist = "##";
            for (int i = 0; i < clientSocketList.Count; i++)
            {
                string clientKey = Convert.ToString(i + 1);
                Socket userSocket = (Socket)clientSocketList[i];
                if (userSocket != null)
                {
                    if (userSocket.Connected)
                    {
                        foreach (DictionaryEntry ht in hashTable)
                        {
                            if (Convert.ToInt16(ht.Key) == Convert.ToInt16(clientKey))
                            {
                                lstOnlineUsers.Items.Add(clientKey + " " + "( " + Convert.ToString(ht.Value) + ")");
                                namelist += Convert.ToString(ht.Value) + ";";
                            }
                        }
                    }
                }
            }
            sendAllUsers(namelist);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            SocketPacket socketData = (SocketPacket)ar.AsyncState;
            int inLength = 0;
            string message;
            string cmd;
            string strMessage;
            string userName;
            try
            {
                inLength = socketData.m_currentSocket.EndReceive(ar);
                message = Encoding.UTF8.GetString(socketData.dataBuffer, 0, inLength);
                string msgTitle = "[" + DateTime.Now.ToString("HH:mm") + "] ";
                cmd = message.Substring(0, 1);
                if (cmd == "0")
                {
                    strMessage = msgTitle + message.Substring(1) + " 加入了聊天室。";
                    userName = message.Substring(1);
                    AppendToChatRoom(strMessage);
                    sendAllUsers(strMessage);
                    hashTable.Add(socketData.m_clientNumber, userName);
                    UpdateClientListControl();
                }
                else
                {
                    strMessage = msgTitle + message;
                    AppendToChatRoom(strMessage);
                    sendAllUsers(strMessage);
                    Debug.WriteLine("endter ReceiveCallback -> sendAllUsers  ...");
                }
                WaitForDataFromClient(socketData.m_currentSocket, socketData.m_clientNumber);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054)
                {
                    string msg = "\r\n" + "Client " + socketData.m_clientNumber + " Disconnected" + "\r\n";
                    //AppendToChatRoom(msg);
                    clientSocketList[socketData.m_clientNumber - 1] = null;
                    UpdateClientListControl();
                }
                else
                {
                    MessageBox.Show(se.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendToChatRoom(string text)
        {
            MethodInvoker invoker = new MethodInvoker(delegate {
                txtChatRoom.Text += text + "\r\n";
                txtChatRoom.SelectionStart = txtChatRoom.Text.Length;
                txtChatRoom.ScrollToCaret();
            });
            this.Invoke(invoker);
        }

        private void sendToUser(string strMsg, string user)
        {

        }

        void CloseSockets()
        {
            if (serverSocket != null)
            {
                serverSocket.Close();
            }
            Socket workerSocket = null;
            for (int i = 0; i < clientSocketList.Count; i++)
            {
                workerSocket = (Socket)clientSocketList[i];
                if (workerSocket != null)
                {
                    workerSocket.Close();
                    workerSocket = null;
                }
            }
        }

        private void sendAllUsers(string msg)
        {
            try
            {
                Debug.WriteLine("endter sendAllUsers  ...");
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes("\r\n" + msg);
                Socket clientSocket = null;
                for (int i = 0; i < clientSocketList.Count; i++)
                {
                    clientSocket = (Socket)clientSocketList[i];
                    if (clientSocket != null)
                    {
                        if (clientSocket.Connected)
                        {
                            clientSocket.Send(byteArray);
                        }
                    }
                }

            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }
    }
}
