using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour {
    public enum ButtonType { StartPLaying, StopPLaying, OneBack, OneForward, FullBack };
    internal enum UIState { Master, First, Middle, Last };

    public Action<ButtonType> ButtonPressedCallback;
    public Action<int> ChapterPressedCallback;

    public Button StartPlaying;
    public Button StopPlaying;
    public Button OneBack;
    public Button OneForward;
    public Button FullBack;

    public GameObject MainMenu;
    public GameObject MenuItemPF;

    public void Init(string[] chapters)
    {
        for (int i = 0; i < chapters.Length; i++)
        {
            GameObject item = Instantiate(MenuItemPF);
            item.transform.SetParent(MainMenu.transform);
            item.GetComponentInChildren<Text>().text = chapters[i];
            item.SetActive(true);

            int t = i;
            item.GetComponent<Button>().onClick.AddListener(delegate { ChapterClicked(t); });
        }

    }

    private void ChapterClicked(int chapterNum)
    {
        ChapterPressedCallback(chapterNum);
    }

    public void ButtonPressed(Button sender) {
        if (sender == StartPlaying) {
            ButtonPressedCallback(ButtonType.StartPLaying);
        } else if (sender == StopPlaying) {
            ButtonPressedCallback(ButtonType.StopPLaying);
        } else if (sender == OneBack) {
            ButtonPressedCallback(ButtonType.OneBack);
        } else if (sender == OneForward) {
            ButtonPressedCallback(ButtonType.OneForward);
        } else if (sender == FullBack) {
            ButtonPressedCallback(ButtonType.FullBack);
        }
    }

    internal void SetUIState(UIState state, bool InPlay) {
        StartPlaying.interactable = (state != UIState.Master);
        StartPlaying.gameObject.SetActive(!InPlay);
        StopPlaying.gameObject.SetActive(InPlay);
        OneBack.interactable = (state != UIState.First && state != UIState.Master);
        OneForward.interactable = (state != UIState.Last);
        FullBack.interactable = (state != UIState.Master);
    }
}
