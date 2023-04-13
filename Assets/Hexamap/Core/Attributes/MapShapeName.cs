using System;

namespace Hexamap
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MapShapeName : Attribute
    {
        public string Name { get; }

        public MapShapeName(string name)
        {
            Name = name;
        }
    }
}