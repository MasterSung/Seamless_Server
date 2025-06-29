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
        var prevCellIdx = CalcCellIdx(x, y);
        var nextCellIdx = CalcCellIdx(inX, inY);
        var moveCellIdx = (nextCellIdx.c - prevCellIdx.c, nextCellIdx.r - prevCellIdx.r);

        if (prevCellIdx != nextCellIdx)
        {
            ActionHandler.OnBroadcastSightLeaveNotify(this, GetMoveNearCell(this, false, moveCellIdx));
            ActionHandler.OnSendSpawnLeaveNotify(this, moveCellIdx);
        }

        x = inX;
        y = inY;

        if (prevCellIdx != nextCellIdx)
        {
            cellIdx = nextCellIdx;

            ActionHandler.OnBroadcastSightEnterNotify(this, GetMoveNearCell(this, true, moveCellIdx));
            ActionHandler.OnSendSpawnEnterNotify(this, moveCellIdx);
        }

        //Console.WriteLine($"x:{inX} y:{inY} / c:{nextCellIdx.c} r:{nextCellIdx.r}");
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

    public static (int c, int r) CalcCellIdx(float inX, float inY)
    {
        return ((int)Math.Floor(inX / Config.CellSize), (int)Math.Floor(inY / Config.CellSize));
    }

    public static bool IsNearCell((int c, int r) inUserCellIdx, (int c, int r) inCompareCellIdx)
    {
        return (inUserCellIdx.c + -1 == inCompareCellIdx.c && inUserCellIdx.r + 1 == inCompareCellIdx.r) ||
               (inUserCellIdx.c == inCompareCellIdx.c && inUserCellIdx.r + 1 == inCompareCellIdx.r) ||
               (inUserCellIdx.c + 1 == inCompareCellIdx.c && inUserCellIdx.r + 1 == inCompareCellIdx.r) ||
               (inUserCellIdx.c + -1 == inCompareCellIdx.c && inUserCellIdx.r == inCompareCellIdx.r) ||
               (inUserCellIdx.c == inCompareCellIdx.c && inUserCellIdx.r == inCompareCellIdx.r) ||
               (inUserCellIdx.c + 1 == inCompareCellIdx.c && inUserCellIdx.r == inCompareCellIdx.r) ||
               (inUserCellIdx.c + -1 == inCompareCellIdx.c && inUserCellIdx.r + -1 == inCompareCellIdx.r) ||
               (inUserCellIdx.c == inCompareCellIdx.c && inUserCellIdx.r + -1 == inCompareCellIdx.r) ||
               (inUserCellIdx.c + 1 == inCompareCellIdx.c && inUserCellIdx.r + -1 == inCompareCellIdx.r);
    }

    public static List<(int c, int r)> GetMoveNearCell(User inUser, bool isEnter, (int c, int r) moveCellIdx)
    {
        var cellIdxList = new List<(int c, int r)>();
        var currentCellIdx = inUser.CellIdx;

        if (isEnter)
        {
            // 오른쪽으로 이동
            if (moveCellIdx.c > 0)
            {
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + -1));
            }
            // 왼쪽으로 이동
            else if (moveCellIdx.c < 0)
            {
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r));
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + -1));
            }

            // 위로 이동
            if (moveCellIdx.r > 0)
            {
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + 1));
            }
            // 아래로 이동
            else if (moveCellIdx.r < 0)
            {
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + -1));
                cellIdxList.Add((currentCellIdx.c, currentCellIdx.r + -1));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + -1));
            }
        }
        else
        {
            // 오른쪽으로 이동
            if (moveCellIdx.c > 0)
            {
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r));
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + -1));
            }
            // 왼쪽으로 이동
            else if (moveCellIdx.c < 0)
            {
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + -1));
            }

            // 위로 이동
            if (moveCellIdx.r > 0)
            {
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + -1));
                cellIdxList.Add((currentCellIdx.c, currentCellIdx.r + -1));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + -1));
            }
            // 아래로 이동
            else if (moveCellIdx.r < 0)
            {
                cellIdxList.Add((currentCellIdx.c + -1, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c, currentCellIdx.r + 1));
                cellIdxList.Add((currentCellIdx.c + 1, currentCellIdx.r + 1));
            }
        }

        return cellIdxList;
    }

    public static List<PlayerInfo> GetAddPlayerInfoList(User inUser, (int c, int r) moveCellIdx)
    {
        var playerInfoList = new List<PlayerInfo>();

        (int c, int r) cellIdx = CalcCellIdx(inUser.X, inUser.Y);

        foreach (var pair in userDic)
        {
            var targetUser = pair.Value;
            if (targetUser == null)
                continue;

            if (targetUser.Id.Equals(inUser.Id))
                continue;

            var targetCellIdx = CalcCellIdx(targetUser.X, targetUser.Y);
            bool isCondition = false;

            // 처음 접속 했을때
            if (moveCellIdx.c == 0 && moveCellIdx.r == 0)
            {
                isCondition = IsNearCell(cellIdx, targetCellIdx);
            }
            // 오른쪽으로 이동
            else if (moveCellIdx.c > 0)
            {
                isCondition = (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r);
            }
            // 왼쪽으로 이동
            else if (moveCellIdx.c < 0)
            {
                isCondition = (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r == targetCellIdx.r) ||
                              (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r);
            }
            // 위로 이동
            else if (moveCellIdx.r > 0)
            {
                isCondition = (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r);
            }
            // 아래로 이동
            else if (moveCellIdx.r < 0)
            {
                isCondition = (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r) ||
                              (cellIdx.c == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r);
            }

            if (!isCondition)
                continue;

            var playerInfo = new PlayerInfo();
            playerInfo.id = pair.Key;
            playerInfo.x = targetUser.X;
            playerInfo.y = targetUser.Y;

            playerInfoList.Add(playerInfo);
        }

        return playerInfoList;
    }

    public static List<string> GetRemovePlayerIdList(User inUser, (int c, int r) moveCellIdx)
    {
        var removePlayerIdList = new List<string>();
        if (moveCellIdx.c == 0 && moveCellIdx.r == 0)
            return removePlayerIdList;

        (int c, int r) cellIdx = CalcCellIdx(inUser.X, inUser.Y);

        foreach (var pair in userDic)
        {
            var targetUser = pair.Value;
            if (targetUser == null)
                continue;

            if (targetUser.Id.Equals(inUser.Id))
                continue;

            var targetCellIdx = CalcCellIdx(targetUser.X, targetUser.Y);
            bool isCondition = false;

            // 오른쪽으로 이동
            if (moveCellIdx.c > 0)
            {
                isCondition = (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r == targetCellIdx.r) ||
                              (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r);
            }
            // 왼쪽으로 이동
            else if (moveCellIdx.c < 0)
            {
                isCondition = (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r);
            }
            // 위로 이동
            else if (moveCellIdx.r > 0)
            {
                isCondition = (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r) ||
                              (cellIdx.c == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + -1 == targetCellIdx.r);
            }
            // 아래로 이동
            else if (moveCellIdx.r < 0)
            {
                isCondition = (cellIdx.c + -1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r) ||
                              (cellIdx.c + 1 == targetCellIdx.c && cellIdx.r + 1 == targetCellIdx.r);
            }

            if (!isCondition)
                continue;

            removePlayerIdList.Add(targetUser.Id);
        }

        return removePlayerIdList;
    }
}
