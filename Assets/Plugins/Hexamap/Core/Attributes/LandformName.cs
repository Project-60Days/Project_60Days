using System;

namespace Hexamap
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LandformName : Attribute
    {
        public string Name { get; }

        public LandformName(string name)
        {
            Name = name;
        }
    }
}