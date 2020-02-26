using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemarksManager : MonoBehaviour {
    public Button ShowRemarksButt;
    public Button HideRemarksButt;
    public GameObject RemarksCanvas;
    public GameObject[] Remarks;

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
    }
}
