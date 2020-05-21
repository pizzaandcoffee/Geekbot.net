using System;
using System.IO;
using Geekbot.net.Lib.Logger;

namespace Geekbot.net.Lib.Media
{
    public class MediaProvider : IMediaProvider
    {
        private readonly Random _random;
        private readonly IGeekbotLogger _logger;
        private string[] _pandaImages;
        private string[] _croissantImages;
        private string[] _squirrelImages;
        private string[] _pumpkinImages;
        private string[] _turtlesImages;
        private string[] _pinguinImages;
        private string[] _foxImages;
        private string[] _dabImages;
        
        public MediaProvider(IGeekbotLogger logger)
        {
            _random = new Random();
            _logger = logger;

            logger.Information(LogSource.Geekbot, "Loading Media Files");
;
            LoadPandas();
            BakeCroissants();
            LoadSquirrels();
            LoadPumpkins();
            LoadTurtles();
            LoadPinguins();
            LoadFoxes();
            LoadDab();
        }
        
        private void LoadPandas()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/pandas"));
            _pandaImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_pandaImages.Length} Panda Images");
        }
        
        private void BakeCroissants()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/croissant"));
            _croissantImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_croissantImages.Length} Croissant Images");
        }
        
        private void LoadSquirrels()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/squirrel"));
            _squirrelImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_squirrelImages.Length} Squirrel Images");
        }
        
        private void LoadPumpkins()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/pumpkin"));
            _pumpkinImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_pumpkinImages.Length} Pumpkin Images");
        }
        
        private void LoadTurtles()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/turtles"));
            _turtlesImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_turtlesImages.Length} Turtle Images");
        }
        
        private void LoadPinguins()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/pinguins"));
            _pinguinImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_pinguinImages.Length} Pinguin Images");
        }
        
        private void LoadFoxes()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/foxes"));
            _foxImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_foxImages.Length} Foxes Images");
        }
        
        private void LoadDab()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/dab"));
            _dabImages = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {_dabImages.Length} Dab Images");
        }

        public string GetPanda()
        {
            return _pandaImages[_random.Next(0, _pandaImages.Length)];
        }
        
        public string GetCrossant()
        {
            return _croissantImages[_random.Next(0, _croissantImages.Length)];
        }
        
        public string GetSquirrel()
        {
            return _squirrelImages[_random.Next(0, _squirrelImages.Length)];
        }
        
        public string GetPumpkin()
        {
            return _pumpkinImages[_random.Next(0, _pumpkinImages.Length)];
        }
        
        public string GetTurtle()
        {
            return _turtlesImages[_random.Next(0, _turtlesImages.Length)];
        }
        
        public string GetPinguin()
        {
            return _pinguinImages[_random.Next(0, _pinguinImages.Length)];
        }
        
        public string GetFox()
        {
            return _foxImages[_random.Next(0, _foxImages.Length)];
        }
        
        public string GetDab()
        {
            return _dabImages[_random.Next(0, _dabImages.Length)];
        }
    }
}