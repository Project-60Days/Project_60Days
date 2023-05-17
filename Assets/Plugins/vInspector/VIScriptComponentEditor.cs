#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;
using static VInspector.VInspectorData;



namespace VInspector
{
#if !DISABLED
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
#endif
    class VIScriptComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (scriptMissing) { ScriptMissingWarningGUI(); return; }

            if (!VIResettablePropDrawer.scriptTypesWithVInspector.Contains(target.GetType()))
                VIResettablePropDrawer.scriptTypesWithVInspector.Add(target.GetType());

            var curProperty = serializedObject.GetIterator();
            curProperty.NextVisible(true);

            if (!VIMenuItems.hideScriptField)
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(curProperty);


            var selectedTabPath = "";
            void updateSelectedTabPath()
            {
                var tab = data.rootTab;
                while (tab != null)
                {
                    selectedTabPath += "/" + tab.name;

                    if (tab.subtabs.Any() && (tab.selectedSubtab == null || !tab.subtabs.Contains(tab.selectedSubtab)))
                        tab.selectedSubtab = tab.subtabs.First();

                    tab = tab.selectedSubtab;
                }
                selectedTabPath = selectedTabPath.Trim('/');
            }

            void drawBody()
            {
                var noVariablesShown = true;
                var drawingTabPath = "";
                var drawingFoldoutPath = "";
                var hide = false;
                var disable = false;

                void ensureNeededTabsDrawn()
                {
                    if (!selectedTabPath.StartsWith(drawingTabPath)) return;


                    void drawSubtabs(Tab tab)
                    {
                        if (!tab.subtabs.Any()) return;

                        Space(noVariablesShown ? 2 : 6);
                        var selName = Tabs(tab.selectedSubtab.name, false, 24, tab.subtabs.Select(r => r.name).ToArray());
                        Space(5);



                        if (selName != tab.selectedSubtab.name)
                        {
                            data.RecordUndo();
                            tab.selectedSubtab = tab.subtabs.Find(r => r.name == selName);
                            updateSelectedTabPath();
                        }


                        GUI.backgroundColor = Color.white;

                        tab.subtabsDrawn = true;
                    }

                    var cur = data.rootTab;
                    foreach (var name in drawingTabPath.Split('/').Where(r => r != ""))
                    {
                        if (!cur.subtabsDrawn)
                            drawSubtabs(cur);


                        cur = cur.subtabs.Find(r => r.name == name);
                    }
                }
                void drawCurProperty()
                {
                    if (!((target.GetType().GetField(curProperty.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance) ?? target.GetType().BaseType.GetField(curProperty.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) is FieldInfo field)) return;
                    if (field.FieldType == typeof(VInspectorData)) return;
                    if (field.GetCustomAttribute<ButtonAttribute>() != null) return;


                    void updateIndentLevel(string path)
                    {
                        var prev = EditorGUI.indentLevel;

                        EditorGUI.indentLevel = path.Split('/').Where(r => r != "").Count();

                        if (prev > EditorGUI.indentLevel)
                            Space(6);
                    }

                    void ifs()
                    {
                        var endIfAttribute = field.GetCustomAttribute<EndIfAttribute>();

                        if (endIfAttribute != null) hide = disable = false;


                        var ifAttribute = field.GetCustomAttribute<IfAttribute>();

                        if (ifAttribute is HideIfAttribute) hide = ifAttribute.Evaluate(target);
                        if (ifAttribute is ShowIfAttribute) hide = !ifAttribute.Evaluate(target);
                        if (ifAttribute is DisableIfAttribute) disable = ifAttribute.Evaluate(target);
                        if (ifAttribute is EnableIfAttribute) disable = !ifAttribute.Evaluate(target);

                    }
                    void tabs()
                    {
                        var tabAttribute = field.GetCustomAttribute<TabAttribute>();
                        var endTabAttribute = field.GetCustomAttribute<EndTabAttribute>();
                        if (tabAttribute != null) { drawingTabPath = tabAttribute.name; drawingFoldoutPath = ""; hide = disable = false; }
                        if (endTabAttribute != null) { drawingTabPath = ""; drawingFoldoutPath = ""; hide = disable = false; }


                        ensureNeededTabsDrawn();

                    }
                    void foldouts()
                    {
                        var foldoutAttribute = field.GetCustomAttribute<FoldoutAttribute>();
                        var endFoldoutAttribute = field.GetCustomAttribute<EndFoldoutAttribute>();
                        var newFoldoutPath = drawingFoldoutPath;
                        if (foldoutAttribute != null) newFoldoutPath = foldoutAttribute.name;
                        if (endFoldoutAttribute != null) newFoldoutPath = "";

                        var drawingPathSplit = drawingFoldoutPath.Split('/').Where(r => r != "").ToArray();
                        var newPathSplit = newFoldoutPath.Split('/').Where(r => r != "").ToArray();
                        var sharedLength = 0;
                        for (; sharedLength < newPathSplit.Length && sharedLength < drawingPathSplit.Length; sharedLength++)
                            if (drawingPathSplit[sharedLength] != newPathSplit[sharedLength])
                                break;

                        drawingFoldoutPath = string.Join("/", drawingPathSplit.Take(sharedLength));

                        for (int i = sharedLength; i < newPathSplit.Length; i++)
                        {
                            if (!data.rootFoldout.IsSubfoldoutContentVisible(drawingFoldoutPath)) break;


                            var prevPath = drawingFoldoutPath;
                            drawingFoldoutPath += '/' + newPathSplit[i];
                            drawingFoldoutPath = drawingFoldoutPath.Trim('/');

                            updateIndentLevel(prevPath);
                            var foldout = data.rootFoldout.GetSubfoldout(drawingFoldoutPath);
                            var newExpanded = Foldout(foldout.name, foldout.expanded);
                            if (newExpanded != foldout.expanded)
                            {
                                data.RecordUndo();
                                foldout.expanded = newExpanded;
                            }
                        }
                    }



                    ifs();

                    if (hide) return;

                    GUI.enabled = !disable;



                    tabs();

                    if (!selectedTabPath.StartsWith(drawingTabPath)) return;

                    noVariablesShown = false;



                    foldouts();

                    if (!data.rootFoldout.IsSubfoldoutContentVisible(drawingFoldoutPath)) return;



                    updateIndentLevel(drawingFoldoutPath);

                    EditorGUILayout.PropertyField(curProperty, true);


                }



                serializedObject.UpdateIfRequiredOrScript();

                while (curProperty.NextVisible(false))
                    drawCurProperty();

                if (noVariablesShown)
                    using (new EditorGUI.DisabledScope(true))
                        GUILayout.Label("No variables to show");

                serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel = 0;

            }
            void drawButtons()
            {
                var noButtonsToShow = true;

                foreach (var button in data.buttons)
                {
                    if (button.tab != "" && !selectedTabPath.StartsWith(button.tab)) continue;

                    if (button.ifAttribute is HideIfAttribute && button.ifAttribute.Evaluate(target)) continue;
                    if (button.ifAttribute is ShowIfAttribute && !button.ifAttribute.Evaluate(target)) continue;

                    var prevGuiEnabled = GUI.enabled;
                    if (button.ifAttribute is DisableIfAttribute && button.ifAttribute.Evaluate(target)) GUI.enabled = false;
                    if (button.ifAttribute is EnableIfAttribute && !button.ifAttribute.Evaluate(target)) GUI.enabled = false;


                    GUILayout.Space(button.space - 2);

                    GUI.backgroundColor = button.isPressed() ? pressedButtonCol : Color.white;
                    if (GUILayout.Button(button.name, GUILayout.Height(button.size)))
                    {
                        target.RecordUndo();
                        button.action();
                    }
                    GUI.backgroundColor = Color.white;


                    noButtonsToShow = false;
                    GUI.enabled = prevGuiEnabled;
                }

                if (noButtonsToShow)
                    Space(-17);

            }



            SetupHeader();
            data.rootTab.ResetSubtabsDrawn();
            updateSelectedTabPath();


            Space(4);
            drawBody();

            Space(17);
            drawButtons();

            Space(4);

        }

