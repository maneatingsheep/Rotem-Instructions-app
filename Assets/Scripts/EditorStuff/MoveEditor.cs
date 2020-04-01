using System;
using UnityEditor;
using UnityEngine;

public class MoveEditor : MonoBehaviour
{
    internal Transform[] Transforms;
    internal Transform[] RemarkTransforms;
    
    public string[] TransformNames;
    
    public string[] Remarks;
    public string[] RemarkTransformNames;

    public PosRot RelativeMove;
    public Quaternion ViewRot;

    public string Assembly;
    public string[] SupportingAssemblies;

    public bool DoResetAxis;

    [Serializable]
    public class PosRot {
        public Vector3 Pos;
        public Vector3 PublicRot;

        internal Quaternion Rot {
            get {
                return Quaternion.Euler(PublicRot);
            }
        }
    }


    internal void BuildGeometry(Transform allPartsRoot)
    {

        Transforms = new Transform[TransformNames.Length];


        for (int i = 0; i < TransformNames.Length; i++)
        {
            Transforms[i] = allPartsRoot.Find(TransformNames[i]);
            
            Transforms[i].parent = allPartsRoot.Find(Assembly);
        }
        

        if (DoResetAxis)
        {
            ResetAxis();
        }

        RemarkTransforms = new Transform[Remarks.Length];
        for (int i = 0; i < Remarks.Length; i++)
        {
            RemarkTransforms[i] = allPartsRoot.Find(RemarkTransformNames[i]);
        }
    }

    public void ResetAxis() {

        for (int i = 0; i < Transforms.Length; i++) {
            ResetSingleRotationScale(Transforms[i]);
            ResetSinglePart(Transforms[i], this);
        }
        
    }

    

    static private void ResetSingleRotationScale(Transform t) {
        if (t.childCount > 0) {

            Quaternion parentRot = t.rotation;

            for (int i = 0; i < t.childCount; i++) {
                Transform child = t.GetChild(i);

                if (t.parent != null) {
                    child.localPosition = t.parent.InverseTransformPoint(t.TransformPoint(child.localPosition)) - t.localPosition;
                } else {
                    child.localPosition = t.TransformPoint(child.localPosition) - t.localPosition;
                }

                child.localRotation = t.localRotation * child.localRotation;
                child.localScale *= t.localScale.x;
            }

            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;


            for (int i = 0; i < t.childCount; i++) {
                Transform child = t.GetChild(i);
                ResetSingleRotationScale(child);
            }
        }
    }

    static private bool ResetSinglePart(Transform t, MoveEditor me) {

        Vector3 avg = new Vector3();

        if (t.childCount > 0) {

            int realChildCount = 0;


            Transform[] children = new Transform[t.childCount];

            for (int i = 0; i < t.childCount; i++) {
                children[i] = t.GetChild(i);
            }


            for (int i = 0; i < children.Length; i++) { 

                if (ResetSinglePart(children[i], me)) {

                    avg += children[i].localPosition;
                    realChildCount++;
                }
            }

            avg /= realChildCount;

            

            for (int i = 0; i < t.childCount; i++) {
                Transform child = t.GetChild(i);

                child.localPosition -= avg;
            }

            t.localPosition += avg;

        } else {
            MeshFilter mf = t.gameObject.GetComponent<MeshFilter>();
            
            if (mf == null) {
                return false;
            }
            else
            {

                for (int j = 0; j < mf.sharedMesh.vertices.Length; j++)
                {
                    avg += t.TransformPoint(mf.sharedMesh.vertices[j]);
                }

                avg /= mf.sharedMesh.vertices.Length;


                GameObject container = new GameObject();
                container.transform.parent = t.parent;
                container.transform.position = avg;
                t.parent = container.transform;
                t.gameObject.SetActive(true);
                for (int i = 0; i < me.Transforms.Length; i++)
                {
                    if (t == me.Transforms[i])
                    {
                        me.Transforms[i] = container.transform;
                        break;
                    }
                }
                

            }

            
        }

        return true;
    }

#if UNITY_EDITOR
    internal void CaptureCamera() {
        
        ViewRot = GameObject.Find("full part").transform.rotation;
    }
    
#endif

}
