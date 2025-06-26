using System.IO;

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

            case PacketEnum.MOVE_RQ: ActionHandler.OnMoveRq(inUser, packet); break;

            case PacketEnum.WORLD_MOVE_START_RQ: WorldHandler.OnWorldMoveStartRq(inUser, packet); break;
            case PacketEnum.WORLD_MOVE_FINISH_RQ: WorldHandler.OnWorldMoveFinishRq(inUser, packet); break;
        }
    }

    public static void OnSendClient(User inUser, byte[] inData)
    {
        if (inUser.Stream != null)
            inUser.Stream.Write(inData, 0, inData.Length);

        //using (MemoryStream ms = new MemoryStream())
        //using (BinaryWriter writer = new BinaryWriter(ms))
        //{
        //    writer.Write(inData.Length);
        //    writer.Write(inData);

        //    byte[] frame = ms.ToArray();
        //    inUser.Stream.Write(frame, 0, frame.Length);
        //}
    }

    public static void OnBroadcastClient(User inUser, byte[] inBuffer, bool inIsExceptionSelf = true)
    {
        foreach (var pair in User.UserDic)
        {
            if (inIsExceptionSelf)
            {
                if (inUser.Id.Equals(pair.Key))
                    continue;
            }

            if (inUser.CellIdx != pair.Value.CellIdx)
                continue;

            OnSendClient(pair.Value, inBuffer);
        }
    }
}
