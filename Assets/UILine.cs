using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILine : MonoBehaviour
{

    public LineRenderer Renderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetPositions(Vector3 targetVertex, RectTransform rectTransform) {

        Vector3 toCamVec = targetVertex - Camera.main.transform.position;

        toCamVec.Normalize();

        Renderer.SetPosition(0, Camera.main.transform.position + toCamVec * 5f);

        Rect screenPos = RectTransformToScreenSpace(rectTransform);

        Renderer.SetPosition(1, Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 5)) + new Vector3(0.1f, 0.1f, 0));
        
    }


    public static Rect RectTransformToScreenSpace(RectTransform transform) {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }
}
