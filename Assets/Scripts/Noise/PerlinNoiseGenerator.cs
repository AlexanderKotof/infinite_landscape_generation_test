using UnityEngine;

namespace Test.Noise
{
    public class PerlinNoiseGenerator : INoiseGenerator
    {
        private readonly int _resolution;
        private readonly int _rnd;

        public PerlinNoiseGenerator(int resolution, int seed)
        {
            _resolution = resolution;
            // store random value for perlin noise offset
            _rnd = new System.Random(seed).Next(100);
        }

        public float[,] GenerateNoiseMap(int x, int y)
        {
            // Noise map shoold be 2^n + 1 size because of https://docs.unity3d.com/Manual/terrain-OtherSettings.html
            var noiseMap = new float[_resolution + 1, _resolution + 1];

            for (int i = 0; i <= _resolution; i++)
            {
                for (int j = 0; j <= _resolution; j++)
                {
                    // Assign 
                    noiseMap[j, i] = Mathf.PerlinNoise(
                        x + (float)i / _resolution + _rnd,
                        y + (float)j / _resolution + _rnd
                        );
                }
            }

            return noiseMap;
        }
    }
}
