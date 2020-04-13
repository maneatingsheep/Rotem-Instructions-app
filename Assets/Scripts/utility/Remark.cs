using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Remark 
{
    public string Text;
    public string[] TargetTransformNames;
    internal Transform[] TargetTransforms;

    internal void Init(Transform assemblyPartsRoot) {

        TargetTransforms = new Transform[TargetTransformNames.Length];

        for (int i = 0; i < TargetTransformNames.Length; i++) {
            TargetTransforms[i] = assemblyPartsRoot.Find(TargetTransformNames[i]);

        }
    }
}
