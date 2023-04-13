using System;
using System.Linq;

namespace Hexamap
{
    public class SettingsLandform
    {
        public Type Type { get; }
        public int Distribution { get; }
        public int Quantity { get; }
        public int Order { get; }

        public SettingsLandform(Type type)
        {
            Type = type;
        }
        public SettingsLandform(Type type, int distribution, int quantity, int order) : this (type)
        {
            Distribution = distribution;
            Quantity = quantity;
            Order = order;
        }

        // "a" depends on "b" = "b" has to be generated before "a"
        public static bool DependencyCheck(SettingsLandform a, SettingsLandform b)
        {
            foreach(var d in a.Type.GetCustomAttributes(typeof(LandformDependency), true).Cast<LandformDependency>())
                if (d.Dependency == b.Type)
                    return true;

            return false;
        }
    }
}