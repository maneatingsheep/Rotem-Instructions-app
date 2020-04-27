using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssSelectionButton : MonoBehaviour
{

    public Action<AssSelectionButton> ButonClicked;
   public void Clicked() {
        ButonClicked(this);
    }
}
