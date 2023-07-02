using Cinemachine;
using Unity.Netcode;

public class SetCamera : NetworkBehaviour
{
    private CinemachineVirtualCamera VirtualCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        base.OnNetworkSpawn();

        VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        VirtualCamera.Follow = gameObject.transform;
    }
}
