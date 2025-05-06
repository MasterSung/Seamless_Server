using System;

public static class PacketSelector
{
    public static void OnSendServer(User inUser, byte[] inData)
    {
        var packet = PacketHandler.DeserializePacket(inData);
        if (packet == null)
            return;

        switch (packet.PacketId)
        {
            case PacketEnum.LOGIN_RQ:
                LoginHandler.OnLoginRq(inUser, packet);
                break;
        }
    }

    public static void OnSendClient(User inUser, byte[] inData)
    {
        inUser.Stream.Write(inData, 0, inData.Length);
    }
}
