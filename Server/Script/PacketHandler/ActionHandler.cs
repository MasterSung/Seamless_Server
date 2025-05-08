using System;

public static class ActionHandler
{
    public static void OnBroadcastSightEnterNotify(User inUser)
    {
        var playerInfo = new PlayerInfo();
        playerInfo.id = inUser.Id;

        var sightEnterNotify = new SightEnterNotify();
        sightEnterNotify.playerInfo = playerInfo;

        var buffer = sightEnterNotify.Serialize();

        foreach(var pair in User.UserDic)
        {
            if (inUser.Id.Equals(pair.Key))
                continue;

            PacketSelector.OnSendClient(pair.Value, buffer);
        }
    }

    public static void OnBroadcastSightLeaveNotify(User inUser)
    {
        var playerInfo = new PlayerInfo();
        playerInfo.id = inUser.Id;

        var sightLeaveNotify = new SightLeaveNotify();
        sightLeaveNotify.playerInfo = playerInfo;

        var buffer = sightLeaveNotify.Serialize();

        foreach (var pair in User.UserDic)
        {
            if (inUser.Id.Equals(pair.Key))
                continue;

            PacketSelector.OnSendClient(pair.Value, buffer);
        }
    }
}