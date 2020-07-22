using System;
using System.Collections.Generic;
using System.IO;
using Geekbot.net.Lib.Logger;
using Geekbot.net.Lib.RandomNumberGenerator;

namespace Geekbot.net.Lib.Media
{
    public class MediaProvider : IMediaProvider
    {
        private readonly IRandomNumberGenerator _random;
        private readonly IGeekbotLogger _logger;
        private readonly string[] _pandaImages;
        private readonly string[] _croissantImages;
        private readonly string[] _squirrelImages;
        private readonly string[] _pumpkinImages;
        private readonly string[] _turtlesImages;
        private readonly string[] _penguinImages;
        private readonly string[] _foxImages;
        private readonly string[] _dabImages;
        
        public MediaProvider(IGeekbotLogger logger, IRandomNumberGenerator random)
        {
            _random = random;
            _logger = logger;

            logger.Information(LogSource.Geekbot, "Loading Media Files");
            LoadMedia("./Storage/pandas", ref _pandaImages);
            LoadMedia("./Storage/croissant", ref _croissantImages);
            LoadMedia("./Storage/squirrel", ref _squirrelImages);
            LoadMedia("./Storage/pumpkin", ref _pumpkinImages);
            LoadMedia("./Storage/turtles", ref _turtlesImages);
            LoadMedia("./Storage/penguins", ref _penguinImages);
            LoadMedia("./Storage/foxes", ref _foxImages);
            LoadMedia("./Storage/dab", ref _dabImages);
        }

        private void LoadMedia(string path, ref string[] storage)
        {
            var rawLinks = File.ReadAllText(Path.GetFullPath(path));
            storage = rawLinks.Split("\n");
            _logger.Trace(LogSource.Geekbot, $"Loaded {storage.Length} Images from ${path}");
        }

        public string GetMedia(MediaType type)
        {
            var collection = type switch
            {
                MediaType.Panda => _pandaImages,
                MediaType.Croissant => _croissantImages,
                MediaType.Squirrel => _squirrelImages,
                MediaType.Pumpkin => _pumpkinImages,
                MediaType.Turtle => _turtlesImages,
                MediaType.Penguin => _penguinImages,
                MediaType.Fox => _foxImages,
                MediaType.Dab => _dabImages,
                _ => new string[0]
            };

            return collection[_random.Next(0, collection.Length)];
        }
    }
}