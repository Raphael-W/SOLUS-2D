using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class ServerManager : NetworkBehaviour
{
    public NetworkVariable<int> seed = new NetworkVariable<int>();
    private CinemachineVirtualCamera VirtualCamera;

    private GameObject MainUniverse;
    private GameObject ConnectionUI;
    private GameObject ServerUI;

    private int MapSize;
    private Generation generation;

    private void Start()
    {
        ServerUI = GameObject.FindGameObjectWithTag("ServerUI");
        ConnectionUI = GameObject.FindGameObjectWithTag("ConnectionUI");
        MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
        generation = MainUniverse.GetComponent<Generation>();
        VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        ServerUI.SetActive(false);
        MapSize = generation.MapSize;
    }

    public void StartServer()
    {

        ConnectionUI.SetActive(false);
        ServerUI.SetActive(true);

        NetworkManager.Singleton.StartServer();
        NewSeed();

        VirtualCamera.transform.position = new Vector3(0, (MapSize / 2) - 0.12f * (MapSize / 2), -50);
        VirtualCamera.m_Lens.OrthographicSize = (MapSize / 2) * 1.2f;
    }

    public void NewSeed()
    {
        seed.Value = Random.Range(1000000, 9999999);
        generation.BeginGeneration(seed.Value);
    }

    public void StopServer()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
