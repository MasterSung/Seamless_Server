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

    (int c, int r) cellIdx;
    public (int c, int r) CellIdx => cellIdx;

    public User(TcpClient inClient)
    {
        client = inClient;
    }

    public bool Init(string inId)
    {
        id = inId;

        if (!Add(this))
            return false;

        Console.WriteLine($"LogIn : {inId}");

        return true;
    }

    public void Release()
    {
        Client?.Close();

        if (!string.IsNullOrEmpty(id))
        {
            Console.WriteLine($"LogOut : {id}");
            ActionHandler.OnBroadcastSightLeaveNotify(this, new List<(int c, int r)>());
            Remove(id);
        }
    }

    public void SetPosition(float inX, float inY)
    {
        var prevCellIdx = IndexUtil.CalcCellIdx(x, y);
        var nextCellIdx = IndexUtil.CalcCellIdx(inX, inY);
        var moveCellIdx = (nextCellIdx.c - prevCellIdx.c, nextCellIdx.r - prevCellIdx.r);

        if (prevCellIdx != nextCellIdx)
        {
            ActionHandler.OnBroadcastSightLeaveNotify(this, IndexUtil.GetMoveNearCell(this, false, moveCellIdx));
            ActionHandler.OnSendSpawnLeaveNotify(this, moveCellIdx);
        }

        x = inX;
        y = inY;

        if (prevCellIdx != nextCellIdx)
        {
            cellIdx = nextCellIdx;

            ActionHandler.OnBroadcastSightEnterNotify(this, IndexUtil.GetMoveNearCell(this, true, moveCellIdx));
            ActionHandler.OnSendSpawnEnterNotify(this, moveCellIdx);
        }
    }

    public static bool Add(User inUser)
    {
        return userDic.TryAdd(inUser.Id, inUser);
    }

    public static bool Remove(string inId)
    {
        if (string.IsNullOrEmpty(inId))
            return false;

        return userDic.TryRemove(inId, out _);
    }
}
