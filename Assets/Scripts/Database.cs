using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal Move[] Init() {

        Move[] moves = new Move[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) {
            
            Transform child = transform.GetChild(i);

            if (child.childCount > 0) {
                
                moves[i] = new Move() { Submoves = new Move[child.childCount] };

                for (int j = 0; j < child.childCount; j++) {
                    moves[i].Submoves[j] = ConstructMove(child.GetChild(j).GetComponent<MoveEditor>());
                }
            } else {

                moves[i] = ConstructMove(child.GetComponent<MoveEditor>());
            }
        }

        return moves;
    }

    private Move ConstructMove(MoveEditor me) {
        
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
