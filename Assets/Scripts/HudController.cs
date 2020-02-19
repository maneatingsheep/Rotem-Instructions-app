using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour {
    public enum ButtonType { PlayOne, PlayAll, OneBack, OneForward, FullBack };
    public enum UIState { First, Middle, Last };

    public Action<ButtonType> ButtonPressedCallback;

    public Button PlayOne;
    public Button PlayAll;
    public Button OneBack;
    public Button OneForward;
    public Button FullBack;

    public void ButtonPressed(Button sender) {
        if (sender == PlayOne) {
            ButtonPressedCallback(ButtonType.PlayOne);
        } else if (sender == PlayAll) {
            ButtonPressedCallback(ButtonType.PlayAll);
        } else if (sender == OneBack) {
            ButtonPressedCallback(ButtonType.OneBack);
        } else if (sender == OneForward) {
            ButtonPressedCallback(ButtonType.OneForward);
        } else if (sender == FullBack) {
            ButtonPressedCallback(ButtonType.FullBack);
        }
    }

    public void SetUIState(UIState state) {
        PlayOne.interactable = (state != UIState.First);
        PlayAll.interactable = (state != UIState.Last);
        OneBack.interactable = (state != UIState.First);
        OneForward.interactable = (state != UIState.Last);
        FullBack.interactable = (state != UIState.First);
    }
}
