using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    public Sprite NavMenuClosedImage;
    public Sprite NavMenuOpenedImage;
    public Sprite InventoryOpenedImage;
    public Sprite InventoryClosedImage;
    public Sprite RemarksOpenedImage;
    public Sprite RemarksClosedImage;

    public Sprite DoneOnImage;
    public Sprite DoneOffImage;

    public Image NavMenuImg;
    public Image InventoryImg;
    public Image RemarksImg;
    public Image DoneImg;

    bool isInvOpen = false;
    bool isNavOpen = false;
    public bool isRemarksOpen = true;

    public Transform ScrollerViewTr;
    public GameObject NavMenu;
    public NavMenuItem MenuItemPF;
    public GameObject InvMenu;

    public Action<int> StepPressedCallback;

    public Text CountText;
    public ScrollRect ScrollRectRef;
    public RemarksManager RemarksManagerRef;

    public int CompletedStep;
    private int _currnetStep;

    public void Init(Move[] moves) {

        

        for (int i = 0; i < moves.Length; i++) {
            NavMenuItem item = Instantiate<NavMenuItem>(MenuItemPF);
            item.transform.SetParent(ScrollerViewTr.transform);
            item.UpdateContent(i + 1);
            item.gameObject.SetActive(true);

            int t = i;
            item.GetComponent<Button>().onClick.AddListener(delegate { StepClicked(t); });
        }

        ScrollRectRef.verticalNormalizedPosition = 1;
        CompletedStep = -1;
        _currnetStep = -1;

        RemarksManagerRef.ShowRemarks(isRemarksOpen);
        SetSkin();
    }

    private void StepClicked(int stepNum) {
        StepPressedCallback(stepNum);
    }

    public void NavMenuToggled() {
        isNavOpen = !isNavOpen;
        isInvOpen = false;

        NavMenu.SetActive(isNavOpen);
        InvMenu.SetActive(isInvOpen);

        SetSkin();
    }

    public void InvMenuToggled() {
        isInvOpen = !isInvOpen;
        isNavOpen = false;
        
        NavMenu.SetActive(isNavOpen);
        InvMenu.SetActive(isInvOpen);

        SetSkin();
    }

    public void RemarksToggled() {
        isRemarksOpen = !isRemarksOpen;
        RemarksManagerRef.ShowRemarks(isRemarksOpen);
        SetSkin();
    }

    public void DoneToggled() {
        if (_currnetStep <= CompletedStep) {
            CompletedStep = _currnetStep - 1;
        } else {
            CompletedStep = _currnetStep;
        }
        UpdateToStepNum(_currnetStep);
    }


    public void UpdateToStepNum(int step) {
        DoneImg.gameObject.SetActive(step > -1);
        DoneImg.sprite = (step <= CompletedStep) ? DoneOnImage : DoneOffImage;

        _currnetStep = step;
    }

    private void SetSkin() {

        NavMenuImg.sprite = (isNavOpen) ? NavMenuOpenedImage : NavMenuClosedImage;
        InventoryImg.sprite = (isInvOpen) ? InventoryOpenedImage : InventoryClosedImage;
        RemarksImg.sprite = (isRemarksOpen) ? RemarksOpenedImage : RemarksClosedImage;
    }

}
