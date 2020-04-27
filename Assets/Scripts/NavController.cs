﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavController : MonoBehaviour {
    public enum ButtonType { StartPLaying, StopPLaying, OneBack, OneForward, FullBack };
    internal enum UIState { Home, First, Middle, Last };

    public Action<ButtonType> ButtonPressedCallback;
    

    public Button StartPlaying;
    public Button StopPlaying;
    public Button OneBack;
    public Button OneForward;
    
    
    public Button HomeButt;
    public Button LogoutButt;

    public Text MoveNumFld;

    

    

    public void ButtonPressed(Button sender) {
        if (sender == StartPlaying) {
            ButtonPressedCallback(ButtonType.StartPLaying);
        } else if (sender == StopPlaying) {
            ButtonPressedCallback(ButtonType.StopPLaying);
        } else if (sender == OneBack) {
            ButtonPressedCallback(ButtonType.OneBack);
        } else if (sender == OneForward) {
            ButtonPressedCallback(ButtonType.OneForward);
        } else if (sender == HomeButt) {
            ButtonPressedCallback(ButtonType.FullBack);
        }
    }

    internal void SetUIState(UIState state, bool InPlay, int currnetStep, int totalSteps) {
        /*StartPlaying.interactable = (state != UIState.Master);
        StartPlaying.gameObject.SetActive(!InPlay);
        StopPlaying.gameObject.SetActive(InPlay);*/
        OneBack.interactable = (state != UIState.First && state != UIState.Home);
        OneForward.interactable = (state != UIState.Last);
        HomeButt.gameObject.SetActive(state != UIState.Home);
        LogoutButt.gameObject.SetActive(state == UIState.Home);

        if (state == UIState.Home) {
            MoveNumFld.text = "HOME";
        } else {
            MoveNumFld.text = string.Format("{0}/{1}", currnetStep + 1, totalSteps);
        }
    }
}
