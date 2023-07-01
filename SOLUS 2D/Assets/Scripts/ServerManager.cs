using UnityEngine;
using Unity.Netcode;

public class ServerManager : NetworkBehaviour
{
    private NetworkVariable<int> seed = new NetworkVariable<int>();

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.StartServer();
        seed.Value = Random.Range(1000000, 9999999); //8024669
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
