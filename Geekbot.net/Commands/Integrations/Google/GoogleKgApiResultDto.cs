namespace Geekbot.net.Commands.Integrations.Google
{
    public class GoogleKgApiResultDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GoogleKgApiImageDto Image { get; set; }
        public GoogleKgApiDetailedDto DetailedDtoDescription { get; set; }
    }
}