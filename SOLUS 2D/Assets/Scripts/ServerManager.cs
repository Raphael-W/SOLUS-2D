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

    public GameObject BulletPrefab;
    private GameObject SpawnedBullet;

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

    [ServerRpc(RequireOwnership = false)]
    public void ShootBulletServerRpc(Vector3 position, Quaternion rotation, Vector3 direction, ulong ClientID)//ServerRpcParams serverRpcParams = default)
    {
        //var clientId = serverRpcParams.Receive.SenderClientId;
        SpawnedBullet = Instantiate(BulletPrefab, position, rotation);
        SpawnedBullet.GetComponent<NetworkObject>().SpawnWithOwnership(ClientID);
        SpawnedBullet.GetComponent<BulletController>().ReadyToShootClientRpc(direction);
        //SpawnedBullet.GetComponent<NetworkObject>().ChangeOwnership(ClientID);
    }
}
