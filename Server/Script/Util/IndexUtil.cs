using System;
using System.Collections.Generic;

public static class IndexUtil
{
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

        foreach (var pair in User.UserDic)
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

        foreach (var pair in User.UserDic)
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