namespace Geekbot.net.Database
{
    public class SqlConnectionString
    {
        public string Server { get; set; } = "localhost";
        public string Port { get; set; } = "3306";
        public string Database { get; set; } = "geekbot";
        public string Username { get; set; } = "geekbot";
        public string Password { get; set; } = "";

        public override string ToString()
        {
            return $"Server={Server};Port={Port};Database={Database};Uid={Username};Pwd={Password};";
        }
    }
}