// #define DISABLED    // comment to enable menu items at Tools/vInspector

#if UNITY_EDITOR && !DISABLED
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;


namespace VInspector
{
    class VIMenuItems
    {
        const string menuDir = "Tools/vInspector/";


        [MenuItem(menuDir + "Script inspector")]
        static void das() => ToggleDefineDisabledInScript(typeof(VIScriptComponentEditor));
        [MenuItem(menuDir + "Script inspector", true)]
        static bool dassadc() { Menu.SetChecked(menuDir + "Script inspector", !ScriptHasDefineDisabled(typeof(VIScriptComponentEditor))); return true; }


        [MenuItem(menuDir + "ScriptableObject inspector")]
        static void adsasddsa() => ToggleDefineDisabledInScript(typeof(VIScriptableObjectEditor));
        [MenuItem(menuDir + "ScriptableObject inspector", true)]
        static bool adsasdsda() { Menu.SetChecked(menuDir + "ScriptableObject inspector", !ScriptHasDefineDisabled(typeof(VIScriptableObjectEditor))); return true; }


        [MenuItem(menuDir + "Static inspector")]
        static void asdsasda() => ToggleDefineDisabledInScript(typeof(VIScriptAssetEditor));
        [MenuItem(menuDir + "Static inspector", true)]
        static bool adsassad() { Menu.SetChecked(menuDir + "Static inspector", !ScriptHasDefineDisabled(typeof(VIScriptAssetEditor))); return true; }


        [MenuItem(menuDir + "Resettable variables")]
        static void adsdsaads() => ToggleDefineDisabledInScript(typeof(VIResettablePropDrawer));
        [MenuItem(menuDir + "Resettable variables", true)]
        static bool sadsadads() { Menu.SetChecked(menuDir + "Resettable variables", !ScriptHasDefineDisabled(typeof(VIResettablePropDrawer))); return true; }
    }
}
#endif