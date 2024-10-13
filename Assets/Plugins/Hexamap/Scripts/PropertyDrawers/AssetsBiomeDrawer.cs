#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// warning : the whole file is a mess. im sorry...
namespace Hexamap
{
    [CustomPropertyDrawer(typeof(AssetsBiome))]
    public class AssetsBiomePropertyDrawer : PropertyDrawer
    {
        private const int FieldHeight = 16;
        private const int Padding = 5;

        private List<Type> _landforms;
        private Dictionary<string, string> _pickingMethods;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return FieldHeight;
            int height = (FieldHeight + Padding) * 3;

            SerializedProperty landformSelectedProperty = property.FindPropertyRelative("Landforms");
            for (int i = 0; i < landformSelectedProperty.arraySize; i++)
            {
                var currentLandform = landformSelectedProperty.GetArrayElementAtIndex(i);
                Type currentLandformType = Type.GetType(currentLandform.FindPropertyRelative("TypeAsString").stringValue);

                if (currentLandformType == null)
                    continue;

                if (currentLandform.FindPropertyRelative("IsSelected").boolValue)
                {
                    height += (FieldHeight + Padding) * 2;

                    if (!landformIsFiller(currentLandformType))
                        height += (FieldHeight + Padding) * 2;

                    if (currentLandform.FindPropertyRelative("PrefabsIsExpanded").boolValue)
                    {
                        height += (FieldHeight + Padding) * 3;
                        height += (FieldHeight + Padding) * currentLandform.FindPropertyRelative("PrefabsArraySize").intValue;
                    }
                }
                else
                    height += (FieldHeight + Padding);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get properties
            SerializedProperty nameProperty = property.FindPropertyRelative("Name");
            SerializedProperty minimumTilesCountProperty = property.FindPropertyRelative("MinimumTilesCount");
            SerializedProperty landformsProperty = property.FindPropertyRelative("Landforms");

            // Get landforms (classes with attribute LandformName)
            _landforms = ReflectionData.Landforms;
            _landforms.Remove(typeof(LandformWorldLimit));

            // Get picking methods
            _pickingMethods = ReflectionData.PickingMethods;

            // Create element in the array foreach landform
            if (landformsProperty.arraySize != _landforms.Count)
            {
                // Find which entry to add or remove
                List<Type> actual = new List<Type>();
                List<string> toDelete = new List<string>();
                for (int i = 0; i < landformsProperty.arraySize; i++)
                {
                    var element = landformsProperty.GetArrayElementAtIndex(i);

                    string typeAsString = landformsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("TypeAsString").stringValue;
                    Type t = Type.GetType(typeAsString);

                    // If the type is null it means reflection is not able to find it, it does not exist anymore
                    if (t != null)
                        actual.Add(t);
                    else
                        toDelete.Add(typeAsString);
                }

                var toAdd = _landforms.Except(actual).ToList();

                // Add
                foreach (var landform in toAdd)
                {
                    landformsProperty.InsertArrayElementAtIndex(0);
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("TypeAsString").stringValue = landform.ToString();
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("IsSelected").boolValue = false;
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("Distribution").intValue = 0;
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("Quantity").intValue = 0;
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("Order").intValue = 0;
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("PrefabsIsExpanded").boolValue = true;
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("PrefabsArraySize").intValue = 1;
                    landformsProperty.GetArrayElementAtIndex(0).FindPropertyRelative("Prefabs").arraySize = 1;
                }

                // Delete
                foreach (var landform in toDelete)
                {
                    for (int i = 0; i < landformsProperty.arraySize; i++)
                    {
                        var element = landformsProperty.GetArrayElementAtIndex(i);
                        var typeAsString = landformsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("TypeAsString").stringValue;
                        if (typeAsString == landform)
                        {
                            landformsProperty.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
            }

            // Set indentation to 0
            var indent = EditorGUI.indentLevel;

            // Foldout header
            var rect = new Rect(position.x, position.y, position.width, FieldHeight);
            string header = nameProperty.stringValue == string.Empty ? "Unnamed" : nameProperty.stringValue;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, header, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Name
                rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                EditorGUI.PropertyField(rect, nameProperty, new GUIContent("Name"));

                // Minimum Tile Count
                rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                minimumTilesCountProperty.intValue =
                    Mathf.Max(
                        10,
                        EditorGUI.IntField(rect, new GUIContent("Min. Tiles Count"), minimumTilesCountProperty.intValue));

                // Landforms
                foreach (var landform in _landforms)
                {
                    // Get the name of the landform (retrieve the Name property of the Attribute)
                    string landformName = (landform.GetCustomAttributes(typeof(LandformName), false).First() as LandformName).Name;

                    // Find the landform in the selection serialized property
                    SerializedProperty landformSelection = findLandformInPropertyByName(landform.ToString(), landformsProperty);

                    if (landformSelection == null)
                        continue;

                    rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);

                    // Checkbox for selection
                    var pptyIsSelected = landformSelection.FindPropertyRelative("IsSelected");
                    bool hasDependenciesSelected = landformHasDependencySelected(landform, landformsProperty);
                    bool otheFillerSelected = otherFillerIsSelected(landform, landformsProperty);

                    EditorGUI.BeginDisabledGroup(hasDependenciesSelected || otheFillerSelected);
                    pptyIsSelected.boolValue = EditorGUI.ToggleLeft(rect, new GUIContent(landformName), pptyIsSelected.boolValue);
                    EditorGUI.EndDisabledGroup();

                    // If the landform is selected
                    var pptyDistribution = landformSelection.FindPropertyRelative("Distribution");
                    var pptyQuantity = landformSelection.FindPropertyRelative("Quantity");
                    var pptyOrder = landformSelection.FindPropertyRelative("Order");
                    var pptyPrefabsIsExpanded = landformSelection.FindPropertyRelative("PrefabsIsExpanded");
                    var pptyPrefabsArraySize = landformSelection.FindPropertyRelative("PrefabsArraySize");
                    var pptyPrefabPickingMethodPopupIndex = landformSelection.FindPropertyRelative("PrefabPickingMethodPopupIndex");
                    var pptyPrefabPickingMethodAsString = landformSelection.FindPropertyRelative("PrefabPickingMethodTypeAsString");
                    var pptyPrefabOrientationMethod = landformSelection.FindPropertyRelative("PrefabOrientationMethod");
                    var pptyPrefabs = landformSelection.FindPropertyRelative("Prefabs");

                    if (landformSelection.FindPropertyRelative("IsSelected").boolValue)
                    {
                        EditorGUI.indentLevel++;

                        // If the landform is distributed, display the "Distribution" property
                        if (landformIsDistributed(landform))
                        {
                            // Int slider for distribution
                            rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                            pptyDistribution.intValue = Mathf.Clamp(EditorGUI.IntField(rect, new GUIContent("Distribution"), pptyDistribution.intValue), 1, 100);
                        }
                        else // Else, set the property to 0 in order to not mess up calculations
                        {
                            pptyDistribution.intValue = 0;
                        }

                        // If the landform is quantified, display the "Quantity" property
                        if (landformIsQuantified(landform))
                        {
                            // Int slider for distribution
                            rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                            pptyQuantity.intValue = Mathf.Clamp(EditorGUI.IntField(rect, new GUIContent("Quantity"), pptyQuantity.intValue), 1, 1000);
                        }

                        // If the landform is not a filler, display the "Order" property
                        if (!landformIsFiller(landform))
                        {
                            // Int slider for order
                            rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                            pptyOrder.intValue = Mathf.Clamp(EditorGUI.IntField(rect, new GUIContent("Order"), pptyOrder.intValue), 0, 500);
                        }

                        // Foldout prefabs
                        // Header (foldout prefabs)
                        rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                        pptyPrefabsIsExpanded.boolValue = EditorGUI.Foldout(rect, pptyPrefabsIsExpanded.boolValue, "Prefabs", true);

                        // Content (foldout prefabs)
                        if (pptyPrefabsIsExpanded.boolValue)
                        {
                            EditorGUI.indentLevel++;

                            // Picking method
                            rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);

                            int popupIndex = 0;
                            for (int i = 0; i < _pickingMethods.Count; i++)
                            {
                                if (_pickingMethods.ElementAt(i).Key == pptyPrefabPickingMethodAsString.stringValue)
                                {
                                    popupIndex = i;
                                    break;
                                }
                            }

                            pptyPrefabPickingMethodPopupIndex.intValue = EditorGUI.Popup(rect, "Pick", popupIndex, _pickingMethods.Select(x => x.Value).ToArray());
                            pptyPrefabPickingMethodAsString.stringValue = _pickingMethods.ElementAt(pptyPrefabPickingMethodPopupIndex.intValue).Key;

                            // Rotation method
                            rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                            EditorGUI.PropertyField(rect, pptyPrefabOrientationMethod, new GUIContent("Rotate"));

                            // Size
                            rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                            pptyPrefabsArraySize.intValue = Mathf.Clamp(EditorGUI.IntField(rect, new GUIContent("Size"), pptyPrefabsArraySize.intValue), 1, 100);

                            pptyPrefabs.arraySize = pptyPrefabsArraySize.intValue;

                            // Objects
                            for (int i = 0; i < pptyPrefabs.arraySize; i++)
                            {
                                var currentPrefab = pptyPrefabs.GetArrayElementAtIndex(i);

                                rect = new Rect(position.x, rect.y + rect.height + Padding, position.width, FieldHeight);
                                EditorGUI.PropertyField(rect, currentPrefab, new GUIContent(i.ToString()));
                            }
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        pptyDistribution.intValue = 0;
                    }

                }
                EditorGUI.indentLevel--;
            }

            // Adjusts distribution sliders to not go above 100
            redistributeLandforms(landformsProperty);

            // Select dependencies
            selectLandformDependencies(landformsProperty);

            // Reset indentation
            EditorGUI.indentLevel = indent;
        }

        private bool otherFillerIsSelected(Type landform, SerializedProperty property)
        {
            if (!landformIsFiller(landform))
                return false;

            for (int i = 0; i < property.arraySize; i++)
            {
                var currentLandform = property.GetArrayElementAtIndex(i);
                Type currentLandformType = Type.GetType(currentLandform.FindPropertyRelative("TypeAsString").stringValue);
                if (currentLandformType != landform
                    && landformIsFiller(currentLandformType)
                    && currentLandform.FindPropertyRelative("IsSelected").boolValue)
                {
                    return true;
                }
            }

            return false;
        }

        private bool landformHasDependencySelected(Type landform, SerializedProperty property)
        {
            foreach (Type l in _landforms)
            {
                var dependencies = l
                    .GetCustomAttributes(typeof(LandformDependency), false)
                    .Cast<LandformDependency>()
                    .Where(d => d.Dependency == landform)
                    .ToList();

                foreach (LandformDependency d in dependencies)
                {
                    var serializedDependency = findLandformInPropertyByName(l.ToString(), property);
                    if (serializedDependency != null && serializedDependency.FindPropertyRelative("IsSelected").boolValue)
                        return true;
                }
            }

            return false;
        }

        private SerializedProperty findLandformInPropertyByName(string landformName, SerializedProperty property)
        {
            for (int i = 0; i < property.arraySize; i++)
                if (property.GetArrayElementAtIndex(i).FindPropertyRelative("TypeAsString").stringValue == landformName)
                    return property.GetArrayElementAtIndex(i);
            return null;
        }

        private void redistributeLandforms(SerializedProperty property)
        {
            // Calculate the sum of distribution
            float sumDistribution = 0;
            for (int i = 0; i < property.arraySize; i++)
            {
                int distribution = property.GetArrayElementAtIndex(i).FindPropertyRelative("Distribution").intValue;
                sumDistribution += distribution;
            }

            // If the sum is above 100, readjust each slider to make it 100
            if (sumDistribution > 100)
                for (int i = 0; i < property.arraySize; i++)
                {
                    var distribution = property.GetArrayElementAtIndex(i).FindPropertyRelative("Distribution");
                    distribution.intValue =
                        (int)(distribution.intValue / sumDistribution * 100f);
                }
        }

        private void selectLandformDependencies(SerializedProperty property)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                var currentLandform = property.GetArrayElementAtIndex(i);
                // Get type
                string typeName = currentLandform.FindPropertyRelative("TypeAsString").stringValue;
                Type landformType = Type.GetType(typeName);

                // Find the dependencies to select
                if (currentLandform.FindPropertyRelative("IsSelected").boolValue)
                {
                    var dependencies = landformType.GetCustomAttributes(typeof(LandformDependency), false).Cast<LandformDependency>();
                    foreach (LandformDependency d in dependencies)
                    {
                        var serializedDependency = findLandformInPropertyByName(d.Dependency.ToString(), property);
                        serializedDependency.FindPropertyRelative("IsSelected").boolValue = true;
                    }
                }
            }
        }

        private bool landformIsFiller(Type landform)
        {
            return landform.IsSubclassOf(typeof(LandformFiller));
        }
        private bool landformIsDistributed(Type landform)
        {
            return landform.IsSubclassOf(typeof(LandformDistribution));
        }
        private bool landformIsQuantified(Type landform)
        {
            return landform.IsSubclassOf(typeof(LandformQuantity));
        }
    }
}
#endif