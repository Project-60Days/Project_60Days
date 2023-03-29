#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;


namespace VInspector
{
#if !DISABLED
    [CustomPropertyDrawer(typeof(Object), true)]
    [CustomPropertyDrawer(typeof(int), true)]
    [CustomPropertyDrawer(typeof(float), true)]
    [CustomPropertyDrawer(typeof(string), true)]
    [CustomPropertyDrawer(typeof(bool), true)]
    [CustomPropertyDrawer(typeof(RangeResettable))]
#endif
    class VIResettablePropDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            bool isResetted = true;
            bool actAsPrefabModifiation;
            var isObjectField = typeof(Object).IsAssignableFrom(fieldInfo.FieldType);

            void buttonInteraction()
            {
                if (!GUI.enabled) return;

                var parent = GetParent(prop);

                if (!scriptTypesWithVInspector.Contains(parent.GetType())) return;

                if (parent.GetType() != fieldInfo.DeclaringType) return;


                if (!parentsWithDefaultValues.ContainsKey(parent.GetType()))
                {
                    if (parent is MonoBehaviour || parent is ScriptableObject)
                        parentsWithDefaultValues.Add(parent.GetType(), ScriptableObject.CreateInstance(parent.GetType()));
                    else if (parent.GetType().GetConstructor(System.Type.EmptyTypes) != null)
                        parentsWithDefaultValues.Add(parent.GetType(), System.Activator.CreateInstance(parent.GetType()));
                    else
                        return;
                }

                var valCur = fieldInfo.GetValue(parent);
                var valDefault = fieldInfo.GetValue(parentsWithDefaultValues[parent.GetType()]);


                actAsPrefabModifiation = prop.isInstantiatedPrefab && !holdingAlt;

                if (actAsPrefabModifiation)
                    isResetted = !prop.prefabOverride;
                else
                {
                    isResetted = object.Equals(valCur, valDefault);

                    if (typeof(Object).IsAssignableFrom(fieldInfo.FieldType))
                        isResetted |= (valDefault == null) && !(bool)(Object)valCur;

                    if (fieldInfo.FieldType == typeof(string))
                        isResetted |= fieldInfo.FieldType == typeof(string) && (object.Equals(valCur, "") && object.Equals(valDefault, null));
                }



                if (_ResetFieldButton(rect, isObjectField))
                {
                    prop.serializedObject.targetObject.RecordUndo();

                    PrefabUtility.IsPartOfPrefabInstance(prop.serializedObject.targetObject);

                    if (actAsPrefabModifiation)
                        PrefabUtility.RevertPropertyOverride(prop, InteractionMode.AutomatedAction);
                    else
                    {
                        if (parent.GetType().IsValueType)
                            fieldInfo.SetValueDirect(__makeref(parent), valDefault);
                        else
                            fieldInfo.SetValue(parent, valDefault);
                    }

                    GUI.FocusControl(null);

                    prop.serializedObject.targetObject.Dirty();
                    prop.serializedObject.ApplyModifiedProperties();
                }

                if ((e.keyDown() || e.keyUp()) && holdingAlt)
                    prop.serializedObject.targetObject.Dirty();
            }
            void defaultPropDrawer()
            {
                EditorGUI.BeginProperty(rect, label, prop);

                if (attribute is RangeResettable)
                {
                    var range = attribute as RangeResettable;

                    if (prop.propertyType == SerializedPropertyType.Float)
                        EditorGUI.Slider(rect, prop, range.min, range.max, label);
                    else if (prop.propertyType == SerializedPropertyType.Integer)
                        EditorGUI.IntSlider(rect, prop, (int)range.min, (int)range.max, label);
                }
                else
                {
                    EditorGUI.PropertyField(rect, prop, label);
                }



                EditorGUI.EndProperty();
            }
            void buttonIcon()
            {
                if (!GUI.enabled) return;

                if (isResetted) return;

                _DrawResettableFieldCrossIcon(rect, isObjectField);
            }

            var isArrayElement = prop.propertyPath.Split('.').Last().StartsWith("data[");


            buttonInteraction();
            defaultPropDrawer();
            buttonIcon();
        }

        public static Dictionary<System.Type, object> parentsWithDefaultValues = new Dictionary<System.Type, object>();
        public static List<System.Type> scriptTypesWithVInspector = new List<System.Type>();


        public object GetParent(SerializedProperty prop)
        {
            if (prop.serializedObject.targetObject.GetType() == (fieldInfo as MemberInfo).ReflectedType)
                return prop.serializedObject.targetObject;


            var path = prop.propertyPath;
            object cur = prop.serializedObject.targetObject;
            object prev = null;
            int i = 0;
            while (cur != null && HasInChildren(prop.propertyPath, ref i, out var token))
            {
                prev = cur;
                cur = MoveToNext(cur, token);
            }
            return prev;
        }

        bool HasInChildren(string propertyPath, ref int index, out PropertyPathComponent component)
        {
            component = new PropertyPathComponent();

            if (index >= propertyPath.Length)
                return false;

            var arrayElementMatch = arrayElementRegex.Match(propertyPath, index);
            if (arrayElementMatch.Success)
            {
                index += arrayElementMatch.Length + 1;
                component.elementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
                return true;
            }

            int dot = propertyPath.IndexOf('.', index);
            if (dot == -1)
            {
                component.propertyName = propertyPath.Substring(index);
                index = propertyPath.Length;
            }
            else
            {
                component.propertyName = propertyPath.Substring(index, dot - index);
                index = dot + 1;
            }

            return true;
        }
        object MoveToNext(object container, PropertyPathComponent component)
        {
            if (component.propertyName == null)
                return ((IList)container)[component.elementIndex];
            else
                return GetMemberValue(container, component.propertyName);
            // return null;
        }
        static object GetMemberValue(object container, string name)
        {
            if (container == null)
                return null;
            var type = container.GetType();
            var members = type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < members.Length; ++i)
            {
                if (members[i] is FieldInfo field)
                    return field.GetValue(container);
                else if (members[i] is PropertyInfo property)
                    return property.GetValue(container);
            }
            return null;
        }
        static Regex arrayElementRegex = new Regex(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);

        struct PropertyPathComponent
        {
            public string propertyName;
            public int elementIndex;
        }

    }
}
#endif


