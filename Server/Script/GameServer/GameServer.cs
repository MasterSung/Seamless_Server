using System;
using System.Net.Sockets;
using System.Threading;

public class GameServer : IDisposable
{
    bool isRunning;
    public bool IsRunning => isRunning;

    TcpListener listener;
    Thread serverThread;

    public GameServer()
    {
        isRunning = true;

        serverThread = new Thread(StartServer);
        serverThread.IsBackground = true;
        serverThread.Start();

        Console.WriteLine("[GameServer] Start");
    }

    public void Dispose()
    {
        isRunning = false;

        listener?.Stop();
        if (serverThread != null && serverThread.IsAlive)
            serverThread.Join();

        Console.WriteLine("[GameServer] Release");
    }

    void StartServer()
    {
        try
        {
            listener = new TcpListener(Config.IP, Config.Port);
            listener.Start();

            while (isRunning)
            {
                try
                {
                    if (!listener.Pending())
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    TcpClient client = listener.AcceptTcpClient();
                    var user = new User(client);

                    Thread clientThread = new Thread(() => HandleClient(user));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (SocketException)
                {
                    if (!isRunning)
                        break;

                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("서버 예외 : " + ex.Message);
        }
    }

    void HandleClient(User user)
    {
        try
        {
            byte[] buffer = new byte[1024];
            int length;

            while ((length = user != null ? user.Stream.Read(buffer, 0, buffer.Length) : 0) != 0)
            {
                byte[] data = new byte[length];
                Array.Copy(buffer, data, length);

                PacketSelector.OnSendServer(user, data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("클라이언트 처리 중 예외 : " + ex.Message);
        }
        finally
        {
            user?.Release();
        }
    }
}
