using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexamap
{
    [Serializable]
    public class AssetsBiome
    {
        public string UUID;
        public string Name;
        public int MinimumTilesCount;
        public Landforms[] Landforms;

        public bool IsExpanded;

        public static explicit operator SettingsBiome(AssetsBiome unity)
        {
            List<SettingsLandform> landforms = new List<SettingsLandform>();

            foreach (Landforms landform in unity.Landforms)
            {
                if (landform.IsSelected)
                {
                    Type landformType = Type.GetType(landform.TypeAsString);

                    if ((landformType.IsSubclassOf(typeof(LandformDistribution)) && landform.Distribution > 0)
                        || (landformType.IsSubclassOf(typeof(LandformQuantity)) && landform.Quantity > 0)
                        || landformType.IsSubclassOf(typeof(LandformFiller))
                        || landformType.IsSubclassOf(typeof(LandformAdaptable)))
                    {
                        landforms.Add((SettingsLandform)landform);
                    }
                }
            }

            unity.UUID = Guid.NewGuid().ToString();

            return new SettingsBiome(unity.UUID, unity.Name, unity.MinimumTilesCount, landforms);
        }
    }

    [Serializable]
    public struct Landforms
    {
        public string TypeAsString;
        public bool IsSelected;
        public int Distribution;
        public int Quantity;
        public int Order;

        public int PrefabsArraySize;
        public bool PrefabsIsExpanded;
        public string PrefabPickingMethodTypeAsString;
        public int PrefabPickingMethodPopupIndex;
        public OrientationMethod PrefabOrientationMethod;
        public GameObject[] Prefabs;

        public Type PrefabPickingMethod => Type.GetType(PrefabPickingMethodTypeAsString);

        public static explicit operator SettingsLandform(Landforms unity)
        {
            return new SettingsLandform(Type.GetType(unity.TypeAsString), unity.Distribution, unity.Quantity, unity.Order);
        }
    }
}