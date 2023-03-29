#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;

namespace VInspector
{
    class VIScriptAssetEditor
    {
        static void OnGUI(Editor editor)
        {
            if (editor.GetType() != typeof(Editor).Assembly.GetType("UnityEditor.MonoScriptImporterInspector")) return;

            var script = (editor.target as MonoImporter)?.GetScript();
            if (!script) return;

            var classType = script.GetClass();
            if (classType == null) return;

            var staticVariables = classType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(r => !r.IsLiteral && !r.IsInitOnly && supportedTypes.Any(rr => rr.IsAssignableFrom(r.FieldType))).ToArray();
            var staticFunctions = classType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(r => r.GetParameters().Count() == 0).ToArray();



            void saveDefaultValues()
            {
                if (defaultValuesByClassType.ContainsKey(classType)) return;



                defaultValuesByClassType[classType] = new Dictionary<FieldInfo, object>();
                foreach (var r in staticVariables)
                    defaultValuesByClassType[classType][r] = r.GetValue(null);




            }
            void bg()
            {
                var lineCol = Greyscale(.1f);

                Space(10);
                var lineRect = ExpandWidthLabelRect();
                lineRect = lineRect.SetWidthFromMid(lineRect.width + 123).SetHeight(1);

                lineRect.Draw(lineCol);

                var bgRect = lineRect.MoveY(1).SetHeight(123123);
                bgRect.Draw(backgroundCol);

            }
            void staticVariables_()
            {
                if (!staticVariables.Any()) return;
                if (!(staticVariablesExpanded = Foldout("Static Variables", staticVariablesExpanded))) return;





                foreach (var field in staticVariables)
                {
                    var name = "  " + field.Name.PrettifyVarName(false);


                    if (typeof(Object).IsAssignableFrom(field.FieldType))
                        field.SetValue(null, ResettableField(name, (Object)field.GetValue(null), (Object)defaultValuesByClassType[classType][field]));

                    if (field.FieldType == typeof(float))
                        field.SetValue(null, ResettableField(name, (float)field.GetValue(null), (float)defaultValuesByClassType[classType][field]));

                    if (field.FieldType == typeof(int))
                        field.SetValue(null, ResettableField(name, (int)field.GetValue(null), (int)defaultValuesByClassType[classType][field]));

                    if (field.FieldType == typeof(string))
                        field.SetValue(null, ResettableField(name, (string)field.GetValue(null), (string)defaultValuesByClassType[classType][field]));

                    if (field.FieldType == typeof(bool))
                        field.SetValue(null, ResettableField(name, (bool)field.GetValue(null), (bool)defaultValuesByClassType[classType][field]));
                }

                Space(6);
            }
            void staticFunctions_()
            {
                if (!staticFunctions.Any()) return;
                if (!(staticFunctionsExpanded = Foldout("Static Functions", staticFunctionsExpanded))) return;




                Space(6);
                foreach (var function in staticFunctions)
                {
                    var name = " " + function.Name;//.Decamelcase();

                    Space(-2);
                    GUILayout.BeginHorizontal();
                    Space(16);
                    if (GUILayout.Button(name, GUILayout.Height(28)))
                        function.Invoke(null, null);
                    Space(8);
                    GUILayout.EndHorizontal();
                }

                Space(10);
            }



            saveDefaultValues();
            bg();
            Space(-6);
            EditorGUI.indentLevel = 1;

            staticVariables_();
            staticFunctions_();


            EditorGUI.indentLevel = 0;
        }
        static bool staticVariablesExpanded { get => EditorPrefs.GetBool("VIScriptAssetEditor-staticVariablesExpanded", false); set => EditorPrefs.SetBool("VIScriptAssetEditor-staticVariablesExpanded", value); }
        static bool staticFunctionsExpanded { get => EditorPrefs.GetBool("VIScriptAssetEditor-staticMethodsExpanded", false); set => EditorPrefs.SetBool("VIScriptAssetEditor-staticMethodsExpanded", value); }
        static Dictionary<System.Type, Dictionary<FieldInfo, object>> defaultValuesByClassType = new Dictionary<System.Type, Dictionary<FieldInfo, object>>();
        static System.Type[] supportedTypes = new[] { typeof(Object), typeof(float), typeof(int), typeof(string), typeof(bool) };


#if !DISABLED
        [InitializeOnLoadMethod]
        static void Subscribe() => Editor.finishedDefaultHeaderGUI += OnGUI;
#endif
    }

}
#endif
