namespace Geekbot.net.Database
{
    public class SqlConnectionString
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            return $"Server={Host};Port={Port};Database={Database};Uid={Username};Pwd={Password};";
        }
    }
}