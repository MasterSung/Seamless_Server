using System;

public static class ActionHandler
{
    public static void OnBroadcastSightEnterNotify(User inUser)
    {
        var sightEnterNotify = new SightEnterNotify();
        sightEnterNotify.playerInfo.id = inUser.Id;
        sightEnterNotify.playerInfo.x = inUser.X;
        sightEnterNotify.playerInfo.y = inUser.Y;

        PacketSelector.OnBroadcastClient(inUser, sightEnterNotify.Serialize());
    }

    public static void OnBroadcastSightLeaveNotify(User inUser)
    {
        var sightLeaveNotify = new SightLeaveNotify();
        sightLeaveNotify.playerInfo.id = inUser.Id;
        sightLeaveNotify.playerInfo.x = inUser.X;
        sightLeaveNotify.playerInfo.y = inUser.Y;

        PacketSelector.OnBroadcastClient(inUser, sightLeaveNotify.Serialize());
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