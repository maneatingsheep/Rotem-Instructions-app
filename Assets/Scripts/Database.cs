using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public Transform AllPartsRoot;
    public Transform CurrentPartRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal void Init() {

        
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

        //while (AllPartsRoot.childCount > 0)
        for (int i = 0; i < AllPartsRoot.childCount; i++)
        {
            Destroy(AllPartsRoot.GetChild(i).gameObject);
        }

        CurrentPartRoot = transform.GetChild(currentPart);

        string[] staticParts = CurrentPartRoot.GetComponent<PartEditor>().StaticParts;

        for (int i = 0; i < staticParts.Length; i++)
        {
            Transform pf = Resources.Load<Transform>(staticParts[i]);

            Transform partRoot = Instantiate<Transform>(pf);
            //Resources.UnloadAsset(pf);


            partRoot.name = staticParts[i];

            partRoot.SetParent(AllPartsRoot);
        }
        


        

        Move[] moves = new Move[CurrentPartRoot.childCount];

        for (int i = 0; i < CurrentPartRoot.childCount; i++)
        {

            Transform child = CurrentPartRoot.GetChild(i);


            if (child.childCount > 0)
            {

                moves[i] = new Move() { Submoves = new Move[child.childCount] };



                for (int j = 0; j < child.childCount; j++)
                {
                    MoveEditor me = child.GetChild(j).GetComponent<MoveEditor>();
                    moves[i].Submoves[j] = BuildMove(me);
                }
            }
            else
            {

                moves[i] = BuildMove(child.GetComponent<MoveEditor>());
            }
        }

        return moves;
    }

    

    private Move BuildMove(MoveEditor me) {

        me.BuildGeometry(AllPartsRoot);


        Move m = new Move() { Transforms = me.Transforms };

        m.CameraPos = me.CameraPos;
        m.CameraRot = me.CameraRot;

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
