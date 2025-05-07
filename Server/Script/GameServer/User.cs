using System;
using System.Net.Sockets;
using System.Collections.Concurrent;

public class User
{
    public TcpClient client;
    public TcpClient Client => client;

    public NetworkStream Stream => Client?.GetStream();

    string id;
    public string Id => id;

    static ConcurrentDictionary<string, User> userDic = new ConcurrentDictionary<string, User>();

    public User(TcpClient inClient)
    {
        client = inClient;
    }

    public bool Init(string inId)
    {
        if (!Add(inId, this))
            return false;

        id = inId;
        Console.WriteLine($"LogIn : {inId}");

        return true;
    }

    public void Release()
    {
        Client?.Close();

        if (!string.IsNullOrEmpty(id))
        {
            Console.WriteLine($"LogOut : {id}");
            Remove(id);
        }
    }

    static bool Add(string inId, User inUser)
    {
        return userDic.TryAdd(inId, inUser);
    }

    static bool Remove(string inId)
    {
        if (string.IsNullOrEmpty(inId))
            return false;

        return userDic.TryRemove(inId, out _);
    }
}
