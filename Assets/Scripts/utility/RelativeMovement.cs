using System;
using UnityEngine;

[Serializable]
public class RelativeMovement {
    public Vector3 Pos;
    public Vector3 PublicRot;

    internal Quaternion Rot {
        get {
            return Quaternion.Euler(PublicRot);
        }
    }
}