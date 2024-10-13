using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public abstract class Map : Section
    {
        private readonly SettingsMap _settings;
        private readonly List<Biome> _biomes = new List<Biome>();
        private readonly Dictionary<Coords, WeakReference> _tilesCache = new Dictionary<Coords, WeakReference>();

        protected int WorldLimitSize => _settings.WorldLimitSize;

        public Random RandomObject { get; }
        public int Seed => _settings.Seed;
        public override int Size => AllocatedCoords.Count;
        public IReadOnlyList<Biome> Biomes => _biomes;
        public IReadOnlyList<Landform> Landforms => Biomes.SelectMany(b => b.Landforms).ToList();
        public IReadOnlyList<MetaLandform> MetaLandforms => Landforms.Select(l => l.MetaLandform).ToList();
        public override IReadOnlyList<Tile> Tiles => Biomes.SelectMany(b => b.Tiles).ToList();

        protected Map(SettingsMap settingsMap)
        {
            _settings = settingsMap;

            // Initialize the RNG (random number generator) with the map's seed.
            RandomObject = new Random(settingsMap.Seed);
        }
        public override void Generate()
        {
        	// If settings are empty
        	if (_settings.SettingsBiome.Count == 0)
                return;

            // Generate the shape of the map
            Allocate(AllocateMap(_settings.SettingsBiome.Sum(b => b.MinimumTilesCount)));

            // Generate world limits
            generateWorldLimits();

            // Biomes space biomeAllocations
            IEnumerable<BiomeAllocation> allocationBiomes = allocateBiomes();

            foreach (BiomeAllocation allocation in allocationBiomes)
                _biomes.Add(new Biome(this, allocation.Settings, allocation.Allocated.ToList()));

            // Generate all biomes
            _biomes.ForEach(b => b.Generate());

            // Generate metalandforms
            generateMetaLandforms();

            base.Generate();
        }
        public void RegisterTile(Coords coords, Tile tile)
        {
            _tilesCache[coords] = new WeakReference(tile);
        }

        public Tile GetTileFromCoords(Coords coords)
        {
            if (_tilesCache.TryGetValue(coords, out var tile) && tile.IsAlive)
                return (Tile)tile.Target;
            
            return null;
        }
        public List<Tile> GetTilesInRange(Coords coords, int range, bool excludeWorldLimits = false)
        {
            var toSelect = coords.GetCoordsInRange(range);
            var tiles = Tiles.Where(t => toSelect.Contains(t.Coords));

            if (excludeWorldLimits)
                tiles = tiles.Where(t => t.Landform.GetType() != typeof(LandformWorldLimit)).ToList();

            return tiles.ToList();
        }
        public List<Tile> GetTilesInRange(Tile tile, int range, bool excludeWorldLimits = false)
        {
            return GetTilesInRange(tile.Coords, range, excludeWorldLimits);
        }
        public List<Tile> GetTilesInRange(Type landformType, Coords coords, int range)
        {
            return GetTilesInRange(coords, range)
                .Where(t => t.Landform.GetType() == landformType)
                .ToList();
        }
        public List<Tile> GetTilesInRange(Type landformType, Tile tile, int range)
        {
            return GetTilesInRange(landformType, tile.Coords, range);
        }

        protected abstract IEnumerable<Coords> AllocateMap(int size);
        protected virtual IEnumerable<Coords> AllocateWorldLimits()
        {
            return Coords.Expand(AllocatedCoords, WorldLimitSize)
                .Except(AllocatedCoords)
                .ToList();
        }

        private IEnumerable<BiomeAllocation> allocateBiomes()
        {
            List<BiomeAllocation> biomeAllocations = new List<BiomeAllocation>();

            // If there is only one biome to allocate, fill the whole map
            if (_settings.SettingsBiome.Count == 1)
                return new List<BiomeAllocation>() { new BiomeAllocation(_settings.SettingsBiome[0], AllocatedCoords.ToHashSet()) };

            // Foreach biome to allocate, pick a random start to expand from and initialize a BiomeAllocation
            foreach (SettingsBiome settings in _settings.SettingsBiome)
            {
                // Find Coords not already picked as a start for another biome
                List<Coords> availableCoords = AllocatedCoords
                    .Except(biomeAllocations.SelectMany(x => x.Allocated))
                    .ToList();

                Coords startCoords = availableCoords[RandomObject.Next(0, availableCoords.Count)];

                biomeAllocations.Add(new BiomeAllocation(settings, startCoords));
            }

            // This dictionary indicates if a biome is allowed to trim other biomes during expansion,
            // at the beginning, none are allowed to
            var biomesAllowedToTrim = new Dictionary<BiomeAllocation, bool>();
            biomeAllocations.ForEach(b => biomesAllowedToTrim[b] = false);

            bool allValid = false;

            // Keep allocating until every biome is valid or there is still unallocated space in the Map
            while (!allValid || biomeAllocations.Sum(b => b.Allocated.Count) < AllocatedCoords.Count)
            {
                // Find which biome to expand. If all biomes are already valid, select every biome, sort them randomly and prevent them from trimming
                // (this is used to fill the whole map)
                // Else, prioritize biome not already valid
                List<BiomeAllocation> biomeToExpand;
                if (allValid)
                {
                    biomeAllocations.ForEach(b => biomesAllowedToTrim[b] = false);
                    biomeToExpand = biomeAllocations.OrderBy(x => RandomObject.Next()).ToList();
                }
                else
                {
                    biomeToExpand = biomeAllocations.Where(b => !b.IsValid).OrderBy(x => x.TargetDelta).ToList();
                }

                // Foreach allocation to be done (sorted randomly to ensure good spreading)
                foreach (BiomeAllocation biome in biomeToExpand)
                {
                    // Generate the list of available coords to expand to
                    // If the biome is not allowed to trim, available coords are the ones not already allocated by another biome
                    // Else, if the biome can trim, available coords are all AllocatedCoords of the map
                    IEnumerable<Coords> available;
                    if (biomesAllowedToTrim[biome] == false)
                        available = AllocatedCoords.Except(biomeAllocations.Where(b => !b.Equals(biome)).SelectMany(b => b.Allocated)).ToList();
                    else
                        available = AllocatedCoords;

                    IEnumerable<Coords> newCoords = biome.Expand(available);

                    // If the biome was not able to expand, it means its fully surrounded by other biomes
                    // If so, allow it to trim other biomes next time
                    if (!newCoords.Any())
                    {
                        biomesAllowedToTrim[biome] = true;
                        continue;
                    }

                    int nbOfItems = newCoords.Count();
                    int nbOfItemsToAdd = (int) Math.Ceiling(nbOfItems * 0.8f);
                    int nbOfItemsAdded = 0;
                    int i = RandomObject.Next(0, nbOfItems);

                    while (nbOfItemsAdded < nbOfItemsToAdd)
                    {
                        Coords c = newCoords.ElementAt(i);
                        // Find if this coords is already allocated by another biome
                        BiomeAllocation biomeToTrim = biomeAllocations.FirstOrDefault(b => b.Allocated.Contains(c));

                        // If the coords is not allocated by another biome or if the biome is not allowed to trim
                        if (biomeToTrim == null || biomesAllowedToTrim[biome] == false)
                        {
                            biome.Allocate(c);
                            nbOfItemsAdded++;
                        }
                        // If the coords should be trimmed from the other biome
                        else if (biomesAllowedToTrim[biome]) 
                        {
                            biome.Allocate(c);
                            biomeToTrim.Deallocate(c);
                            nbOfItemsAdded++;
                        }

                        if (biome.TargetDelta == 0)
                            break;

                        i = (i + 1) % nbOfItems;
                    }
                }

                allValid = biomeAllocations.All(kpv => kpv.IsValid);
            }

            return biomeAllocations;
        }
        private void generateWorldLimits()
        {
            IEnumerable<Coords> worldLimitsCoords = AllocateWorldLimits();

            // Create a biome with only one world limit landform
            SettingsLandform settingsLandform = new SettingsLandform(typeof(LandformWorldLimit));
            SettingsBiome settingsBiomeWorldLimits = new SettingsBiome(SettingsMap.BiomeWorldLimitUUID, SettingsMap.BiomeWorldLimitName, new List<SettingsLandform>() { settingsLandform });
            Biome biomeWorldLimit = new Biome(this, settingsBiomeWorldLimits, worldLimitsCoords);

            biomeWorldLimit.Generate();
            _biomes.Add(biomeWorldLimit);
        }
        private void generateMetaLandforms()
        {
            List<Landform> toCheck = Landforms.ToList();

            while (toCheck.Any())
            {
                Landform current = toCheck.First();
                List<Landform> adjacentSameType = current
                    .AdjacentLandforms
                    .Where(l => l.GetType() == current.GetType())
                    .ToList();

                if (adjacentSameType.Any())
                {
                    List<MetaLandform> existingMetaLandforms = adjacentSameType
                        .Select(l => l.MetaLandform)
                        .Where(ml => ml != null)
                        .ToList();

                    // If there is no existing metalandform
                    if (existingMetaLandforms.Count == 0)
                    {
                        // Create a new one to regroup all adjacent landforms
                        MetaLandform ml = new MetaLandform();
                        ml.AddLandform(current);

                        foreach (Landform l in adjacentSameType) { 
                            ml.AddLandform(l);
                            toCheck.Remove(l);
                        }
                    }
                    else if (existingMetaLandforms.Count == 1)
                    {
                        MetaLandform ml = existingMetaLandforms.First();
                        ml.AddLandform(current);
                    }
                    else
                    {
                        List<Landform> landformsToGroup = existingMetaLandforms.SelectMany(m => m.Landforms).ToList();
                        landformsToGroup.Add(current);

                        MetaLandform ml = new MetaLandform();
                        foreach (Landform l in landformsToGroup)
                        {
                            ml.AddLandform(l);
                            toCheck.Remove(l);
                        }
                    }
                }
                else
                {
                    MetaLandform ml = new MetaLandform();
                    ml.AddLandform(current);
                }

                toCheck.Remove(current);
            }
        }
        
        private class BiomeAllocation
        {
            private IEnumerable<Coords> _available;
            private readonly HashSet<Coords> _allocated = new HashSet<Coords>();
            private readonly IEnumerator<IEnumerable<Coords>> _enumerator;

            public SettingsBiome Settings { get; }
            public IReadOnlyCollection<Coords> Allocated => _allocated;
            public int TargetDelta => Settings.MinimumTilesCount - _allocated.Count;
            public bool IsValid => _allocated.Count >= Settings.MinimumTilesCount;
            public Coords Start { get; }

            private BiomeAllocation(SettingsBiome settings)
            {
                Settings = settings;
                _enumerator = expand().GetEnumerator();
            }
            public BiomeAllocation(SettingsBiome settings, HashSet<Coords> allocated) : this(settings)
            {
                _allocated = allocated;
            }
            public BiomeAllocation(SettingsBiome settings, Coords start) : this(settings)
            {
                Start = start;
                _allocated.Add(start);
            }

            public IEnumerable<Coords> Expand(IEnumerable<Coords> available)
            {
                _available = available;

                _enumerator.MoveNext();

                return _enumerator.Current;
            }
            public void Allocate(Coords toAdd)
            {
                _allocated.Add(toAdd);
            }
            public void Deallocate(Coords toRemove)
            {
                _allocated.Remove(toRemove);
            }

            private IEnumerable<IEnumerable<Coords>> expand()
            {
                while (true)
                {
                    List<Coords> newCoords = Coords
                        .Expand(_allocated.ToList(), 1)
                        .Except(_allocated)
                        .Intersect(_available)
                        .ToList();

                    yield return newCoords;
                }
            }
        }
    }
}