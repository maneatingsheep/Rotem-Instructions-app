
using UnityEngine;


internal class Move {

    public Move[] Submoves;

    public Transform[] Transforms;
    public Transform[] RemarkTransforms;
    public string[] Remarks;
    internal PosRots Initital;
    internal PosRots Final;
    internal Vector3 CameraPos;
    internal Quaternion CameraRot;
}

internal class PosRots {
    public Vector3[] Pos;
    public Quaternion[] Rot;
}


