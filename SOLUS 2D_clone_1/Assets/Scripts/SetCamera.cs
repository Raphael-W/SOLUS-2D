using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class SetCamera : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Initialize()
    {
        var vcam = FindObjectOfType<CinemachineVirtualCamera>();

        vcam.Follow = gameObject.transform;
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }
}
