using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CustomEditorGUI :Editor{

    private bool _isAddToList = false;
    private int _inputIndex = 0;
    private string _inputIndexStr = "";

    public override void OnInspectorGUI() {

        base.DrawDefaultInspector();

        EditorControlled controlledElement = (EditorControlled)target;

        if (GUILayout.Button("Capture Part View")) {
            controlledElement.CapturePartView();
        }

        if (GUILayout.Button("Apply Part View")) {
            controlledElement.ApplyPartView();
        }

        TransformsCaptureUI(controlledElement, ref _isAddToList, ref _inputIndex, ref _inputIndexStr);



    }

    public static void TransformsCaptureUI(EditorControlled element, ref bool isAddToList, ref int inputIndex, ref string inputIndexStr) {

        isAddToList = GUILayout.Toggle(isAddToList, "Add to existing list");
        if (inputIndexStr == null) {
            inputIndexStr = "";
        }

        GUIStyle gs = new GUIStyle();

        inputIndexStr = GUILayout.TextArea(inputIndexStr, gs);

        inputIndex = 0;

        try {
            inputIndex = int.Parse(inputIndexStr);
        } catch (Exception e) {
            //Debug.Log(e.Message);
        }

        if (GUILayout.Button("Capture Transforms By Names")) {

            element.CaptureTransformsbyNames(inputIndex, isAddToList);
        }
        if (GUILayout.Button("Capture Remark Transforms By Names")) {

            element.CaptureRemarkTargetsNames(inputIndex, isAddToList);
        }
    }

}