        VInspectorData data;
        static Dictionary<Object, VInspectorData> datasByTarget = new Dictionary<Object, VInspectorData>();
        IMGUIContainer header;
        System.Action defaultHeaderGUI;
        bool isScriptableObjectEditor => typeof(ScriptableObject).IsAssignableFrom(target.GetType());
        bool scriptMissing;




        void OnHeaderGUIOverride()
        {
            var bgNorm = EditorGUIUtility.isProSkin ? Greyscale(.248f) : Greyscale(.8f);
            var bgHovered = EditorGUIUtility.isProSkin ? Greyscale(.28f) : Greyscale(.84f);
            var name = target.GetType().Name.Decamelcase();
            var nameRect = header.contentRect.MoveX(60).SetWidth(name.LabelWidth());



            void headerClick()
            {
                if (e.mouseDown())
                    mousePressedOnHeader = true;
                if (e.mouseUp())
                    mousePressedOnHeader = false;
            }
            void scriptNameClick()
            {
                if (e.mouseUp())
                    mousePressedOnScriptName = false;

                if (!nameRect.IsHovered()) return;
                if (!e.mouseDown()) return;

                e.Use();

                mousePressedOnScriptName = true;


                var script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);

                if (e.clickCount == 2)
                    AssetDatabase.OpenAsset(script);

                if (holdingAlt)
                    PingObject(script);
            }
            void hideScriptText()
            {
                var rect = header.contentRect.SetWidth(60).MoveX(name.LabelWidth() + 60).SetHeightFromMid(15);
                rect.Draw(header.contentRect.IsHovered() && (!mousePressedOnHeader || mousePressedOnScriptName) ? bgHovered : bgNorm);
            }
            void greyoutScriptName()
            {
                if (!mousePressedOnScriptName) return;

                nameRect.Resize(1).Draw(Greyscale(bgHovered.r, EditorGUIUtility.isProSkin ? .3f : .45f));
            }



