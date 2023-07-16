using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMode : MonoBehaviour
{
    public Canvas ArrowUI;

    public void Start()
    {
        ArrowUI.worldCamera = Camera.main;
    }
}
