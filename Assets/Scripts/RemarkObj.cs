using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemarkObj : MonoBehaviour
{
    public Text RemarkTextFld;

    internal void Deactivate() {
        gameObject.SetActive(false);
    }

    internal void Activate(string remarkTxt) {
        gameObject.SetActive(true);
        RemarkTextFld.text = remarkTxt;
    }
}
