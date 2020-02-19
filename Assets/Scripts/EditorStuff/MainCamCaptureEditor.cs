#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssemblyManager))] 
public class MainCamCaptureEditor : Editor {



    public override void OnInspectorGUI() 
    {

        base.DrawDefaultInspector();

        AssemblyManager am = (AssemblyManager)target;

        if (GUILayout.Button("Cam Capture")) {
            am.CaptureCamera();
        }

        if (GUILayout.Button("Apply Mat")) {
            am.ApplyMat();
        }
    }
}
#endif
