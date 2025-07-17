using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hexamap.Unity
{
    [CustomEditor(typeof(AssetsMap))]
    public class MapSettingsEditor : Editor
    {
        private Dictionary<string, string> _mapShapes;
        private bool _showWorldLimitsSettings = true;
        private bool _showWorldLimitsPrefabs = true;
        private Dictionary<string, string> _worldLimitsPickingMethods;

        public void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _worldLimitsPickingMethods = ReflectionData.PickingMethods;
            _mapShapes = ReflectionData.MapShapes;

            AssetsMap settings = (AssetsMap)target;

            // Header Display
            EditorGUILayout.LabelField("Display", EditorStyles.boldLabel);

            // Shape
            int popupIndexMapShapes = 0;
            for (int i = 0; i < _mapShapes.Count; i++)
            {
                if (_mapShapes.ElementAt(i).Key == settings.ShapeAsString)
                {
                    popupIndexMapShapes = i;
                    break;
                }
            }
            settings.ShapePopupIndex = EditorGUILayout.Popup("Shape", popupIndexMapShapes, _mapShapes.Select(x => x.Value).ToArray());
            settings.ShapeAsString = _mapShapes.ElementAt(settings.ShapePopupIndex).Key;

            // Seed
            settings.Seed = Mathf.Clamp(EditorGUILayout.IntField("Seed", settings.Seed), int.MinValue, int.MaxValue);

            // Tile's padding
            settings.Padding = EditorGUILayout.Slider("Tile's padding", settings.Padding, 0, 1);

            // -- World's limits settings
            _showWorldLimitsSettings = EditorGUILayout.Foldout(_showWorldLimitsSettings, "World's Limits Settings");
            if (_showWorldLimitsSettings)
            {
                EditorGUI.indentLevel++;

                // Size
                settings.WorldLimitsSize = Mathf.Max(0, EditorGUILayout.IntField("Size", settings.WorldLimitsSize));

                // Prefabs
                _showWorldLimitsPrefabs = EditorGUILayout.Foldout(_showWorldLimitsPrefabs, "Prefabs");
                if (_showWorldLimitsPrefabs)
                {
                    EditorGUI.indentLevel++;

                    int popupIndex = 0;
                    for (int i = 0; i < _worldLimitsPickingMethods.Count; i++)
                    {
                        if (_worldLimitsPickingMethods.ElementAt(i).Key == settings.WorldLimitsPrefabPickingMethodAsString)
                        {
                            popupIndex = i;
                            break;
                        }
                    }
                    settings.WorldLimitsPrefabPickingMethodPopupIndex = EditorGUILayout.Popup("Pick", popupIndex, _worldLimitsPickingMethods.Select(x => x.Value).ToArray());
                    settings.WorldLimitsPrefabPickingMethodAsString = _worldLimitsPickingMethods.ElementAt(settings.WorldLimitsPrefabPickingMethodPopupIndex).Key;

                    // Rotation
                    settings.WorldLimitsPrefabOrientationMethod = (OrientationMethod) EditorGUILayout.EnumPopup("Rotate", settings.WorldLimitsPrefabOrientationMethod);

                    // Prefab
                    SerializedProperty pptyWorldLimitsPrefabs = serializedObject.FindProperty("WorldLimitsPrefabs");
                    EditorGUILayout.PropertyField(pptyWorldLimitsPrefabs, new GUIContent("Prefabs"), true);

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Header Biomes
            EditorGUILayout.LabelField("Biomes", EditorStyles.boldLabel);

            // Biomes' settings (see BiomeSettingsDrawer)
            SerializedProperty pptyBiomeSettings = serializedObject.FindProperty("BiomeSettings");
            EditorGUILayout.PropertyField(pptyBiomeSettings, true);

            serializedObject.ApplyModifiedProperties();
        }
       
    }
}