using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelButton))]
public class LevelButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelButton levelButton = (LevelButton)target;

        if(GUILayout.Button("Instantiate(Level Button)"))
        {
            levelButton.spawnButton();
        }
    }
}
