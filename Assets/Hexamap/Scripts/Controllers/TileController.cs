using UnityEngine;

namespace Hexamap
{
    public class TileController : MonoBehaviour
    {
        private Vector3 _tileBounds = Vector3.zero;

        public Tile Model { get; set; }
        public Vector2 Coords => Model.Coords.ToVector();

        public void Initialize(Tile model, float padding)
        {
            Model = model;
            Model.GameEntity = gameObject;
            transform.position = calculateWorldPosition(padding);
            name = $"{Model.Coords.ToString()} - {Model.Biome.Name} - {Model.Landform.GetType()}";
        }

        private Vector3 calculateWorldPosition(float padding)
        {
            if (_tileBounds == Vector3.zero)
                _tileBounds = GetComponentInChildren<Renderer>().bounds.size;

            var tileSizeX = _tileBounds.x;
            var tileSizeY = _tileBounds.z;

            // Apply padding
            tileSizeX += tileSizeX * padding;
            tileSizeY += tileSizeY * padding;

            float x = Coords.x * tileSizeX / 2 * 1.5f;
            float y = Coords.y * tileSizeY;

            if (Coords.x % 2 == 0)
                y = Coords.y * tileSizeY + tileSizeY / 2;

            return new Vector3(x, 0, y);
        }
    }
}