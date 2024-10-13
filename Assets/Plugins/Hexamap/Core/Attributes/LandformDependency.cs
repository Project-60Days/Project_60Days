using System;

namespace Hexamap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class LandformDependency : Attribute
    {
        public Type Dependency { get; }

        public LandformDependency(Type dependency)
        {
            Dependency = dependency;
        }
    }
}
