using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Text CountFld;
    public Text NameFld;

    internal void UpdateContent(ElementSet elements) {
        CountFld.text = "X " + elements.Transforms.Length;
        NameFld.text = elements.ElementName;
    }
}
