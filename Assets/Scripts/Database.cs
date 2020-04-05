using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public Transform AllPartsRoot;
    public PartEditor Part;
    private Material _partMaterial;
    void Start()
    {
        
    }

    internal void Init(Material partMaterial) {
        _partMaterial = partMaterial;


    }


    internal string[] GetChapters()
    {
        string[] result = new string[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            result[i] = transform.GetChild(i).GetComponent<PartEditor>().PartName;
        }

        return result;
    }

    internal Move[] BuildMoves(int currentPart)
    {
        //clear previous parts
        for (int i = 0; i < AllPartsRoot.childCount; i++)
        {
            Destroy(AllPartsRoot.GetChild(i).gameObject);
        }

        //create empty assemblies
        for (int i = 0; i < Part.Assemblies.Length; i++) {
            GameObject assemblyRoot = new GameObject(Part.Assemblies[i].Name);
            assemblyRoot.transform.parent = AllPartsRoot;
            assemblyRoot.transform.localRotation = Quaternion.identity;
        }


        
        Transform pf = Resources.Load<Transform>(Part.PrefabName);

        Transform partRoot = Instantiate<Transform>(pf);

        for (int i = partRoot.childCount - 1; i >= 0 ; i--) {
            Transform child = partRoot.GetChild(i);
            child.GetComponent<MeshRenderer>().material = _partMaterial;
            Quaternion rot = child.localRotation;
            child.parent = AllPartsRoot;
            child.localRotation = rot;
            child.gameObject.SetActive(false);
        }
        Destroy(partRoot.gameObject);

        //add static parts to assemblys
        for (int i = 0; i < Part.Assemblies.Length; i++) {
            Transform ar = AllPartsRoot.Find(Part.Assemblies[i].Name);
            for (int j = 0; j < Part.Assemblies[i].StaticParts.Length; j++) {
                AllPartsRoot.Find(Part.Assemblies[i].StaticParts[j]).parent = ar;
            }
        }



        Move[] moves = new Move[Part.transform.childCount];

        for (int i = 0; i < Part.transform.childCount; i++)
        {

            Transform child = Part.transform.GetChild(i);


            if (child.childCount > 0)
            {


                //build submoves
                moves[i] = new Move() { Submoves = new Move[child.childCount] };

                for (int j = 0; j < child.childCount; j++)
                {
                    MoveEditor me = child.GetChild(j).GetComponent<MoveEditor>();
                    moves[i].Submoves[j] = BuildMove(me);
                    
                    
                }

            }
            else
            {
                //build single move/submove
                moves[i] = BuildMove(child.GetComponent<MoveEditor>());
            }
        }

        

        return moves;
    }

    

    private Move BuildMove(MoveEditor me) {

        me.BuildGeometry(AllPartsRoot, Part);


        Move m = new Move() { Transforms = me.Transforms };

        m.ViewRot = me.ViewRot;
        m.ViewFocusPoint = me.ViewFocus;
        m.ViewCamDistance = me.ViewCamDistance;


        m.Final = new PosRots();
        m.Initital = new PosRots();

        m.Final.Pos = new Vector3[m.Transforms.Length];
        m.Final.Rot = new Quaternion[m.Transforms.Length];
        m.Initital.Pos = new Vector3[m.Transforms.Length];
        m.Initital.Rot = new Quaternion[m.Transforms.Length];

        for (int j = 0; j < m.Transforms.Length; j++) {
            m.Final.Pos[j] = m.Transforms[j].localPosition;
            m.Final.Rot[j] = m.Transforms[j].localRotation;
            m.Initital.Pos[j] = m.Final.Pos[j] - me.RelativeMove.Pos;
            m.Initital.Rot[j] = m.Final.Rot[j] * me.RelativeMove.Rot;
        }

        m.RemarkTransforms = me.RemarkTransforms;
        m.Remarks = me.Remarks;
        m.Assembly = me.Assembly;
        m.SupportingAssemblies = me.SupportingAssemblies;

        return m;
        
    }

    internal void ApplyMat(Material partMat) {
        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.childCount > 0) {
                for (int j = 0; j < child.childCount; j++) {
                    foreach (Transform t in child.GetChild(j).GetComponent<MoveEditor>().Transforms) {
                        ApplyMatToTransform(t, partMat);
                    }
                }
            } else {
                foreach (Transform t in child.GetComponent<MoveEditor>().Transforms) {
                    ApplyMatToTransform(t, partMat);
                }
            }

        }
    }


    private void ApplyMatToTransform(Transform t, Material partMat) {
        if (t.childCount > 0) {
            for (int i = 0; i < t.childCount; i++) {
                ApplyMatToTransform(t.GetChild(i), partMat);
            }
        } else {
            MeshRenderer r = t.GetComponent<MeshRenderer>();
            if (r != null) {
                r.sharedMaterial = partMat;
            }
        }
    }
}
