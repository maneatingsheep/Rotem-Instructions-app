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

        if (GUILayout.Button("Capture Part Rotation")) {
            Move.CapturePartRotation();
        }

        if (GUILayout.Button("Apply Part Rotation")) {
            Move.ApplyPartRotation();
        }

        if (GUILayout.Button("Capture Focus Point")) {
            Move.CapturePartFocus();
        }

        if (GUILayout.Button("Apply Focus Point")) {
            Move.ApplyPartFocus();
        }

        if (GUILayout.Button("Capture Transforms By Names")) {
            Move.CaptureTransformsbyNames();
        }


    }
}

#endif
