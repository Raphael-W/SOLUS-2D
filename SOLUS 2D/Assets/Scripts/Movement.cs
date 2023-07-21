using System;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Movement : NetworkBehaviour
{
    [Header("Player Settings")]
    public Rigidbody2D RigidBody;
    public int health;

    [Header("Shoot Settings")]
    public GameObject Gun;
    public float Bullets;
    private float BulletsRemaining;
    public float ReloadSpeed;
    public float landPrecision = 15f;

    private MvInputKey Key;

    [Header("Movement Settings")]

    public float rotationMomentum = 0.8f;
    private float prevRotation = 0f;
    public float mainThrust = 600;
    public float rotationThrust = 150f;

    private float currentRotation;

    private GameObject BulletsDisplay;
    private TMP_Text BulletsText;

    private GameObject HealthDisplay;
    private TMP_Text HealthText;

    private GameObject UIHandler;
    private UIHandler UIHandlerScript;

    private LayerMask tileLayer;
    private Vector2 rayOrigin;
    private Vector2 rayDirection;
    private Vector2 landOffset;
    private RaycastHit2D landCheckL;
    private RaycastHit2D landCheckR;
    
    [Header("Spawn Settings")]
    public Color[] PlayerColours;
    private Color PlayerColour;

    private GameObject ArrowUI;
    public GameObject PlayerArrowPrefab;
    private RectTransform ArrowRectTransform;
    private GameObject InstantiatedPlayerPrefab;
    public float BorderSize;

    private Vector3 PointerWorldPosition;
    private Vector3 CappedTargetScreenPosition;
    private Vector3 targetPositionScreenPoint;

    private Camera UICamera;

    private bool ValidSpawn;
    private GameObject MapGenerator;
    private Generation GenerationScript;
    private int MapSize;
    private RaycastHit2D SpawnCheckDownL;
    private RaycastHit2D SpawnCheckDownR;

    private RaycastHit2D SpawnCheckUpL;
    private RaycastHit2D SpawnCheckUpR;
    private bool Flat;
    public int SpawnHeadRoom;
    private bool landed;

    public int PlayerSpawned;
    public bool FirstSpawn;

    private GameObject ServerManager;
    private ServerManager ServerManagerScript;

    enum MvInputKey {
        Key_Neutral = 0,
        Key_Left = 1,
        Key_Right = 2,
    };

    private void Awake()
    {
        BulletsRemaining = Bullets;

        MapGenerator = GameObject.FindGameObjectWithTag("MainUniverseTag");
        GenerationScript = MapGenerator.GetComponent<Generation>();

        BulletsDisplay = GameObject.FindGameObjectWithTag("BulletsDisplay");
        BulletsText = BulletsDisplay.GetComponent<TMP_Text>();

        HealthDisplay = GameObject.FindGameObjectWithTag("HealthDisplay");
        HealthText = HealthDisplay.GetComponent<TMP_Text>();

        UIHandler = GameObject.FindGameObjectWithTag("UIHandler");
        UIHandlerScript = UIHandler.GetComponent<UIHandler>();

        ServerManager = GameObject.FindGameObjectWithTag("ServerManager");
        ServerManagerScript = ServerManager.GetComponent<ServerManager>();
    }

    public int FindSpawnPoint(int countdown)
    {
        MapSize = GenerationScript.MapSize;
        ValidSpawn = false;
        rayOrigin = new Vector3(UnityEngine.Random.Range(5, MapSize - 5), UnityEngine.Random.Range(5, MapSize - 5), 0);

        SpawnCheckDownL = Physics2D.Raycast(rayOrigin + landOffset, Vector2.down, Mathf.Infinity, tileLayer);
        SpawnCheckDownR = Physics2D.Raycast(rayOrigin - landOffset, Vector2.down, Mathf.Infinity, tileLayer);

        Flat = (SpawnCheckDownL.distance == SpawnCheckDownR.distance);

        SpawnCheckUpL = Physics2D.Raycast(rayOrigin + landOffset, Vector2.up, Mathf.Infinity, tileLayer);
        SpawnCheckUpR = Physics2D.Raycast(rayOrigin - landOffset, Vector2.up, Mathf.Infinity, tileLayer);

        ValidSpawn = (Mathf.Min(SpawnCheckDownL.distance, SpawnCheckDownR.distance) + Mathf.Min(SpawnCheckUpL.distance, SpawnCheckUpR.distance) >= SpawnHeadRoom) && Flat;
        if (ValidSpawn)
        {
            if (countdown == 0)
            {
                transform.position = (rayOrigin - new Vector2(0, Mathf.Min(SpawnCheckDownL.distance, SpawnCheckDownR.distance)) + new Vector2(0, 2));
                transform.rotation = Quaternion.Euler(0f, 0, 0f);

                if (SceneManager.sceneCount > 1)
                {
                    GenerationScript.LoadGame();
                } 
            }
            return countdown -= 1;
        }

        else
        {
            return countdown;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            PlayerSpawned = Convert.ToInt32(OwnerClientId);
            RigidBody = GetComponent<Rigidbody2D>();

            tileLayer = LayerMask.GetMask("TileMap");
            UIHandlerScript.GameUI.enabled = true;
        }

        if (!IsOwner)
        {
            PlayerColour = PlayerColours[OwnerClientId];

            GetComponent<SpriteRenderer>().color = PlayerColour;

            ArrowUI = transform.Find("ArrowUI").gameObject;

            InstantiatedPlayerPrefab = Instantiate(PlayerArrowPrefab, ArrowUI.transform);
            InstantiatedPlayerPrefab.GetComponent<Image>().color = PlayerColour;

            ArrowRectTransform = InstantiatedPlayerPrefab.GetComponent<RectTransform>();

            UICamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }
    

    void Update()
    {
        if (health <= 0 && IsOwner)
        {
            NetworkManager.Shutdown();
            UIHandlerScript.Error("HAHA biiaaatch! You lose!");
        }

        if ((PlayerSpawned >= 0) && IsOwner)
        {
            PlayerSpawned = FindSpawnPoint(PlayerSpawned);
        }

        if (IsOwner)
        {
            Key = MvInputKey.Key_Neutral;

            if (Input.GetKey(KeyCode.A))
            {
                Key |= MvInputKey.Key_Left;
            }

            if (Input.GetKey(KeyCode.D))
            {
                Key |= MvInputKey.Key_Right;
            }

            if ((Input.GetKeyDown(KeyCode.W) ||  Input.GetKeyDown(KeyCode.Space)) && (BulletsRemaining >= 1) && (!landed))
            {
                Vector3 ShootDirection = (Gun.transform.position - transform.position).normalized;
                ServerManagerScript.ShootBulletServerRpc(Gun.transform.position, Quaternion.identity, ShootDirection, RigidBody.velocity.magnitude, OwnerClientId);
                BulletsRemaining -= 1;
            }

            BulletsText.text = (Math.Floor(BulletsRemaining)).ToString();
            HealthText.text = (health).ToString();
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
                landed = true;
                if (BulletsRemaining <= Bullets)
                {
                    BulletsRemaining += ReloadSpeed;
                }

                else
                {
                    BulletsRemaining = Bullets;
                }
            }

            else
            {
                landed = false;
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
            
        }
        if ((Key & MvInputKey.Key_Right) != 0) {
            prevRotation = -rotationThrust;
            RotatePlayer(-rotationThrust);
        }
        if (Key != MvInputKey.Key_Neutral) {

            if ((Key & MvInputKey.Key_Left) != 0 && (Key & MvInputKey.Key_Right) != 0) {
                RigidBody.AddRelativeForce(Vector2.up * mainThrust * Time.deltaTime);
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

    [ClientRpc]
    public void PlayerHitClientRpc(ulong PlayerHitID)
    {
        if (PlayerHitID == OwnerClientId)
        {
            health--;
        }

        if (IsOwner)
        {
            PlayerSpawned = 1;
        }
    }
}
