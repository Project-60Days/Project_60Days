using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public class SettingsBiome
    {
        public string UUID { get; }
        public string Name { get; }
        public int MinimumTilesCount { get; }
        public IReadOnlyList<SettingsLandform> Landforms { get; }

        public SettingsBiome(string uuid, string name, List<SettingsLandform> landforms)
        {
            UUID = uuid;
            Name = name;
            Landforms = landforms;

            Landforms = sortLandforms();
        }
        public SettingsBiome(string uuid, string name, int minimumTilesCount, List<SettingsLandform> landforms) : this(uuid, name, landforms)
        {
            MinimumTilesCount = minimumTilesCount;
        }

        private List<SettingsLandform> sortLandforms()
        {
            // Get fillers. Exclude them from the sorting process.
            var fillers = Landforms.Where(l => l.Type.BaseType == typeof(LandformFiller));

            // First, sort landforms (except fillers) by order
            var sortedByOrder = Landforms.Except(fillers).OrderBy(l => l.Order).ToList();

            // Then sort them topologically
            var sortedTopologically = TopologicalSort.StableOrder(sortedByOrder, SettingsLandform.DependencyCheck);

            // Re-add fillers to the end of the hierarchy
            var sorted = sortedTopologically.Concat(fillers).ToList();

            return sorted;
        }
    }
}

