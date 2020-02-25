#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveEditor))] 
public class MoveCustomEditor : Editor {


    public override void OnInspectorGUI() {

        base.DrawDefaultInspector();

        MoveEditor Move = (MoveEditor)target;

        if (GUILayout.Button("Nullify")) {
            Move.ResetAxis();
        }

        if (GUILayout.Button("Cam Capture")){
            Move.CaptureCamera();
        }
    }
}

#endif
