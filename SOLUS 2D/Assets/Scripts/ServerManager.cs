using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ServerManager : NetworkBehaviour
{
    private readonly NetworkVariable<int> seed = new();

    private GameObject MapGenerator;
    private Generation generation;

    private bool ClientReady;
    private bool ServerReady;

    private GameObject[] ServerManagers;

    public void Awake()
    {
        ServerManagers = GameObject.FindGameObjectsWithTag("ServerManager");
        foreach (GameObject ServerManagerInstance in  ServerManagers)
        {
            if(ServerManagerInstance != this.gameObject)
            {
                Destroy(ServerManagerInstance);
            }    
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        MapGenerator = GameObject.FindGameObjectWithTag("MainUniverseTag");
        generation = MapGenerator.GetComponent<Generation>();

        ClientReady = false;
        ServerReady = false;

        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
    }

    public void ClientConnected(ulong clientId)
    {
        ClientReady = true;
    }

    public void StartHost(string Address)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(Address, (ushort)6666);
        NetworkManager.Singleton.StartHost();
        ServerReady = true;
    }

    public void StartClient(string Address)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(Address, (ushort)6666);
        NetworkManager.Singleton.StartClient();
        ServerReady = true;
    }

    public void Seed()
    {
        if (IsServer)
        {
            seed.Value = Random.Range(1000000, 9999999);
        }
        generation.BeginGeneration(seed.Value);
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public void Update()
    {
        if (ClientReady && ServerReady)
        {
            Seed();
            ClientReady = false;
            ServerReady = false;
        }
    }
}
