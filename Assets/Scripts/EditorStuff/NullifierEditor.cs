#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveEditor))] 
public class NullifyEditor : Editor {


    public override void OnInspectorGUI() {

        base.DrawDefaultInspector();

        MoveEditor Move = (MoveEditor)target;

        if (GUILayout.Button("Nullify")) {
            Move.Nullify();
        }

        if (GUILayout.Button("Cam Capture")){
            Move.CaptureCamera();
        }
    }
}

#endif
