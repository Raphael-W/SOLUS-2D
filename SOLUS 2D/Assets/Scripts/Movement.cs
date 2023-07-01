using UnityEngine;
using Unity.Netcode;


public class Movement : NetworkBehaviour
{

    public Rigidbody2D rb;
    private MvInputKey Key;

    private Vector2 oldPosition;
    public float rotationMomentum = 0.8f;
    private float speed;

    private GameObject MainUniverse;

    public float mainThrust = 600;
    public float rotationThrust = 150f;
    private float prevRotation = 0f;

    private GameObject ConnectionUI;


    enum MvInputKey {
        Key_Neutral = 0,
        Key_Left = 1,
        Key_Right = 2,
    };

    private void Initialize()
    {
        if (IsOwner)
        {
            rb = GetComponent<Rigidbody2D>();

            ConnectionUI = GameObject.FindGameObjectWithTag("ConnectionUI");
            ConnectionUI.SetActive(false);
        }
        
        transform.position = new Vector3(250, 200, 0);
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            base.OnNetworkSpawn();

            MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
            Generation generation = MainUniverse.GetComponent<Generation>();
            generation.BeginGeneration(ServerManager.GetSeed());
        }

        
        Initialize();

    }

    void Update()
    {
        if (!IsOwner) return;

        Key = MvInputKey.Key_Neutral;

        if (Input.GetKey(KeyCode.A)) {
            Key = Key | MvInputKey.Key_Left;

        }
    
        if (Input.GetKey(KeyCode.D)) {
            Key = Key | MvInputKey.Key_Right;

        }



    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if (!IsOwner) return;

        ProcessRotation();
        //rb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        speed = Vector2.Distance(oldPosition, transform.position) * 100f;
        oldPosition = transform.position;



    }



    void ProcessRotation()
    {
        //textLabel.text = "";

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
                rb.AddRelativeForce(Vector2.up * mainThrust * Time.deltaTime);
            }

            else {
                rb.AddRelativeForce(Vector2.up * (mainThrust / 2) * Time.deltaTime);
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
        rb.freezeRotation = true; //freezing rotation so we can manually rotate
        //transform.Rotate(Vector3.forward * Time.deltaTime * rotationThisFrame);
        //transform.eulerAngles = Vector3.Lerp(Vector3.zero, rotationThisFrame, Time.deltaTime * 5);

        Quaternion desiredRotation = transform.rotation * Quaternion.Euler(Vector3.forward * rotationThisFrame * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * 100f);

        rb.freezeRotation = false; //unfreezing rotation so the physics system can take over
    }
}
