using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class BulletController : MonoBehaviour
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

    void Update()
    {
        if (shoot)
        {
            transform.position += Direction * MoveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
