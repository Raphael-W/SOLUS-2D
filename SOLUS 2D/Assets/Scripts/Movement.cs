using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using Unity.Burst.CompilerServices;

public class Movement : NetworkBehaviour
{

    public Rigidbody2D RigidBody;

    private MvInputKey Key;

    private Vector2 oldPosition;
    public float rotationMomentum = 0.8f;
    private float prevRotation = 0f;
    public float mainThrust = 600;
    public float rotationThrust = 150f;
    private float speed;

    private float currentRotation;
    private Vector3Int posBeneath;

    public float fuel = 5000000;
    public float refuelSpeed = 1f;
    public float landPrecision = 15f;
    public float fuelRemaining;

    private GameObject MainUniverse;
    private GameObject ServerManager;
    private GameObject ConnectionUI;

    private Tilemap tilemap;

    private ServerManager ServerScript;
    private Generation generation;


    enum MvInputKey {
        Key_Neutral = 0,
        Key_Left = 1,
        Key_Right = 2,
    };

    public void MapReset(int previous, int current)
    {
        if (IsOwner)
        {
            
            generation.BeginGeneration(current);
        }

        transform.position = new Vector3(250, 200, 0);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            base.OnNetworkSpawn();

            RigidBody = GetComponent<Rigidbody2D>();

            ConnectionUI = GameObject.FindGameObjectWithTag("ConnectionUI");
            ServerManager = GameObject.FindGameObjectWithTag("Server");
            ServerScript = ServerManager.GetComponent<ServerManager>();
            MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
            generation = MainUniverse.GetComponent<Generation>();
            tilemap = MainUniverse.GetComponent<Tilemap>();

            ConnectionUI.SetActive(false);

            ServerScript.seed.OnValueChanged += MapReset;
            MapReset(0, ServerScript.seed.Value); //0 is the "Previous value". It can be anything because although it isnt used, it is requited for the OnValueChanged.

            fuelRemaining = fuel;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        //Debug.Log("Fuel: " + Math.Round((fuelRemaining / fuel) * 100, 0) + "%");

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


    // Update is called once per frame
    void FixedUpdate()
    {

        if (!IsOwner) return;

        ProcessRotation();

        speed = Vector2.Distance(oldPosition, transform.position) * 100f;
        oldPosition = transform.position;
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

        Quaternion desiredRotation = transform.rotation * Quaternion.Euler(Vector3.forward * rotationThisFrame * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 100f);

        RigidBody.freezeRotation = false; //unfreezing rotation so the physics system can take over
    }
}
