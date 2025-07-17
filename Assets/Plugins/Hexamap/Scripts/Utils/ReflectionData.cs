using System;
using System.Collections.Generic;
using System.Linq;

namespace Hexamap
{
    public static class ReflectionData
    {
        private static Dictionary<string, string> _pickingMethods;
        private static List<Type> _landforms;
        private static Dictionary<string, string> _mapShapes;

        public static Dictionary<string, string> PickingMethods
        {
            get
            {
                if (_pickingMethods == null)
                    Refresh();
                return _pickingMethods;
            }
        }
        public static List<Type> Landforms
        {
            get
            {
                if (_landforms == null)
                    Refresh();
                return _landforms;
            }
        }
        public static Dictionary<string, string> MapShapes
        {
            get
            {
                if (_mapShapes == null)
                    Refresh();
                return _mapShapes;
            }
        }

        public static void Refresh()
        {
            _pickingMethods = getPickingMethods();
            _landforms = getLandforms();
            _mapShapes = getMapShapes();
        }

        private static Dictionary<string, string> getPickingMethods()
        {
            var methods = new Dictionary<string, string>();

            // Search in all assemblies classes with the custom attribute PickingMethodName
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                foreach (Type type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(PickingMethodName), true);
                    if (attributes.Length > 0)
                    {
                        string methodName = (attributes[0] as PickingMethodName).Name;
                        methods.Add(type.ToString(), methodName);
                    }
                }

            return methods;
        }

        private static List<Type> getLandforms()
        {
            var landforms = new List<Type>();

            // Search in all assemblies classes with the custom attribute LandformName
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                foreach (Type type in assembly.GetTypes())
                    if (type.GetCustomAttributes(typeof(LandformName), true).Length > 0)
                        landforms.Add(type);

            landforms = landforms.OrderBy(l => l.ToString()).ToList();

            return landforms;
        }

        private static Dictionary<string, string> getMapShapes()
        {
            var shapes = new Dictionary<string, string>();

            // Search in all assemblies classes with the custom attribute LandformName
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                foreach (Type type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(MapShapeName), true);
                    if (attributes.Length > 0)
                    {
                        string shapeName = (attributes[0] as MapShapeName).Name;
                        shapes.Add(type.ToString(), shapeName);
                    }
                }

            return shapes;
        }
    }
}
