using System;

public static class LoginHandler
{
    public static void OnLoginRq(User inUser, PacketBase inPacket)
    {
        var loginRq = inPacket as LoginRq;
        if (loginRq == null)
            return;
    
        var loginRp = new LoginRp();
        loginRp.id = loginRq.id;

        if (string.IsNullOrEmpty(loginRq.id))
            loginRp.resultCode = ResultCode.FAILED;

        if (!inUser.Init(loginRq.id))
            loginRp.resultCode = ResultCode.FAILED;

        PacketSelector.OnSendClient(inUser, loginRp.Serialize());
    }
}