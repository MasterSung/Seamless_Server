using System;

/// <summary>
/// 클라이언트로부터 받는 패킷들만 정의
/// </summary>

public static class PacketSelector
{
    public static void OnSendServer(User inUser, byte[] inData)
    {
        var packet = PacketHandler.DeserializePacket(inData);
        if (packet == null)
            return;

        switch (packet.PacketId)
        {
            case PacketEnum.LOGIN_RQ: LoginHandler.OnLoginRq(inUser, packet); break;

            case PacketEnum.WORLD_MOVE_START_RQ: WorldHandler.OnWorldMoveStartRq(inUser, packet); break;
            case PacketEnum.WORLD_MOVE_FINISH_RQ: WorldHandler.OnWorldMoveFinishRq(inUser, packet); break;
        }
    }

    public static void OnSendClient(User inUser, byte[] inData)
    {
        inUser.Stream.Write(inData, 0, inData.Length);
    }
}
