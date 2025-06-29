using System.IO;
using System.Collections.Generic;

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
        if (inUser.Stream == null)
            return;

        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            writer.Write(inData.Length);
            writer.Write(inData);
            byte[] framed = ms.ToArray();
            inUser.Stream.Write(framed, 0, framed.Length);
        }
    }

    public static void OnBroadcastClient(User inUser, byte[] inBuffer, bool inIsExceptionSelf = true)
    {
        var userCellIdx = User.CalcCellIdx(inUser.X, inUser.Y);

        foreach (var pair in User.UserDic)
        {
            if (inIsExceptionSelf)
            {
                if (inUser.Id.Equals(pair.Key))
                    continue;
            }

            if (!User.IsNearCell(userCellIdx, pair.Value.CellIdx))
                continue;

            OnSendClient(pair.Value, inBuffer);
        }
    }

    public static void OnBroadcastClient(User inUser, byte[] inBuffer, List<(int c, int r)> inSendCellIdxList)
    {
        foreach (var pair in User.UserDic)
        {
            if (inUser.Id.Equals(pair.Key))
                continue;

            if (inSendCellIdxList.Count <= 0)
            {
                if (User.IsNearCell(inUser.CellIdx, pair.Value.CellIdx))
                    OnSendClient(pair.Value, inBuffer);
            }
            else
            {
                foreach (var compareCellIdx in inSendCellIdxList)
                {
                    if (pair.Value.CellIdx == compareCellIdx)
                        OnSendClient(pair.Value, inBuffer);
                }
            }
        }
    }
}
