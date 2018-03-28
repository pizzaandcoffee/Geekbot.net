using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Serilog;

namespace Geekbot.net.Lib.Media
{
    public class MediaProvider : IMediaProvider
    {
        private readonly Random _random;
        private readonly IGeekbotLogger _logger;
        private string[] _checkemImages;
        private string[] _pandaImages;
        private string[] _croissantImages;
        private string[] _squirrelImages;
        private string[] _pumpkinImages;
        private string[] _turtlesImages;
        private string[] _pinguinImages;
        private string[] _foxImages;
        
        public MediaProvider(IGeekbotLogger logger)
        {
            _random = new Random();
            _logger = logger;

            logger.Information("Geekbot", "Loading Media Files");
            
            LoadCheckem();
            LoadPandas();
            BakeCroissants();
            LoadSquirrels();
            LoadPumpkins();
            LoadTurtles();
            LoadPinguins();
            LoadFoxes();
        }

        private void LoadCheckem()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/checkEmPics"));
            _checkemImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_checkemImages.Length} CheckEm Images");
        }
        
        private void LoadPandas()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/pandas"));
            _pandaImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_pandaImages.Length} Panda Images");
        }
        
        private void BakeCroissants()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/croissant"));
            _croissantImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_croissantImages.Length} Croissant Images");
        }
        
        private void LoadSquirrels()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/squirrel"));
            _squirrelImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_squirrelImages.Length} Squirrel Images");
        }
        
        private void LoadPumpkins()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/pumpkin"));
            _pumpkinImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_pumpkinImages.Length} Pumpkin Images");
        }
        
        private void LoadTurtles()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/turtles"));
            _turtlesImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_turtlesImages.Length} Turtle Images");
        }
        
        private void LoadPinguins()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/pinguins"));
            _pinguinImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_pinguinImages.Length} Pinguin Images");
        }
        
        private void LoadFoxes()
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath("./Storage/foxes"));
            _foxImages = rawLinks.Split("\n");
            _logger.Debug("Geekbot", $"Loaded {_foxImages.Length} Foxes Images");
        }
        
        public string getCheckem()
        {
            return _checkemImages[_random.Next(0, _checkemImages.Length)];
        }
        
        public string getPanda()
        {
            return _pandaImages[_random.Next(0, _pandaImages.Length)];
        }
        
        public string getCrossant()
        {
            return _croissantImages[_random.Next(0, _croissantImages.Length)];
        }
        
        public string getSquirrel()
        {
            return _squirrelImages[_random.Next(0, _squirrelImages.Length)];
        }
        
        public string getPumpkin()
        {
            return _pumpkinImages[_random.Next(0, _pumpkinImages.Length)];
        }
        
        public string getTurtle()
        {
            return _turtlesImages[_random.Next(0, _turtlesImages.Length)];
        }
        
        public string getPinguin()
        {
            return _pinguinImages[_random.Next(0, _pinguinImages.Length)];
        }
        
        public string getFox()
        {
            return _foxImages[_random.Next(0, _foxImages.Length)];
        }
    }

    public interface IMediaProvider
    {
        string getCheckem();
        string getPanda();
        string getCrossant();
        string getSquirrel();
        string getPumpkin();
        string getTurtle();
        string getPinguin();
        string getFox();
    }
}