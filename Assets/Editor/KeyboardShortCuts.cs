using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class KeyboardShortCuts : MonoBehaviour
{

    /* 
    To create a hotkey you can use the following special characters: % (ctrl on Windows, cmd on macOS), # (shift), & (alt). 
    If no special modifier key combinations are required the key can be given after an underscore. 
    For example to create a menu with hotkey shift-alt-g use "MyMenu/Do Something #&g". 
    To create a menu with hotkey g and no key modifiers pressed use "MyMenu/Do Something _g".

    Some special keyboard keys are supported as hotkeys, for example "#LEFT" would map to shift-left. 
    The keys supported like this are: LEFT, RIGHT, UP, DOWN, F1 .. F12, HOME, END, PGUP, PGDN.

    https://docs.unity3d.com/ScriptReference/MenuItem.html
    */

    // Snippet to creat a shortcut 
    // [MenuItem("MyMenu/Do Something with a Shortcut Key %g")]
    // static void DoSomethingWithAShortcutKey()
    // {
    //     Debug.Log("Doing something with a Shortcut Key...");
    // }

    [MenuItem("MyMenu/Toggle active &a")]
    static void ToggleActiveState()
    {
        GameObject[] selectedGOs = Selection.gameObjects;

        if ( selectedGOs != null )
            foreach ( GameObject selected in selectedGOs )
                selected.SetActive ( !selected.activeSelf );
    }

    [MenuItem("MyMenu/Generate Lightmap &l")]
    static void GenerateLightMap()
    {
        if ( !Lightmapping.Bake() )
            Debug.LogError("Lightmap baking failed");
    }
}