            if (e.mouseUp())
                Repaint();

            headerClick();
            scriptNameClick();

            defaultHeaderGUI();

            hideScriptText();
            greyoutScriptName();

        }
        bool mousePressedOnScriptName;
        bool mousePressedOnHeader;



        public void SetupHeader()
        {
            if (header is VisualElement v && v.panel == null)
            {
                header.onGUIHandler = defaultHeaderGUI;
                header = null;
            }

            if (header != null || isScriptableObjectEditor) return;

            var window = typeof(Editor).GetProperty("propertyViewer", maxBindingFlags).GetValue(this) as EditorWindow;
                        
            if (!window) return;

            var editorField = typeof(InspectorElement).GetField("m_Editor", maxBindingFlags);

            void findHeader(VisualElement element)
            {
                if (element == null) return;


                if (element.GetType().Name == "EditorElement")
                {
                    IMGUIContainer curHeader = null;
                    foreach (var child in element.Children())
                    {
                        curHeader = curHeader ?? new[] { child as IMGUIContainer }.FirstOrDefault(r => r.name.EndsWith("Header"));

                        if (curHeader is null) continue;
                        if (!(child is InspectorElement)) continue;

                        if (editorField.GetValue(child).Equals(this))
                        {
                            header = curHeader;
                            return;
                        }

                    }
                }


                foreach (var r in element.Children())
                    if (header == null)
                        findHeader(r);
            }
            void setupGUICallbacks()
            {
                defaultHeaderGUI = header.onGUIHandler;
                header.onGUIHandler = OnHeaderGUIOverride;
            }

            findHeader(window.rootVisualElement);
            setupGUICallbacks();

        }
        public void OnEnable()
        {
            CheckScriptMissing();
            if (scriptMissing) return;

            var serializedDataField = target.GetType().GetFields(maxBindingFlags).FirstOrDefault(r => r.FieldType == typeof(VInspectorData));

            if (datasByTarget.ContainsKey(target) && datasByTarget[target] != null)
                data = datasByTarget[target];
            else
                data = datasByTarget[target] = (VInspectorData)(serializedDataField?.GetValue(target)) ?? ScriptableObject.CreateInstance<VInspectorData>();

            serializedDataField?.SetValue(target, data);

            data.Setup(target);
            data.Dirty();
        }

        void CheckScriptMissing()
        {
            if (target)
                scriptMissing = target.GetType() == typeof(MonoBehaviour) || target.GetType() == typeof(ScriptableObject);
            else
                scriptMissing = target is MonoBehaviour || target is ScriptableObject;
        }
        void ScriptMissingWarningGUI()
        {
            var prev = GUI.enabled;
            GUI.enabled = true;
            var scriptProperty = serializedObject.FindProperty("m_Script");
            if (scriptProperty != null)
            {
                EditorGUILayout.PropertyField(scriptProperty);
                serializedObject.ApplyModifiedProperties();
            }

            Space(4);
            var s = "Script cannot be loaded";
            s += "\nPossible reasons:";
            s += "\n- Compile erros";
            s += "\n- Script is deleted";
            s += "\n- Script file name doesn't match class name";
            s += "\n- Class doesn't inherit from MonoBehaviour";
            EditorGUILayout.HelpBox(s, MessageType.Warning, true);

            Space(4);
            GUI.enabled = prev;
        }




    }
}
#endif
