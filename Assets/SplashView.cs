using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashView : MonoBehaviour
{
    public AssSelectionButton AssemblySelectionButtPF;

    public Transform ButtonsGrid;
    public GameObject AssSelectioncont;

    private List<AssSelectionButton> _buttons;

    int _selectedAssembly = 0;

    public Sprite UnselectedButtSkin;
    public Sprite SelectedButtSkin;

    public Color SelectedTextColor;
    public Color UnselectedTextColor;

    public Action<int> ReadytoGo;

    internal void Init(ElementSet[] assemblies) {
        _buttons = new List<AssSelectionButton>();
        for (int i = 0; i < assemblies.Length; i++) {
            _buttons.Add(Instantiate(AssemblySelectionButtPF));
            _buttons[i].transform.SetParent(ButtonsGrid);
            _buttons[i].transform.GetChild(0).GetComponent<Text>().text = assemblies[i].ElementName;
            _buttons[i].ButonClicked += AssemblySelected;
        }

        _selectedAssembly = 0;
        UpdateButtons();
    }

    public void StartPresssed() {
        ReadytoGo(_selectedAssembly);
    }

    public void OpenAssembltSelection() {
        AssSelectioncont.SetActive(!AssSelectioncont.activeSelf);
    }

    public void AssemblySelected(AssSelectionButton targetButt) {
        _selectedAssembly = _buttons.IndexOf(targetButt);
        UpdateButtons();
    }

    private void UpdateButtons() {
        for (int i = 0; i < _buttons.Count; i++) {
            _buttons[i].GetComponent<Image>().sprite = (i == _selectedAssembly) ? SelectedButtSkin : UnselectedButtSkin;
            _buttons[i].transform.GetChild(0).GetComponent<Text>().color = (i == _selectedAssembly) ? SelectedTextColor : UnselectedTextColor;
        }
    }
}
