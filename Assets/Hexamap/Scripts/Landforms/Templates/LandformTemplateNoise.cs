using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public abstract class LandformTemplateNoise<T> : LandformDistribution where T : Landform
    {
        private static readonly List<Noise> _noises = new List<Noise>();
        private readonly FastNoise.NoiseType _noiseType;
        private Noise _noise;

        protected float NoiseFrequency { get; set; }

        protected LandformTemplateNoise(Biome biome, IEnumerable<Coords> allocatedCoords, FastNoise.NoiseType noiseType, int maxSize) : base(biome, allocatedCoords, maxSize, true)
        {
            _noiseType = noiseType;
        }

        protected override IEnumerable<Coords> AllocateLandform()
        {
            generateNoise();

            return _noise
                .Values
                .Where(kpv => AllocatedCoords.Contains(kpv.Key))
                .OrderByDescending(kpv => kpv.Value)
                .Take(MaxSize)
                .Select(kpv => kpv.Key)
                .ToList();
        }

        private void generateNoise()
        {
            _noise = _noises.FirstOrDefault(n => n.Map == Map);

            if (_noise == null)
            {
                int seedModifier = typeof(T).ToString().ToCharArray().Sum(Convert.ToInt32);

                int seed = Map.Seed + seedModifier % int.MaxValue;
                _noise = new Noise(Map, _noiseType, NoiseFrequency, seed);
                _noises.Add(_noise);
            }

            var noiseToGenerate = AllocatedCoords.Except(_noise.Values.Keys).ToList();
            _noise.Generate(noiseToGenerate);
        }

        private class Noise
        {
            private readonly FastNoise _fastNoise;

            public Map Map { get; }
            public Dictionary<Coords, float> Values { get; }

            public Noise(Map map, FastNoise.NoiseType noiseType, float frequency, int seed)
            {
                Map = map;
                Values = new Dictionary<Coords, float>();

                _fastNoise = new FastNoise();
                _fastNoise.SetFrequency(frequency);
                _fastNoise.SetNoiseType(noiseType);
                _fastNoise.SetSeed(seed);
            }

            public void Generate(IEnumerable<Coords> toGenerate)
            {
                foreach (Coords c in toGenerate)
                    Values[c] = _fastNoise.GetNoise(c.X, c.Y);
            }
        }
    }
}
