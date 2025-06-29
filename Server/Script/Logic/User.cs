using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class User
{
    static ConcurrentDictionary<string, User> userDic = new ConcurrentDictionary<string, User>();
    public static ConcurrentDictionary<string, User> UserDic => userDic;

    public TcpClient client;
    public TcpClient Client => client;

    public NetworkStream Stream => Client?.GetStream();

    string id;
    public string Id => id;

    float x;
    public float X => x;

    float y;
    public float Y => y;

    int cellIdx;
    public int CellIdx => cellIdx;

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
        int prevCellIdx = CalcCellIdx(x, y);
        int nextCellIdx = CalcCellIdx(inX, inY);

        if (prevCellIdx != nextCellIdx)
        {
            ActionHandler.OnBroadcastSightLeaveNotify(this);
            ActionHandler.OnSendSpawnLeaveNotify(this);
        }

        x = inX;
        y = inY;

        if (prevCellIdx != nextCellIdx)
        {
            cellIdx = nextCellIdx;
            ActionHandler.OnBroadcastSightEnterNotify(this);
            ActionHandler.OnSendSpawnEnterNotify(this);
        }
    }

    public static int CalcCellIdx(float inX, float inY)
    {
        (int col, int row) position = ((int)inX / Config.CellSize, (int)inY / Config.CellSize);

        return position.col + (position.row * 10);
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

    public static List<PlayerInfo> GetPlayerInfoList(User inExceptionUser)
    {
        var playerInfoList = new List<PlayerInfo>();
        if (inExceptionUser == null)
            return playerInfoList;

        int cellIdx = CalcCellIdx(inExceptionUser.X, inExceptionUser.Y);

        foreach (var pair in userDic)
        {
            var targetUser = pair.Value;
            if (targetUser == null)
                continue;

            if (pair.Key.Equals(inExceptionUser.Id))
                continue;

            int targetCellIdx = CalcCellIdx(targetUser.X, targetUser.Y);
            if (targetCellIdx != cellIdx)
                continue;

            var playerInfo = new PlayerInfo();
            playerInfo.id = pair.Key;
            playerInfo.x = targetUser.X;
            playerInfo.y = targetUser.Y;

            playerInfoList.Add(playerInfo);
        }

        return playerInfoList;
    }
}
