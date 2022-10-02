using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace BasicAsyncServer
{
    public partial class ServerForm : Form
    {
        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();        
        private byte[] buffer;

        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        int slide = 10;

        public ServerForm()
        {
            InitializeComponent();
            StartServer();
            timer1.Interval = 50;
            timer1.Enabled = true;
        }

        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Construct server socket and bind socket to all local network interfaces, then listen for connections
        /// with a backlog of 10. Which means there can only be 10 pending connections lined up in the TCP stack
        /// at a time. This does not mean the server can handle only 10 connections. The we begin accepting connections.
        /// Meaning if there are connections queued, then we should process them.
        /// </summary>
        private void StartServer()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, 3333));
                serverSocket.Listen(10);
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            try
            {
                Socket handler = serverSocket.EndAccept(AR);
                clientSockets.Add(handler);

                buffer = new byte[handler.ReceiveBufferSize];

                // Send a message to the newly connected client.
                ShapePackage shape = new ShapePackage(20, 20, 30, 30);
                byte[] shapeBuffer = shape.ToByteArray();
                handler.BeginSend(shapeBuffer, 0, shapeBuffer.Length, SocketFlags.None, SendCallback, null);
                // Continue listening for clients.
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void SendCallback(IAsyncResult AR)      // Send package here
        {
            try
            {
                Socket current = (Socket)AR.AsyncState;
                current.EndSend(AR);

                //clientSocket.EndSend(AR);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                // Socket exception will raise here when client closes, as this sample does not
                // demonstrate graceful disconnects for the sake of simplicity.
                Socket current = (Socket)AR.AsyncState;
                int received = current.EndReceive(AR);
                //int received = clientSocket.EndReceive(AR);
            }
            // Avoid Pokemon exception handling in cases like these.
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update rect position here
            back();

            rect.X += slide;
            Invalidate();

            // Serialize Package
            ShapePackage shape = new ShapePackage(rect.X, rect.Y, rect.Width, rect.Height);
            byte[] buffer = shape.ToByteArray();

            // Send Package
            serverSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, null);
        }

        private void back()
        {
            if (rect.X >= this.Width - rect.Width * 2)
                slide = -10;
            else
            if (rect.X <= rect.Width / 2)
                slide = 10;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }

    }
}
