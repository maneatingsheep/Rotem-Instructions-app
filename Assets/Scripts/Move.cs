
using UnityEngine;


internal class Move {

    public Move[] Submoves;

    public Transform[] Transforms;
    public Transform[] RemarkTransforms;
    public string[] Remarks;
    internal PosRots Initital;
    internal PosRots Final;
    internal Quaternion ViewRot;
    public Vector3 ViewFocusPoint;
    public float ViewCamDistance;

    private string _assembly;
    private string[] _supportingAssemblies;

    public string[] SupportingAssemblies { get => (Submoves != null)?(Submoves[0].SupportingAssemblies) :_supportingAssemblies; set => _supportingAssemblies = value; }
    public string Assembly { get => (Submoves != null) ?(Submoves[0]._assembly) : _assembly; set => _assembly = value; }
}

internal class PosRots {
    public Vector3[] Pos;
    public Quaternion[] Rot;
}


