using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class SetCamera : NetworkBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Initialize()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();

        vcam.Follow = gameObject.transform;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsClient) return;
        Initialize();
    }
}
