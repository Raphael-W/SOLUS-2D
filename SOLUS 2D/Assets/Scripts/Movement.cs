using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Tilemaps;

public class Movement : NetworkBehaviour
{

    public Rigidbody2D RigidBody;

    private MvInputKey Key;

    [Header("Movement Settings")]

    public float rotationMomentum = 0.8f;
    private float prevRotation = 0f;
    public float mainThrust = 600;
    public float rotationThrust = 150f;

    private float currentRotation;
    private Vector3Int posBeneath;

    [Header("Fuel Settings")]

    public float fuel = 5000000;
    public float refuelSpeed = 1f;
    public float landPrecision = 15f;
    public float fuelRemaining;

    private GameObject MainUniverse;
    private Tilemap tilemap;


    enum MvInputKey {
        Key_Neutral = 0,
        Key_Left = 1,
        Key_Right = 2,
    };

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            RigidBody = GetComponent<Rigidbody2D>();

            MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
            tilemap = MainUniverse.GetComponent<Tilemap>();

            fuelRemaining = fuel;
        }

        transform.position = new Vector3(250, 200, 0);
    }

    void Update()
    {
        if (!IsOwner) return;

        Debug.Log("Fuel: " + Math.Round((fuelRemaining / fuel) * 100, 0) + "%");

        Key = MvInputKey.Key_Neutral;

        if (Input.GetKey(KeyCode.A)) {
            if (fuelRemaining > 0)
            {
                Key |= MvInputKey.Key_Left;
            }

            else
            {
                fuelRemaining = 0;
            }
        }
    
        if (Input.GetKey(KeyCode.D)) {
            if (fuelRemaining > 0)
            {
                Key |= MvInputKey.Key_Right;
            }

            else
            {
                fuelRemaining = 0;
            }
        }

        posBeneath = Vector3Int.FloorToInt(transform.position - new Vector3Int(0, 1, 0));
        currentRotation = transform.rotation.eulerAngles.z;

        if (tilemap.HasTile(posBeneath) && (360-landPrecision <= currentRotation && currentRotation >= landPrecision))
        {
            if (fuelRemaining <= fuel)
            {
                fuelRemaining += refuelSpeed;
            }

            else
            {
                fuelRemaining = fuel;
            }
            
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            ProcessRotation();
        }
    }

    void ProcessRotation()
    {
        if ((Key & MvInputKey.Key_Left) != 0) {
            prevRotation = rotationThrust;
            RotatePlayer(rotationThrust);
            fuelRemaining -= rotationThrust;
            
        }
        if ((Key & MvInputKey.Key_Right) != 0) {
            prevRotation = -rotationThrust;
            RotatePlayer(-rotationThrust);
            fuelRemaining -= rotationThrust;
            

        }
        if (Key != MvInputKey.Key_Neutral) {

            if ((Key & MvInputKey.Key_Left) != 0 && (Key & MvInputKey.Key_Right) != 0) {
                RigidBody.AddRelativeForce(Vector2.up * mainThrust * Time.deltaTime);
                fuelRemaining -= mainThrust;
                
            }

            else {
                RigidBody.AddRelativeForce(Vector2.up * (mainThrust / 2) * Time.deltaTime);
            }
        }
        else {
            if (Mathf.Abs(prevRotation) > 0.1) {
                prevRotation *= rotationMomentum;
                RotatePlayer(prevRotation);
            }
            else {
                prevRotation = 0;
            }

        }
    }

    private void RotatePlayer(float rotationThisFrame)
    {
        RigidBody.freezeRotation = true; //freezing rotation so we can manually rotate

        Quaternion desiredRotation = transform.rotation * Quaternion.Euler(rotationThisFrame * Time.deltaTime * Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 100f);

        RigidBody.freezeRotation = false; //unfreezing rotation so the physics system can take over
    }
}
