using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void txtGuess_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            bool isConnected = false;
            TcpClient client = null;

            // Thử kết nối với server cho đến khi thành công
            while (!isConnected)
            {
                try
                {
                    string serverIP = "127.0.0.1"; // Địa chỉ IP của server
                    int port = 12345; // Cổng của server

                    // Tạo TcpClient và kết nối tới server
                    client = new TcpClient(serverIP, port);
                    isConnected = true;
                }
                catch (SocketException)
                {
                    MessageBox.Show("Chưa thể kết nối đến server. Đang thử lại...");
                    Thread.Sleep(2000); // Đợi 2 giây rồi thử lại
                }
            }

            try
            {
                // Gửi tin nhắn tới server sau khi kết nối thành công
                string guess = txtGuess.Text;
                byte[] data = Encoding.UTF8.GetBytes(guess);

                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                // Nhận phản hồi từ server
                byte[] responseData = new byte[1024];

                try
                {
                    int bytesRead = stream.Read(responseData, 0, responseData.Length);
                    string responseMessage = Encoding.UTF8.GetString(responseData, 0, bytesRead);
                    lblResponse.Text = responseMessage;
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Lỗi đọc dữ liệu từ server: " + ex.Message);
                }

                client.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }
    }
}
   
