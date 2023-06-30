using UnityEngine;
using Unity.Netcode;

public class JoinClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.StartClient();
    }
}
