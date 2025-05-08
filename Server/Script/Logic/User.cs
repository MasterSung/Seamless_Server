using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class User
{
    public TcpClient client;
    public TcpClient Client => client;

    public NetworkStream Stream => Client?.GetStream();

    string id;
    public string Id => id;

    float x;
    public float X => x;

    float y;
    public float Y => y;

    static ConcurrentDictionary<string, User> userDic = new ConcurrentDictionary<string, User>();
    public static ConcurrentDictionary<string, User> UserDic => userDic;

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
            ActionHandler.OnBroadcastSightLeaveNotify(this);
            Remove(id);
        }
    }

    public void SetPosition(float inX, float inY)
    {
        x = inX;
        y = inY;
    }

    public static bool Add(string inId, User inUser)
    {
        return userDic.TryAdd(inId, inUser);
    }

    public static bool Remove(string inId)
    {
        if (string.IsNullOrEmpty(inId))
            return false;

        return userDic.TryRemove(inId, out _);
    }

    public static List<PlayerInfo> GetPlayerInfoList(string inExceptionUserId = null)
    {
        var playerInfoList = new List<PlayerInfo>();

        foreach (var pair in userDic)
        {
            if (pair.Value == null)
                continue;

            if (pair.Key.Equals(inExceptionUserId))
                continue;

            var playerInfo = new PlayerInfo();
            playerInfo.id = pair.Key;

            playerInfoList.Add(playerInfo);
        }

        return playerInfoList;
    }
}
