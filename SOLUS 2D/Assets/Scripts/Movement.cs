using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;

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

    [Header("Fuel Settings")]

    public float fuel = 5000000;
    public float refuelSpeed;
    public float landPrecision = 15f;
    public float fuelRemaining;

    private GameObject FuelPercentage;
    private TMP_Text FuelPercentageText;

    private GameObject UIHandler;
    private UIHandler UIHandlerScript;

    private GameObject ServerManager;
    private ServerManager ServerManagerScript;

    private LayerMask tileLayer;
    private Vector2 rayOrigin;
    private Vector2 rayDirection;
    private Vector2 landOffset;
    private RaycastHit2D landCheckL;
    private RaycastHit2D landCheckR;

    public Color[] PlayerColours;


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

            FuelPercentage = GameObject.FindGameObjectWithTag("FuelPercentage");
            FuelPercentageText = FuelPercentage.GetComponent<TMP_Text>();

            UIHandler = GameObject.FindGameObjectWithTag("UIHandler");
            UIHandlerScript = UIHandler.GetComponent<UIHandler>();

            ServerManager = GameObject.FindGameObjectWithTag("ServerManager");
            ServerManagerScript = ServerManager.GetComponent<ServerManager>();

            tileLayer = LayerMask.GetMask("TileMap");
            UIHandlerScript.GameUI.enabled = true;

            fuelRemaining = fuel;
        }

        if (!IsOwner)
        {
            GetComponent<SpriteRenderer>().color = PlayerColours[OwnerClientId];
        }

        transform.position = new Vector3(250, 200, 0);
    }

    public void SetColour()
    {

    }

    void Update()
    {
        if (!IsOwner) return;

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

        FuelPercentageText.text = (Math.Round((fuelRemaining / fuel) * 100, 0) + "%");
        currentRotation = transform.rotation.z * 180;

        rayOrigin = transform.position;
        rayDirection = Vector2.down;
        landOffset = new Vector2(0.2f, 0);

        landCheckL = Physics2D.Raycast(rayOrigin - landOffset, rayDirection, 1.5f, tileLayer);
        landCheckR = Physics2D.Raycast(rayOrigin + landOffset, rayDirection, 1.5f, tileLayer);

        Debug.DrawRay(rayOrigin - landOffset, rayDirection, Color.green);
        Debug.DrawRay(rayOrigin + landOffset, rayDirection, Color.green);

        if ((-landPrecision <= currentRotation && currentRotation <= landPrecision) && (landCheckL.collider != null || landCheckR.collider != null))
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
