﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsManager : MonoBehaviour
{
    public InventoryItem ItemPF;

    public RectTransform SideInventory;
    public RectTransform FullInventory;

    private List<InventoryItem> Items;

    public void Clear() {
        if (Items == null) {
            Items = new List<InventoryItem>();
        }

        for (int i = 0; i < Items.Count; i++) {
            Items[i].gameObject.SetActive(false);
        }
    }

    internal void UpdateInventory(Move move) {
        Clear();

        UpdateSingleMove(move);
    }

    internal void UpdateInventory(Move[] moves) {
        Clear();
        for (int i = 0; i < moves.Length; i++) {
            UpdateSingleMove(moves[i]);
        }
    }

    private void UpdateSingleMove(Move move) {
        for (int i = 0; i < move.Transformations.Length; i++) {
            if (Items.Count < i + 1) {
                Items.Add(Instantiate<InventoryItem>(ItemPF));
                Items[i].transform.SetParent(SideInventory, false);
            }
            Items[i].gameObject.SetActive(true);


            Items[i].UpdateContent(move.Transformations[i].Elements);
        }
    }
}
