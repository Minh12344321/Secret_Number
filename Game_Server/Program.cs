using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        IPAddress ipAddress = IPAddress.Any;
        int port = 12345;
        TcpListener server = new TcpListener(ipAddress, port);
        server.Start();
        Console.WriteLine($"Server đang chạy tại IP {ipAddress} trên cổng {port}...");

        // Chọn số ngẫu nhiên từ 1 đến 100 để client đoán
        Random random = new Random();
        int targetNumber = random.Next(1, 101);
        Console.WriteLine($"Số bí mật là: {targetNumber}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client đã kết nối!");

            // Xử lý kết nối với client
            Thread clientThread = new Thread(() => HandleClient(client, targetNumber));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client, int targetNumber)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string clientGuess = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (int.TryParse(clientGuess, out int guessedNumber))
                {
                    string responseMessage;

                    // So sánh dự đoán của client với số ngẫu nhiên
                    if (guessedNumber < targetNumber)
                    {
                        responseMessage = $"Số cao hơn > {guessedNumber}";
                    }
                    else if (guessedNumber > targetNumber)
                    {
                        responseMessage = $"Số thấp hơn < {guessedNumber}";
                    }
                    else
                    {
                        responseMessage = "Chính xác! Bạn đã đoán đúng số.";
                    }

                    byte[] responseData = Encoding.UTF8.GetBytes(responseMessage);
                    stream.Write(responseData, 0, responseData.Length);
                }
                else
                {
                    string invalidMessage = "Vui lòng nhập một số hợp lệ.";
                    byte[] invalidData = Encoding.UTF8.GetBytes(invalidMessage);
                    stream.Write(invalidData, 0, invalidData.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi xử lý client: " + ex.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("Đã đóng kết nối với client.");
        }
    }
}
