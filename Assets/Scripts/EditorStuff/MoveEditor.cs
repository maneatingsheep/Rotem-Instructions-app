using System;
using UnityEditor;
using UnityEngine;

public class MoveEditor : MonoBehaviour
{
    internal Transform[] Transforms;
    internal Transform[] RemarkTransforms;
    
    public string RootTransform;
    public string[] TransformNames;
    
    public string[] Remarks;
    public string[] RemarkTransformNames;

    public PosRot RelativeMove;
    public Vector3 CameraPos;
    public Quaternion CameraRot;

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
         
        
        Transform pf = Resources.Load<Transform>(RootTransform);

        Transform partRoot = Instantiate<Transform>(pf);
        //Resources.UnloadAsset(pf);


        partRoot.name = RootTransform;

        partRoot.SetParent(allPartsRoot);
        
       

        Transforms = new Transform[TransformNames.Length];
        if (TransformNames.Length == 1)
        {
            if (partRoot.name == TransformNames[0])
            {
                Transforms[0] = partRoot;
            }
            else
            {
                Transforms[0] = partRoot.Find(TransformNames[0]);
            }

        }
        else
        {
            for (int i = 0; i < TransformNames.Length; i++)
            {
                Transforms[i] = partRoot.Find(TransformNames[i]);
            }
        }

        if (DoResetAxis)
        {
            ResetAxis();
        }

        RemarkTransforms = new Transform[Remarks.Length];
        for (int i = 0; i < Remarks.Length; i++)
        {
            RemarkTransforms[i] = partRoot.Find(RemarkTransformNames[i]);
        }
    }

    public void ResetAxis() {

        Transform localRoot = Transforms[0];
        while (localRoot.parent != Transforms[0].root) {
            localRoot = localRoot.parent;
        }

        ResetSingleRotationScale(localRoot);
        ResetSinglePart(localRoot, this);
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
        //Vector3[] adjusted;



        if (t.childCount > 0) {

            int realChildCount = 0;

            //adjusted = new Vector3[t.childCount];

            Transform[] children = new Transform[t.childCount];

            for (int i = 0; i < t.childCount; i++) {
                children[i] = t.GetChild(i);
            }


            for (int i = 0; i < children.Length; i++) { 

                //ignore imported lights and cameras
                if (ResetSinglePart(children[i], me)) {

                    /*if (t.parent != null) {
                        adjusted[i] = t.parent.InverseTransformPoint(t.TransformPoint(child.localPosition)) - t.localPosition;
                    } else {
                        adjusted[i] = t.TransformPoint(child.localPosition) - t.localPosition;
                    }



                    avg += adjusted[i];*/

                    avg += children[i].localPosition;
                    realChildCount++;
                }
            }

            avg /= realChildCount;

            /*for (int j = 0; j < adjusted.Length; j++) {
                adjusted[j] -= avg;
            }*/

            for (int i = 0; i < t.childCount; i++) {
                Transform child = t.GetChild(i);

                //ignore imported lights and cameras
                /*if (child.gameObject.GetComponent<MeshFilter>() != null) {
                    child.localPosition = adjusted[i];
                }*/
                child.localPosition -= avg;
            }

            t.localPosition += avg;

        } else {
            MeshFilter mesh = t.gameObject.GetComponent<MeshFilter>();
            
            if (mesh == null) {
                return false;
            }
            else
            {
                //adjusted = new Vector3[mesh.sharedMesh.vertices.Length];

                for (int j = 0; j < mesh.sharedMesh.vertices.Length; j++)
                {
                    //adjusted[j] = t.parent.InverseTransformPoint(t.TransformPoint(mesh.sharedMesh.vertices[j]));
                    //avg += adjusted[j];
                    avg += t.parent.InverseTransformPoint(t.TransformPoint(mesh.sharedMesh.vertices[j]));
                }

                //avg /= adjusted.Length;
                avg /= mesh.sharedMesh.vertices.Length;

                /*for (int j = 0; j < adjusted.Length; j++)
                {
                    //adjusted[j] = t.InverseTransformPoint(t.parent.TransformPoint(adjusted[j]));
                    adjusted[j] -= avg;
                }


                mesh.sharedMesh.vertices = adjusted;
                mesh.sharedMesh.RecalculateBounds();
                mesh.sharedMesh.RecalculateNormals();
                mesh.sharedMesh.RecalculateTangents();

                t.position = avg;
                t.rotation = Quaternion.identity;
                t.localScale = Vector3.one;*/

                GameObject container = new GameObject();
                container.transform.SetParent(t.parent);
                container.transform.position = avg;
                t.SetParent(container.transform);
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
        CameraPos = EditorWindow.GetWindow<SceneView>().camera.transform.position;
        CameraRot = EditorWindow.GetWindow<SceneView>().camera.transform.rotation;
    }
#endif

}
