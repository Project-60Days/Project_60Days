using System;

namespace Hexamap
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PickingMethodName : Attribute
    {
        public string Name { get; }

        public PickingMethodName(string name)
        {
            Name = name;
        }
    }
}