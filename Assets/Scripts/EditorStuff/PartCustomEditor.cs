#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PartEditor))] 
public class PartCustomEditor : Editor {



    public override void OnInspectorGUI() 
    {

        base.DrawDefaultInspector();

        PartEditor pe = (PartEditor)target;

        if (GUILayout.Button("Capture Camera")) {
            pe.CaptureCamera();
        }
    }
}
#endif
