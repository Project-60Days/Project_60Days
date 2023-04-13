using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public class Biome : Section
    {
        private readonly SettingsBiome _settings;
        private readonly List<Landform> _landforms = new List<Landform>();
        
        public Map Map { get; }
        public IReadOnlyCollection<Landform> Landforms => _landforms;
        public override IReadOnlyList<Tile> Tiles => Landforms.SelectMany(l => l.Tiles).ToList();

        public string UUID => _settings.UUID;
        public string Name => _settings.Name;
        public override int Size => AllocatedCoords.Count;

        public Biome(Map map, SettingsBiome settingsBiome, IEnumerable<Coords> allocatedCoords)
        {
            _settings = settingsBiome;
            Map = map;
            Allocate(allocatedCoords);
        }
        public override void Generate() {
            // Generate landforms
            foreach (SettingsLandform s in _settings.Landforms)
                if (AvailableCoords.Count > 0)
                    generateLandform(s);

            // For all landforms, find their neighbours
            _landforms.ForEach(l => l.FindAdjacentLandforms());

            base.Generate();
        }

        private void generateLandform(SettingsLandform settings)
        {
            IEnumerator<Landform> enumerator = null;

            // LandformFill
            if (settings.Type.IsSubclassOf(typeof(LandformFiller)))
                enumerator = generateLandformFiller(settings).GetEnumerator();

            // LandformDistribute
            if (settings.Type.IsSubclassOf(typeof(LandformDistribution)))
            {
                int targetCount = (int) Math.Ceiling(AllocatedCoords.Count * (settings.Distribution / 100f));
                enumerator = generateLandformQuantifiable(settings, targetCount).GetEnumerator();
            }

            // LandformQuantify
            if (settings.Type.IsSubclassOf(typeof(LandformQuantity)))
            {
                int targetCount = Math.Min(AvailableCoords.Count, settings.Quantity);
                enumerator = generateLandformQuantifiable(settings, targetCount).GetEnumerator();
            }

            // LandformAdaptable
            if (settings.Type.IsSubclassOf(typeof(LandformAdaptable)))
                enumerator = generateLandformAdaptable(settings).GetEnumerator();

            while (enumerator.MoveNext())
            {
                Landform l = enumerator.Current;

                if (!l.Tiles.Any())
                    break;

                var subLandforms = l.Subdivide();
                foreach (Landform sub in subLandforms)
                    _landforms.Add(sub);
            }
        }
        private IEnumerable<Landform> generateLandformFiller(SettingsLandform settings)
        {
            Landform landform = (Landform) Activator.CreateInstance(settings.Type, this, AvailableCoords);
            landform.Generate(isFiller: true);

            yield return landform;
        }
        private IEnumerable<Landform> generateLandformQuantifiable(SettingsLandform settings, int targetCount)
        {
            // If distribution is 100, consider the landform a filler for better performances
            if (settings.Distribution == 100)
                yield return generateLandformFiller(settings).First();

            int generated = 0;

            // Generate landforms until the target count is reached
            while (generated < targetCount && AvailableCoords.Count > 0)
            {
                Landform landform = (Landform)Activator.CreateInstance(settings.Type, this, AvailableCoords, (targetCount - generated));

                landform.Generate();
                generated += landform.Tiles.Count;

                yield return landform;
            }
        }
        private IEnumerable<Landform> generateLandformAdaptable(SettingsLandform settings)
        {
            // Generate landform until it returns null, the landform is in charge of controlling the flow in this case
            while (true)
            {
                Landform landform = (Landform)Activator.CreateInstance(settings.Type, this, AvailableCoords);
                landform.Generate();
                yield return landform;
            }
        }
    }
}