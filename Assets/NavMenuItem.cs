using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavMenuItem : MonoBehaviour
{
    public Text CountFld;

    public void UpdateContent(int index) {
        CountFld.text = index.ToString();
    }
}
