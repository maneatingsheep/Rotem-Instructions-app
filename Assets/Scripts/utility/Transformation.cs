using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Transformation
{
    public ElementSet Elements;
    public RelativeMovement Movement;

    internal PosRots Initial;
    internal PosRots Final;

    public bool DoResetAxis;


    internal void Init(Transform allPartsRoot, Part part, string assemblyName) {

        Elements.Transforms = new Transform[Elements.TransformNames.Length];


        for (int i = 0; i < Elements.TransformNames.Length; i++) {
            bool isAssembly = false;
            for (int j = 0; j < part.Assemblies.Length; j++) {
                isAssembly |= (part.Assemblies[j].ElementName == Elements.TransformNames[i]);
            }



            Elements.Transforms[i] = allPartsRoot.Find(Elements.TransformNames[i]);

            if (!isAssembly) {
                Elements.Transforms[i].parent = allPartsRoot.Find(assemblyName);
            }

        }


        if (DoResetAxis) {
            ResetAxis();
        }

        Final = new PosRots();
        Initial = new PosRots();

        Final.Pos = new Vector3[Elements.Transforms.Length];
        Final.Rot = new Quaternion[Elements.Transforms.Length];
        Initial.Pos = new Vector3[Elements.Transforms.Length];
        Initial.Rot = new Quaternion[Elements.Transforms.Length];

        for (int j = 0; j < Elements.Transforms.Length; j++) {
            Final.Pos[j] = Elements.Transforms[j].localPosition;
            Final.Rot[j] = Elements.Transforms[j].localRotation;
            Initial.Pos[j] = Final.Pos[j] - Movement.Pos;
            Initial.Rot[j] = Final.Rot[j] * Movement.Rot;
        }

        
    }

    

    public void ResetAxis() {

        for (int i = 0; i < Elements.Transforms.Length; i++) {
            ResetSingleRotationScale(Elements.Transforms[i]);
            ResetSingleElement(Elements.Transforms[i]);
        }

    }



    private void ResetSingleRotationScale(Transform t) {
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

    private bool ResetSingleElement(Transform t) {

        Vector3 avg = new Vector3();

        if (t.childCount > 0) {

            int realChildCount = 0;


            Transform[] children = new Transform[t.childCount];

            for (int i = 0; i < t.childCount; i++) {
                children[i] = t.GetChild(i);
            }


            for (int i = 0; i < children.Length; i++) {

                if (ResetSingleElement(children[i])) {

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
            } else {

                for (int j = 0; j < mf.sharedMesh.vertices.Length; j++) {
                    avg += t.TransformPoint(mf.sharedMesh.vertices[j]);
                }

                avg /= mf.sharedMesh.vertices.Length;


                GameObject container = new GameObject();
                container.transform.parent = t.parent;
                container.transform.position = avg;
                container.transform.localRotation = Quaternion.identity;
                t.parent = container.transform;
                t.gameObject.SetActive(true);
                for (int i = 0; i < Elements.Transforms.Length; i++) {
                    if (t == Elements.Transforms[i]) {
                        Elements.Transforms[i] = container.transform;
                        break;
                    }
                }
            }
        }

        return true;
    }

}
