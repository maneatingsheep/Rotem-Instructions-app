using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public Transform AllPartsRoot;
    public Part PartData;
    private Material _partMaterial;

    internal Move[] Moves;

    void Start()
    {
        
    }

    internal void Init(Material partMaterial) {
        _partMaterial = partMaterial;

    }


    internal void BuildMoves(int currentPart)
    {
        //clear previous parts
        for (int i = 0; i < AllPartsRoot.childCount; i++)
        {
            Destroy(AllPartsRoot.GetChild(i).gameObject);
        }

        //create empty assemblies
        for (int i = 0; i < PartData.Assemblies.Length; i++) {
            GameObject assemblyRoot = new GameObject(PartData.Assemblies[i].ElementName);
            assemblyRoot.transform.parent = AllPartsRoot;
            assemblyRoot.transform.localRotation = Quaternion.identity;
        }


        
        Transform pf = Resources.Load<Transform>(PartData.PrefabName);

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
        for (int i = 0; i < PartData.Assemblies.Length; i++) {
            
            Transform ar = AllPartsRoot.Find(PartData.Assemblies[i].ElementName);

            PartData.Assemblies[i].Transforms = new Transform[PartData.Assemblies[i].TransformNames.Length];

            for (int j = 0; j < PartData.Assemblies[i].TransformNames.Length; j++) {
                Transform staticPartTransform = AllPartsRoot.Find(PartData.Assemblies[i].TransformNames[j]);
                staticPartTransform.parent = ar;
                PartData.Assemblies[i].Transforms[j] = staticPartTransform;
            }
        }

        Moves = new Move[PartData.transform.childCount];

        for (int i = 0; i < PartData.transform.childCount; i++)
        {
            Moves[i] = PartData.transform.GetChild(i).GetComponent<Move>();
            Moves[i].Init(AllPartsRoot, PartData);


        }

    }

    internal void ApplyMat(Material partMat) {

        for (int i = 0; i < Moves.Length; i++) {
            for (int j = 0; j < Moves[j].Transformations.Length; j++) {
                for (int k = 0; k < Moves[j].Transformations[j].Elements.Transforms.Length; k++) {
                    MeshRenderer r = Moves[j].Transformations[j].Elements.Transforms[k].GetComponent<MeshRenderer>();
                    if (r != null) {
                        r.sharedMaterial = partMat;
                    }
                }
            }
        }

       
    }

}
