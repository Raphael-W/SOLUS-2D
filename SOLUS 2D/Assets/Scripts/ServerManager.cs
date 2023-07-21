using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ServerManager : NetworkBehaviour
{
    private readonly NetworkVariable<int> seed = new();

    private GameObject MapGenerator;
    private Generation generation;
    private int MapSize;

    private GameObject[] Players;

    private bool ClientReady;
    private bool ServerReady;

    public GameObject BulletPrefab;
    private GameObject SpawnedBullet;

    public Vector3Int[] DestroyedTiles;
    private int DestroyedTileIndex;

    public void Awake()
    {
        DestroyedTileIndex = 0;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        MapGenerator = GameObject.FindGameObjectWithTag("MainUniverseTag");
        generation = MapGenerator.GetComponent<Generation>();
        MapSize = generation.MapSize;

        DestroyedTiles = new Vector3Int[MapSize * MapSize];

        ClientReady = false;
        ServerReady = false;

        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
    }

    public void ClientConnected(ulong clientId)
    {
        ClientReady = true;

        for (int TilePosIndex = 0; TilePosIndex < DestroyedTileIndex; TilePosIndex++)
        {
            generation.ClearTileClientRpc(DestroyedTiles[TilePosIndex], false);
        }
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
    public void ShootBulletServerRpc(Vector3 position, Quaternion rotation, Vector3 direction, float playerSpeed, ulong ClientID)
    {
        SpawnedBullet = Instantiate(BulletPrefab, position, rotation);
        SpawnedBullet.GetComponent<NetworkObject>().SpawnWithOwnership(ClientID);
        SpawnedBullet.GetComponent<BulletController>().ReadyToShootClientRpc(direction, playerSpeed);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnBulletServerRpc(ulong ObjectID)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[ObjectID].gameObject.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClearTileServerRpc(Vector3Int Position, bool first)
    {
        generation.ClearTileClientRpc(Position, first);
        DestroyedTiles[DestroyedTileIndex] = Position;
        DestroyedTileIndex++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void HitPlayerServerRpc(ulong ClientID)
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach(var player in Players)
        {
            player.GetComponent<Movement>().PlayerHitClientRpc(ClientID);
        }
    }
}
