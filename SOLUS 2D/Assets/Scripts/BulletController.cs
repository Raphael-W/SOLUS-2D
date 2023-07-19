using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletController : NetworkBehaviour
{

    public float MoveSpeed;
    private Vector3 Direction;
    private bool shoot;
    private float rocketSpeed;
    private bool Explosive;

    private GameObject Universe;
    private Tilemap UniverseTiles;

    private Vector3Int[] explosionPositions;
    public int ExplosionRadius;
    private int Index;

    private void Awake()
    {
        shoot = false;
        Universe = GameObject.FindGameObjectWithTag("MainUniverseTag");
        UniverseTiles = Universe.GetComponent<Tilemap>();
    }

    [ClientRpc]
    public void ReadyToShootClientRpc(Vector3 direction, float playerSpeed, bool explosive)
    {
        shoot = true;
        Direction = direction;
        rocketSpeed = playerSpeed;
        Explosive = explosive;
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
        if (Explosive)
        {
            explosionPositions = GetPositionsInRadius(Vector3Int.FloorToInt(transform.position), ExplosionRadius);
            foreach (Vector3Int position in explosionPositions)
            {
                UniverseTiles.SetTile(position, null);
            }
        }

        Debug.Log(collision.gameObject.layer == LayerMask.NameToLayer("TileMap"));
        GetComponent<NetworkObject>().Despawn();
    }
}
