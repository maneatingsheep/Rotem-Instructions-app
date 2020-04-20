using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemarksManager : MonoBehaviour {
    public GameObject RemarksCanvas;
    public GameObject LinesCanvas;
    public RemarkObj[] Remarks;

    public Camera ActiveCamera;

    public UILine LinePF;
    private List<UILine> _lines;
    private Remark[] _remarksData;


    public void Init() {

        _lines = new List<UILine>();

        //_lineRend = GetComponent<LineRenderer>();
        //_lineRend.SetPosition(0, ActiveCamera.ScreenToWorldPoint(new Vector3(0, 0, 3)));
        //_lineCanvas
    }

    public void ShowRemarks(bool doShow) {
        RemarksCanvas.gameObject.SetActive(doShow);
        LinesCanvas.gameObject.SetActive(doShow);
        
    }

    internal void SetRemarks(Move move) {

        int lineCount = 0;

        for (int i = 0; i < Remarks.Length; i++) {
            if (move != null && i < move.Remarks.Length) {

                Remarks[i].Activate(move.Remarks[i].Text);
                for (int j = 0; j < move.Remarks[i].TargetTransforms.Length; j++) {
                    if (_lines.Count < lineCount + 1) {
                        _lines.Add(Instantiate<UILine>(LinePF));
                        _lines[lineCount].transform.SetParent(LinesCanvas.transform);
                    }
                    _lines[lineCount].gameObject.SetActive(true);
                    lineCount++;
                }
            } else {
                Remarks[i].Deactivate();

            }

        }

        while (lineCount < _lines.Count) {
            _lines[lineCount++].gameObject.SetActive(false);
        }

        if (move != null) {
            _remarksData = move.Remarks;
        }
    
    }

    void Update() {
        if (_remarksData != null && _remarksData.Length > 0) {


            Vector3[][] targetVertexWorldSpace = new Vector3[_remarksData.Length][];
            Vector3[] remarkAvgWorldSpace = new Vector3[_remarksData.Length];

            for (int i = 0; i < _remarksData.Length; i++) {

                remarkAvgWorldSpace[i] = Vector3.zero;
                targetVertexWorldSpace[i] = new Vector3[_remarksData[i].TargetTransforms.Length];
                for (int j = 0; j < _remarksData[i].TargetTransforms.Length; j++) {

                    targetVertexWorldSpace[i][j] = _remarksData[i].TargetTransforms[j].TransformPoint(_remarksData[i].TargetTransforms[j].GetComponent<MeshFilter>().mesh.vertices[0]);
                    remarkAvgWorldSpace[i] += targetVertexWorldSpace[i][j];
                }

                remarkAvgWorldSpace[i] /= _remarksData[i].TargetTransforms.Length;

                remarkAvgWorldSpace[i].Normalize();

                //DBG.position = remarkAvgWorldSpace[0] * 5f;
            }



            int lineCount = 0;

            for (int i = 0; i < _remarksData.Length; i++) {
                for (int j = 0; j < _remarksData[i].TargetTransforms.Length; j++) {
                    _lines[lineCount].SetPositions(targetVertexWorldSpace[i][j], Remarks[i].transform as RectTransform);
                }
            }

        }
    }
}
