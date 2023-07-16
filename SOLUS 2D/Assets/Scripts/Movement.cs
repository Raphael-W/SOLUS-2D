using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;

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

    private LayerMask tileLayer;
    private Vector2 rayOrigin;
    private Vector2 rayDirection;
    private Vector2 landOffset;
    private RaycastHit2D landCheckL;
    private RaycastHit2D landCheckR;

    public Color[] PlayerColours;

    private GameObject ArrowUI;
    public GameObject PlayerArrowPrefab;
    private RectTransform ArrowRectTransform;
    private GameObject InstantiatedPlayerPrefab;
    public float BorderSize;

    private Vector3 PointerWorldPosition;
    private Vector3 CappedTargetScreenPosition;
    private Vector3 targetPositionScreenPoint;

    private Camera UICamera;

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

            tileLayer = LayerMask.GetMask("TileMap");
            UIHandlerScript.GameUI.enabled = true;

            fuelRemaining = fuel;
        }

        if (!IsOwner)
        {
            GetComponent<SpriteRenderer>().color = PlayerColours[OwnerClientId];

            ArrowUI = transform.Find("ArrowUI").gameObject;

            InstantiatedPlayerPrefab = Instantiate(PlayerArrowPrefab, ArrowUI.transform);
            InstantiatedPlayerPrefab.GetComponent<Image>().color = PlayerColours[OwnerClientId];

            ArrowRectTransform = InstantiatedPlayerPrefab.GetComponent<RectTransform>();

            UICamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        transform.position = new Vector3(250, 200, 0);
    }

    void Update()
    {
        if (IsOwner)
        {
            Key = MvInputKey.Key_Neutral;

            if (Input.GetKey(KeyCode.A))
            {
                if (fuelRemaining > 0)
                {
                    Key |= MvInputKey.Key_Left;
                }

                else
                {
                    fuelRemaining = 0;
                }
            }

            if (Input.GetKey(KeyCode.D))
            {
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

        if (!IsOwner)
        {
            Vector3 toPosition = transform.position;
            Vector3 fromPosition = Camera.main.transform.position;
            fromPosition.z = 0f;
            Vector3 direction = (toPosition - fromPosition).normalized;
            float angle = ((Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) % 360);
            ArrowRectTransform.localEulerAngles = new Vector3(0, 0, angle);

            targetPositionScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
            bool IsOffScreen = targetPositionScreenPoint.x <= BorderSize || targetPositionScreenPoint.x >= Screen.width - BorderSize || targetPositionScreenPoint.y <= BorderSize || targetPositionScreenPoint.y >= Screen.height - BorderSize;

            float Distance = Vector3.Distance(transform.position, PointerWorldPosition);

            Color NewColour = InstantiatedPlayerPrefab.GetComponent<Image>().color;
            NewColour.a = (Distance / BorderSize);
            InstantiatedPlayerPrefab.GetComponent<Image>().color = NewColour;

            if (IsOffScreen)
            {
                if (!InstantiatedPlayerPrefab.activeSelf)
                {
                    InstantiatedPlayerPrefab.SetActive(true);
                }
                CappedTargetScreenPosition = targetPositionScreenPoint;
                if (CappedTargetScreenPosition.x <= BorderSize) CappedTargetScreenPosition.x = BorderSize;
                if (CappedTargetScreenPosition.x >= Screen.width - BorderSize) CappedTargetScreenPosition.x = Screen.width - BorderSize;

                if (CappedTargetScreenPosition.y <= BorderSize) CappedTargetScreenPosition.y = BorderSize;
                if (CappedTargetScreenPosition.y >= Screen.height - BorderSize) CappedTargetScreenPosition.y = Screen.height - BorderSize;

                PointerWorldPosition = UICamera.ScreenToWorldPoint(CappedTargetScreenPosition);
                ArrowRectTransform.position = PointerWorldPosition;
                ArrowRectTransform.localPosition = new Vector3(ArrowRectTransform.localPosition.x, ArrowRectTransform.localPosition.y, 0f);
            }

            else
            {
                PointerWorldPosition = UICamera.ScreenToWorldPoint(targetPositionScreenPoint);
                ArrowRectTransform.position = PointerWorldPosition;
                ArrowRectTransform.localPosition = new Vector3(ArrowRectTransform.localPosition.x, ArrowRectTransform.localPosition.y, 0f);

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
