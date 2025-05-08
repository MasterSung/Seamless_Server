using System;

public static class WorldHandler
{
    public static void OnWorldMoveStartRq(User inUser, PacketBase inPacket)
    {
        var worldMoveStartRq = inPacket as WorldMoveStartRq;
        if (worldMoveStartRq == null)
            return;
    
        var worldMoveStartRp = new WorldMoveStartRp();

        PacketSelector.OnSendClient(inUser, worldMoveStartRp.Serialize());
    }

    public static void OnWorldMoveFinishRq(User inUser, PacketBase inPacket)
    {
        var worldMoveFinishRq = inPacket as WorldMoveFinishRq;
        if (worldMoveFinishRq == null)
            return;

        var worldMoveFinishRp = new WorldMoveFinishRp();
        worldMoveFinishRp.playerInfoList = User.GetPlayerInfoList(inUser.Id);

        PacketSelector.OnSendClient(inUser, worldMoveFinishRp.Serialize());
        ActionHandler.OnBroadcastSightEnterNotify(inUser);
    }
}