using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{

    public float MoveSpeed;
    public Vector3 Direction;
    public bool shoot;

    private BoxCollider2D BoxCollider;

    private void Awake()
    {
        shoot = false;
        BoxCollider = GetComponent<BoxCollider2D>();
    }

    [ClientRpc]
    public void ReadyToShootClientRpc(Vector3 direction)
    {
        shoot = true;
        Direction = direction;
    }

    void Update()
    {
        if (IsOwner)
        {
            if (shoot)
            {
                transform.position += Direction * MoveSpeed * Time.deltaTime;
            }
        }    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
