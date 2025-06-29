using System.Collections.Generic;

public static class ActionHandler
{
    public static void OnBroadcastSightEnterNotify(User inUser, List<(int c, int r)> moveCellIdxList)
    {
        var sightEnterNotify = new SightEnterNotify();
        sightEnterNotify.playerInfo.id = inUser.Id;
        sightEnterNotify.playerInfo.x = inUser.X;
        sightEnterNotify.playerInfo.y = inUser.Y;

        PacketSelector.OnBroadcastClient(inUser, sightEnterNotify.Serialize(), moveCellIdxList);
    }

    public static void OnBroadcastSightLeaveNotify(User inUser, List<(int c, int r)> moveCellIdxList)
    {
        var sightLeaveNotify = new SightLeaveNotify();
        sightLeaveNotify.playerInfo.id = inUser.Id;
        sightLeaveNotify.playerInfo.x = inUser.X;
        sightLeaveNotify.playerInfo.y = inUser.Y;

        PacketSelector.OnBroadcastClient(inUser, sightLeaveNotify.Serialize(), moveCellIdxList);
    }

    public static void OnSendSpawnEnterNotify(User inUser, (int c, int r) moveCellIdx)
    {
        var spawnEnterNotify = new SpawnEnterNotify();
        spawnEnterNotify.playerInfoList = User.GetAddPlayerInfoList(inUser, moveCellIdx);

        PacketSelector.OnSendClient(inUser, spawnEnterNotify.Serialize());
    }

    public static void OnSendSpawnLeaveNotify(User inUser, (int c, int r) moveCellIdx)
    {
        var spawnLeaveNotify = new SpawnLeaveNotify();
        spawnLeaveNotify.removePlayerIdList = User.GetRemovePlayerIdList(inUser, moveCellIdx);

        PacketSelector.OnSendClient(inUser, spawnLeaveNotify.Serialize());
    }

    public static void OnMoveRq(User inUser, PacketBase inPacket)
    {
        var moveRq = inPacket as MoveRq;
        if (moveRq == null)
            return;

        if (string.IsNullOrEmpty(moveRq.moveInfo.id))
            return;

        inUser.SetPosition(moveRq.moveInfo.x, moveRq.moveInfo.y);

        OnBroadcastMoveNotify(inUser, moveRq.moveInfo);
    }

    public static void OnBroadcastMoveNotify(User inUser, MoveInfo inMoveInfo)
    {
        var moveNotify = new MoveNotify();
        moveNotify.moveInfo = inMoveInfo;

        PacketSelector.OnBroadcastClient(inUser, moveNotify.Serialize());
    }
}