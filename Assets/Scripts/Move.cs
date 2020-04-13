using System;
using UnityEditor;
using UnityEngine;

public class Move : MonoBehaviour, EditorControlled {

    public Transformation[] Transformations;
    
    public Remark[] Remarks;

    public Quaternion ViewRot;
    public Vector3 ViewFocusPoint;
    public float ViewCamDistance;

    

    public string AssemblyName;
    public string[] SupportingAssemblies;


    internal void Init(Transform allPartsRoot, Part part) {

        

        for (int e = 0; e < Transformations.Length; e++) {
            Transformations[e].Init(allPartsRoot, part, AssemblyName);
        }

        //remarks must be initiallized after transformations, due to parenting change
        for (int i = 0; i < Remarks.Length; i++) {
            Remarks[i].Init(allPartsRoot.Find(AssemblyName));
        }

    }


    

    

#if UNITY_EDITOR
    void EditorControlled.CapturePartRotation() {
        
        ViewRot = GameObject.Find("full part").transform.rotation;
    }

    void EditorControlled.ApplyPartRotation() {

        GameObject.Find("full part").transform.rotation = ViewRot;
    }

    void EditorControlled.CapturePartFocus() {

        ViewFocusPoint = GameObject.Find("full part").transform.position * -1;
        ViewCamDistance = -GameObject.Find("Edge Detection Camera").transform.position.z;
    }

    void EditorControlled.ApplyPartFocus() {

        GameObject.Find("full part").transform.rotation = ViewRot;
    }

    void EditorControlled.CaptureTransformsbyNames(int index, bool addToExistingList) {
        int boundIndex = Mathf.Clamp(index, 0, Transformations.Length - 1);
        AddTransformsToList(addToExistingList, ref Transformations[boundIndex].Elements.TransformNames);
        
    }

    void EditorControlled.CaptureRemarkTargetsNames(int index, bool addToExistingList) {

        AddTransformsToList(addToExistingList, ref Remarks[index].TargetTransformNames);

    }

    private void AddTransformsToList(bool addToExistingList, ref string[] targetList) {
        GameObject[] objects = Selection.gameObjects;


        if (!addToExistingList) {
            targetList = new string[0];
        }

        string[] prevTransformNames = targetList;

        targetList = new string[prevTransformNames.Length + objects.Length];

        for (int i = 0; i < prevTransformNames.Length; i++) {
            targetList[i] = prevTransformNames[i];
        }

        for (int i = 0; i < objects.Length; i++) {
            targetList[i + prevTransformNames.Length] = objects[i].name;
        }
    }


#endif

}
