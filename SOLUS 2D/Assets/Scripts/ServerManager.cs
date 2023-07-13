using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ServerManager : NetworkBehaviour
{
    private static NetworkVariable<int> seed = new NetworkVariable<int>();

    private GameObject MapGenerator;
    private Generation generation;

    private bool ClientReady;
    private bool ServerReady;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        MapGenerator = GameObject.FindGameObjectWithTag("MainUniverseTag");
        generation = MapGenerator.GetComponent<Generation>();

        ClientReady = false;
        ServerReady = false;

        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        seed.OnValueChanged += ValueChanged;
    }

    public void ValueChanged(int previous, int current)
    {
        Debug.Log("Value Changed");
        seed.OnValueChanged -= ValueChanged;
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
            Debug.Log("Seed value before: " + seed.Value);
            seed.Value = Random.Range(1000000, 9999999);
            Debug.Log("Seed value after: " + seed.Value);
        }

        Debug.Log("Seed Retrieved");
        generation.BeginGeneration(seed.Value);
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
