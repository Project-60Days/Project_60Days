using System;
using System.Linq;
using UnityEngine;

namespace Hexamap
{
    public class HexamapController : MonoBehaviour
    {
        public bool GenerateOnStartup = true;
        public AssetsMap Settings;
        public GameObject Parent;

        public Map Map { get; private set; }

        public void Start()
        {
            if (GenerateOnStartup)
            {
                Generate();
                Draw();
            }
        }
        public void Generate()
        {
            // If no parent has been specified, create one at the root of the scene
            if (Parent == null)
                Parent = new GameObject("Hexamap");

            var settings = (SettingsMap)Settings;
            Map = (Map) Activator.CreateInstance(settings.Shape, settings);

            Map.Generate();
        }
        public void Draw()
        {
            foreach (Tile t in Map.Tiles)
                drawTile(t);
        }
        public void Destroy()
        {
            if (Map != null)
            {
                Map = null;
                foreach (Transform child in Parent.GetComponentInChildren<Transform>())
                    Destroy(child.gameObject);
            }
        }

        private void drawTile(Tile tile)
        {
            // Get the right prefab to use based on the name of the biome
            (GameObject prefab, int YRotation) = pickPrefab(tile);
            
            // Instantiate a new gameobject from the previous prefab
            GameObject tileGO = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 0, 0), Parent.transform);

            // Attach a TileController to the new gameobject
            TileController tileController = tileGO.AddComponent<TileController>();
            tileController.Initialize(tile, Settings.Padding);

            // Randomize orientation of the tile to make the map feel less uniform
            int orientation = pickOrientation(tile);
            tileGO.transform.Rotate(tileGO.transform.up, orientation + YRotation, Space.World);
        }
        private (GameObject, int) pickPrefab(Tile tile)
        {
            GameObject prefab;
            int YRotation;

            Type pickingMethod;
            GameObject[] prefabs;

            // If the tile is inside the special biome "worldlimit"
            if (tile.Landform.Biome.UUID == SettingsMap.BiomeWorldLimitUUID)
            {
                pickingMethod = Settings.WorldLimitsPrefabPickingMethod;
                prefabs = Settings.WorldLimitsPrefabs;
            }
            else
            {
                // Get the tile's landform settings
                Landforms landform = Settings.BiomeSettings
                    .First(b => b.UUID == tile.Landform.Biome.UUID)
                    .Landforms
                    .First(l => Type.GetType(l.TypeAsString) == tile.Landform.GetType());

                pickingMethod = landform.PrefabPickingMethod;
                prefabs = landform.Prefabs;
            }

            (prefab, YRotation) = (Tuple<GameObject, int>)pickingMethod.GetMethod("Pick").Invoke(null, new object[] { tile, prefabs });


            return (prefab, YRotation);
        }
        private int pickOrientation(Tile tile)
        {
            int orientation;
            OrientationMethod orientationMethod;

            // If the tile is inside the special biome "worldlimit"
            if (tile.Landform.Biome.UUID == SettingsMap.BiomeWorldLimitUUID)
                orientationMethod = Settings.WorldLimitsPrefabOrientationMethod;
            else
            {
                // Get the tile's landform settings
                orientationMethod = Settings.BiomeSettings
                .First(b => b.UUID == tile.Landform.Biome.UUID)
                .Landforms
                .First(l => Type.GetType(l.TypeAsString) == tile.Landform.GetType())
                .PrefabOrientationMethod;
            }

            switch (orientationMethod)
            {
                case OrientationMethod.Random:
                    orientation = Map.RandomObject.Next(1, 6) * (360 / 6);
                    break;
                default:
                    orientation = (int) orientationMethod;
                    break;
            }

            return orientation;
        }

    }
}