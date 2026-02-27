using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    public GameObject windowPrefab;
    public RectTransform playerCanvas;

    public static CanvasUI instance;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static RectTransform GetRect() => instance.playerCanvas;


}
