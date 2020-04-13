using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemarksManager : MonoBehaviour {
    public Button ShowRemarksButt;
    public Button HideRemarksButt;
    public GameObject RemarksCanvas;
    public RemarkObj[] Remarks;

    public Camera ActiveCamera;

    public UILine LinePF;
    private List<UILine> _lines;
    private Color[,] _linePixels;
    private Remark[] _remarksData;

    public void Init() {

        _lines = new List<UILine>();

        //_lineRend = GetComponent<LineRenderer>();
        //_lineRend.SetPosition(0, ActiveCamera.ScreenToWorldPoint(new Vector3(0, 0, 3)));
        //_lineCanvas
    }

    public void OpenHideRemarks(bool doShow) {
        ShowRemarksButt.gameObject.SetActive(!doShow);
        HideRemarksButt.gameObject.SetActive(doShow);
        RemarksCanvas.gameObject.SetActive(doShow);
    }

    internal void SetRemarks(Move move) {

        ShowRemarksButt.gameObject.SetActive(move != null && move.Remarks.Length > 0);
        HideRemarksButt.gameObject.SetActive(move != null && move.Remarks.Length > 0);

        int lineCount = 0;

        for (int i = 0; i < Remarks.Length; i++) {
            if (move != null && i < move.Remarks.Length) {

                Remarks[i].Activate(move.Remarks[i].Text);
                for (int j = 0; j < move.Remarks[i].TargetTransforms.Length; j++) {
                    if (_lines.Count < lineCount + 1) {
                        _lines.Add(Instantiate<UILine>(LinePF));
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

            int lineCount = 0;

            for (int i = 0; i < _remarksData.Length; i++) {
                for (int j = 0; j < _remarksData[i].TargetTransforms.Length; j++) {

                    Vector3 pos = _remarksData[i].TargetTransforms[j].TransformPoint(_remarksData[i].TargetTransforms[j].GetComponent<MeshFilter>().mesh.vertices[0]);




                    _lines[lineCount].SetPositions(pos, Remarks[i].transform as RectTransform);
                }
            }
            //_lineRend.SetPosition(1, _remarksData[0].TargetTransforms[0].position);
            
        }
    }
}
