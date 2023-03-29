using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;
#endif



namespace VInspector
{
    public class VInspectorData : ScriptableObject
    {
#if UNITY_EDITOR
        public void Setup(Object target)
        {
            void buttons()
            {
                this.buttons = new List<Button>();

                var members = target.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Union(target.GetType().BaseType.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)).OrderBy(r => r is MethodInfo);

                foreach (var member in members)
                {
                    var buttonAttribute = member.GetCustomAttribute<ButtonAttribute>();

                    if (buttonAttribute == null)
                        continue;

                    var button = new Button();


                    if (member.GetCustomAttribute<ButtonSizeAttribute>() is ButtonSizeAttribute sizeAttribute)
                        button.size = sizeAttribute.size;

                    if (member.GetCustomAttribute<ButtonSpaceAttribute>() is ButtonSpaceAttribute spaceAttribute)
                        button.space = spaceAttribute.space;

                    if (member.GetCustomAttribute<TabAttribute>() is TabAttribute tabAttribute)
                        button.tab = tabAttribute.name;

                    if (member.GetCustomAttribute<IfAttribute>() is IfAttribute ifAttribute)
                        button.ifAttribute = ifAttribute;


                    if (member is FieldInfo field && field.FieldType == typeof(bool))
                    {
                        var o = field.IsStatic ? null : target;

                        button.isPressed = () => (bool)field.GetValue(o);
                        button.action = () => field.SetValue(o, !(bool)field.GetValue(o));
                        button.name = buttonAttribute.name != "" ? buttonAttribute.name : field.Name.PrettifyVarName(false);
                    }

                    if (member is MethodInfo method && !method.GetParameters().Any())
                    {
                        var o = method.IsStatic ? null : target;

                        button.isPressed = () => false;
                        button.action = () => method.Invoke(o, null);
                        button.name = buttonAttribute.name != "" ? buttonAttribute.name : method.Name.PrettifyVarName(false);
                    }


                    if (button.action != null)
                        this.buttons.Add(button);
                }
            }
            void tabs()
            {
                var attributes = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Union(target.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)).Select(r => r.GetCustomAttribute<TabAttribute>()).OfType<TabAttribute>();

                void setupTab(Tab tab, IEnumerable<string> allSubtabPaths)
                {
                    var names = allSubtabPaths.Select(r => r.Split('/').First()).ToList();

                    foreach (var name in names)
                        if (tab.subtabs.Find(r => r.name == name) == null)
                            tab.subtabs.Add(new Tab(name));

                    foreach (var subtab in tab.subtabs.ToList())
                        if (names.Find(r => r == subtab.name) == null)
                            tab.subtabs.Remove(subtab);

                    tab.subtabs = tab.subtabs.OrderBy(r => names.IndexOf(r.name)).ToList();

                    foreach (var subtab in tab.subtabs)
                        setupTab(subtab, allSubtabPaths.Where(r => r.StartsWith(subtab.name + "/")).Select(r => r.Remove(subtab.name + "/")).ToList());
                }

                setupTab(rootTab, attributes.Select(r => r.name));
            }
            void foldouts()
            {
                var attributes = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Union(target.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)).Select(r => r.GetCustomAttribute<FoldoutAttribute>()).OfType<FoldoutAttribute>();

                void setupFoldout(Foldout foldout, IEnumerable<string> allSubtabPaths)
                {
                    var names = allSubtabPaths.Select(r => r.Split('/').First()).ToList();

                    foreach (var name in names)
                        if (foldout.subfoldouts.Find(r => r.name == name) == null)
                            foldout.subfoldouts.Add(new Foldout(name));

                    foreach (var subtab in foldout.subfoldouts.ToList())
                        if (names.Find(r => r == subtab.name) == null)
                            foldout.subfoldouts.Remove(subtab);

                    foldout.subfoldouts = foldout.subfoldouts.OrderBy(r => names.IndexOf(r.name)).ToList();

                    foreach (var subtab in foldout.subfoldouts)
                        setupFoldout(subtab, allSubtabPaths.Where(r => r.StartsWith(subtab.name + "/")).Select(r => r.Remove(subtab.name + "/")).ToList());
                }

                setupFoldout(rootFoldout, attributes.Select(r => r.name));
            }

            buttons();
            tabs();
            foldouts();
        }

        public List<Button> buttons = new List<Button>();
        public Tab rootTab = new Tab("");
        public Foldout rootFoldout = new Foldout(true);





        [System.Serializable]
        public class Button
        {
            public string name;
            public string tab = "";
            public float size = 30;
            public float space = 0;
            public IfAttribute ifAttribute;
            public System.Action action;
            public System.Func<bool> isPressed;
        }

        [System.Serializable]
        public class Tab
        {
            public string name;
            public bool subtabsDrawn;
            [SerializeReference] public List<Tab> subtabs = new List<Tab>();
            [SerializeReference] public Tab selectedSubtab;

            public Tab(string name) => this.name = name;


            public void ResetSubtabsDrawn()
            {
                subtabsDrawn = false;
                foreach (var r in subtabs)
                    r.ResetSubtabsDrawn();
            }
        }

        [System.Serializable]
        public class Foldout
        {
            public string name;
            public bool expanded;
            [SerializeReference] public List<Foldout> subfoldouts = new List<Foldout>();

            public Foldout(string name) => this.name = name;
            public Foldout(bool expanded) => this.expanded = expanded;

            public Foldout GetSubfoldout(string path)
            {
                if (path == "")
                    return this;
                else if (!path.Contains('/'))
                    return subfoldouts.Find(r => r.name == path);
                else
                    return subfoldouts.Find(r => r.name == path.Split('/').First()).GetSubfoldout(path.Substring(path.IndexOf('/') + 1));
            }
            public bool IsSubfoldoutContentVisible(string path)
            {
                if (path == "")
                    return expanded;
                else if (!path.Contains('/'))
                    return expanded && subfoldouts.Find(r => r.name == path).expanded;
                else
                    return expanded && subfoldouts.Find(r => r.name == path.Split('/').First()).IsSubfoldoutContentVisible(path.Substring(path.IndexOf('/') + 1));
            }
        }
#endif
    }
}
