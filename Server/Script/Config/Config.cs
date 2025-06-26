using System.Net;

public static class Config
{
    public static IPAddress IP => IPAddress.Parse("127.0.0.1");
    public static int Port => int.Parse("9000");

    public static int CellSize => 3;
}