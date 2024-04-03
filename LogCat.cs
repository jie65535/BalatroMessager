using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BalatroMessager
{
    public class LogCat
    {
        private readonly TcpListener _listener;

        public LogCat(int port)
        {
            _listener = new TcpListener(new IPEndPoint(IPAddress.Loopback, port));
        }

        public async Task Listen(CancellationToken token)
        {
            try
            {
                _listener.Start();
                var buffer = new byte[1024];
                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine($"[LogCat] {_listener.LocalEndpoint} Listening...");
                    while (!_listener.Pending())
                        await Task.Delay(100, token);

                    using (var client = _listener.AcceptTcpClient())
                    using (var stream = client.GetStream())
                    {
                        Console.WriteLine("[LogCat] Connected. Receiving...");
                        try
                        {
                            while (!token.IsCancellationRequested)
                            {
                                var len = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                                if (len == 0)
                                {
                                    Console.WriteLine("[LogCat] Disconnected.");
                                    break;
                                }
                                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, len));
                            }
                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine("[LogCat] Socket Error occurred while listening. Code: {0}\n{1}", ex.ErrorCode, ex);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("[LogCat] {0} Error occurred while listening: {1}", ex.GetType().Name, ex.Message);
                        }

                        try
                        {
                            client.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("[LogCat] close throw exception: {0}", ex);
                        }
                    }
                }
            }
            finally
            {
                _listener.Stop();
                Console.WriteLine("[LogCat] Stopped.");
            }
        }
    }
}
