using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public RectTransform handCanvas;

    public static HandUI instance;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static RectTransform GetRect() => instance.handCanvas;
}
