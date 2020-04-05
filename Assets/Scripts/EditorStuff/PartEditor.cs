using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartEditor : MonoBehaviour
{

    public string PartName;
    public string PrefabName;
    public Assembly[] Assemblies;
    public Quaternion InitialRotation;
    public float InitialCamDist;

    public void Init()
    {

    }

    [Serializable]
    public class Assembly {
        public string Name;
        public string[] StaticParts;
    }

#if UNITY_EDITOR

    internal void CaptureCamera() {
        InitialRotation = GameObject.Find("full part").transform.rotation;
    }



#endif


}
