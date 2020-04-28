using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryItem ItemPF;

    public RectTransform SideInventory;
    public RectTransform FullInventory;

    private List<InventoryItem> ItemsFull;
    private List<InventoryItem> ItemsMove;

    public void Clear(bool isFull) {
        if (isFull) {
            if (ItemsFull == null) {
                ItemsFull = new List<InventoryItem>();
            }

            for (int i = 0; i < ItemsFull.Count; i++) {
                ItemsFull[i].gameObject.SetActive(false);
            }
        } else {
            if (ItemsMove == null) {
                ItemsMove = new List<InventoryItem>();
            }

            for (int i = 0; i < ItemsMove.Count; i++) {
                ItemsMove[i].gameObject.SetActive(false);
            }
        }
       

        
    }

    internal void UpdateInventory(Move move) {
        Clear(false);
        ElementSet[] elements = new ElementSet[move.Transformations.Length];
        for (int i = 0; i < move.Transformations.Length; i++) {
            elements[i] = move.Transformations[i].Elements;
        }
        UpdateSingleMove(elements, false);
    }

    internal void UpdateInventory(Move[] moves, int minMove, int maxMove) {
        Clear(true);

        List<ElementSet> elements = new List<ElementSet>();



        for (int i = 0; i < moves.Length; i++) {
            if (i >=  minMove && i <= maxMove) {
                for (int j = 0; j < moves[i].Transformations.Length; j++) {
                    elements.Add(moves[i].Transformations[j].Elements);
                }
            }
        }

        elements.Sort((a, b) => string.Compare(a.ElementName, b.ElementName));

        int k = 0;
        while (k < elements.Count - 1) {
            if (elements[k].ElementName == elements[k + 1].ElementName) {
                ElementSet es = new ElementSet() { ElementName = elements[k].ElementName, Transforms = new Transform[elements[k].Transforms.Length + elements[k + 1].Transforms.Length] };
                elements[k] = es;
                elements.RemoveAt(k + 1);
            } else {
                k++;
            }
        }

        UpdateSingleMove(elements.ToArray(), true);
    }

    private void UpdateSingleMove(ElementSet[] elementsList, bool isFull) {
        List<InventoryItem> iList = (isFull) ? ItemsFull : ItemsMove;

        for (int i = 0; i < elementsList.Length; i++) {
            if (iList.Count < i + 1) {
                iList.Add(Instantiate<InventoryItem>(ItemPF));
                iList[i].transform.SetParent((isFull) ? FullInventory : SideInventory, false);
            }
            iList[i].gameObject.SetActive(true);


            iList[i].UpdateContent(elementsList[i]);
        }
    }
}
