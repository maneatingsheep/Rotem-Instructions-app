using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNIYY_EDITOR
[CustomEditor(typeof(EdgeDetect))] //1
public class EdgeDetectEditor : Editor
{
    
    public override void OnInspectorGUI() {

        base.DrawDefaultInspector();

        if (GUILayout.Button("adjust camera")) {
            EdgeDetect ed = (EdgeDetect)target;
            Transform t = ed.transform;
            t.position = EditorWindow.GetWindow<SceneView>().camera.transform.position;
            t.rotation = EditorWindow.GetWindow<SceneView>().camera.transform.rotation;
        }
    }
}
#endif
