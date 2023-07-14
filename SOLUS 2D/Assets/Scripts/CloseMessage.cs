using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CloseMessage : MonoBehaviour
{
    public void Close()
    {
        Destroy(transform.parent.gameObject);
    }
}
