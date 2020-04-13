using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Text CountFld;
    public Text NameFld;

    internal void UpdateContent(Transformation transformation) {
        CountFld.text = "X " + transformation.Elements.Transforms.Length;
        NameFld.text = transformation.Elements.ElementName;
    }
}
