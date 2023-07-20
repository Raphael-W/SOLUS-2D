using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletController : NetworkBehaviour
{

    public float MoveSpeed;
    private Vector3 Direction;
    private bool shoot;
    private float rocketSpeed;

    private GameObject Universe;
    private Tilemap UniverseTiles;
    private Generation GeneratorScript;
    private int MapSize;

    private GameObject Server;
    private ServerManager ServerScript;

    private Vector3Int[] explosionPositions;
    public int ExplosionRadius;

    private bool Border;
    private bool hit;

    private void Awake()
    {
        shoot = false;
        hit = false;

        Universe = GameObject.FindGameObjectWithTag("MainUniverseTag");
        UniverseTiles = Universe.GetComponent<Tilemap>();
        GeneratorScript = Universe.GetComponent<Generation>();

        Server = GameObject.FindGameObjectWithTag("ServerManager");
        ServerScript = Server.GetComponent<ServerManager>();

        MapSize = GeneratorScript.MapSize;
        
    }

    [ClientRpc]
    public void ReadyToShootClientRpc(Vector3 direction, float playerSpeed)
    {
        shoot = true;
        Direction = direction;
        rocketSpeed = playerSpeed;
    }

    public static Vector3Int[] GetPositionsInRadius(Vector3Int center, int radius)
    {
        int diameter = radius * 2 + 1;

        int index = 0;
        Vector3Int[] positions = new Vector3Int[diameter * diameter];

        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                Vector3Int position = new Vector3Int(x, y, center.z);
                if (Vector3Int.Distance(center, position) <= radius)
                {
                    positions[index] = position;
                    index++;
                }
            }
        }

        System.Array.Resize(ref positions, index);

        return positions;
    }

    void Update()
    {
        if (IsOwner)
        {
            if (shoot)
            {
                transform.position += Direction * (MoveSpeed + rocketSpeed) * Time.deltaTime;
            }
        }    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner)
        {
            ServerScript.DespawnBulletServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
        }    
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("TileMap") && !hit)
        {
            explosionPositions = GetPositionsInRadius(Vector3Int.FloorToInt(transform.position), ExplosionRadius);
            foreach (Vector3Int position in explosionPositions)
            {
                Border = ((position.x == MapSize - 1) || (position.x == 0) || (position.y == MapSize - 1) || (position.y == 0));
                if (!Border)
                {
                    FindObjectOfType<AudioManager>().Play("Explode");
                    ServerScript.ClearTileServerRpc(position);
                }
            }
        }

        hit = true;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Players"))
        {
            Debug.Log(collision.gameObject.GetComponent<Movement>().Bullets);
        }
    }
}
