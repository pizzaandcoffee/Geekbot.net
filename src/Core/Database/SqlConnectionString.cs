using System.Text;

namespace Geekbot.Core.Database
{
    public class SqlConnectionString
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RequireSsl { get; set; }
        public bool TrustServerCertificate { get; set; }
        public bool RedshiftCompatibility { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Application Name=Geekbot;");
            
            sb.Append($"Host={Host};");
            sb.Append($"Port={Port};");
            sb.Append($"Database={Database};");
            sb.Append($"Username={Username};");
            sb.Append($"Password={Password};");
            
            var sslMode = RequireSsl ? "Require" : "Prefer";
            sb.Append($"SSL Mode={sslMode};");
            sb.Append($"Trust Server Certificate={TrustServerCertificate.ToString()};");

            if (RedshiftCompatibility)
            {
                sb.Append("Server Compatibility Mode=Redshift");
            }

            return sb.ToString();
        }
    }
}