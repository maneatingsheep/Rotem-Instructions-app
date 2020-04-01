using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartEditor : MonoBehaviour
{

    public string PartName;
    public string PrefabName;
    public Assembly[] Assemblies;
    
    public void Init()
    {

    }

    [Serializable]
    public class Assembly {
        public string Name;
        public string[] StaticParts;
    }
}
