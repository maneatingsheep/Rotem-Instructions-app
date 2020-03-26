using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemarksManager : MonoBehaviour {
    public Button ShowRemarksButt;
    public Button HideRemarksButt;
    public GameObject RemarksCanvas;
    public GameObject[] Remarks;

    public Camera ActiveCamera;

    private LineRenderer _lineRend;
    private Transform[] _remarksTransforms;

    public void Init() {
        _lineRend = GetComponent<LineRenderer>();
    }

    public void OpenHideRemarks(bool doShow) {
        ShowRemarksButt.gameObject.SetActive(!doShow);
        HideRemarksButt.gameObject.SetActive(doShow);
        RemarksCanvas.gameObject.SetActive(doShow);
    }

    internal void SetRemarks(Move move) {

        ShowRemarksButt.gameObject.SetActive(move != null && move.Remarks.Length > 0);
        HideRemarksButt.gameObject.SetActive(move != null && move.Remarks.Length > 0);
        for (int i = 0; i < Remarks.Length; i++) {
            if (move != null && i < move.Remarks.Length) {

                Remarks[i].SetActive(true);
                Remarks[i].GetComponentInChildren<Text>().text = move.Remarks[i];
            } else {
                Remarks[i].SetActive(false);

            }

        }
        if (move != null) {

            _remarksTransforms = move.RemarkTransforms;
        }
    
    }

    void Update() {
        if (_remarksTransforms != null && _remarksTransforms.Length > 0) {

            _lineRend.SetPositions(new Vector3[2] { ActiveCamera.ScreenToWorldPoint(new Vector3()) , _remarksTransforms[0].position });
        }
    }
}
