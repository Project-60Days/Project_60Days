using Hexamap;
using UnityEngine;

public static class ConversionExtension
{
    public static Vector2 ToVector(this Coords coords)
    {
        return new Vector2(coords.X, coords.Y);
    }
}
