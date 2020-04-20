using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Part : MonoBehaviour, EditorControlled {

    public string PartName;
    public string PrefabName;
    public ElementSet[] Assemblies;
    public Quaternion InitialRotation;
    public float InitialCamDist;


    public void Init() {

    }


#if UNITY_EDITOR

    void EditorControlled.CapturePartView() {

        InitialRotation = GameObject.Find("full part").transform.rotation;
    }

    void EditorControlled.ApplyPartView() {

        GameObject.Find("full part").transform.rotation = InitialRotation;
    }


    void EditorControlled.CaptureTransformsbyNames(int index, bool addToExistingList) {
        int boundIndex = Mathf.Clamp(index, 0, Assemblies.Length - 1);
        AddTransformsToList(addToExistingList, ref Assemblies[boundIndex].TransformNames);

    }

    void EditorControlled.CaptureRemarkTargetsNames(int index, bool addToExistingList) {

        Debug.Log("Unavalible for assemblies");

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
