using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class ServerManager : NetworkBehaviour
{
    private static NetworkVariable<int> seed = new NetworkVariable<int>();
    private CinemachineVirtualCamera vcam;

    private GameObject MainUniverse;
    private GameObject ConnectionUI;
    private GameObject ServerUI;

    private GameObject[] Players;

    private int MapSize;

    private void Start()
    {
        ServerUI = GameObject.FindGameObjectWithTag("ServerUI");
        ServerUI.SetActive(false);
    }

    public void SetSeed()
    {
        seed.Value = Random.Range(1000000, 9999999); //8024669

        MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
        Generation generation = MainUniverse.GetComponent<Generation>();
        generation.BeginGeneration(seed.Value);
    }

    public void ResetSeed()
    {
        SetSeed();

        Players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < Players.Length; i++)
        {
            Debug.Log("Sent");
            Movement movement = Players[i].GetComponent<Movement>();
            movement.MapResetClientRpc(true, seed.Value);
        }
    }

    public void StopServer()
    {
        NetworkManager.Singleton.Shutdown();
    }

    public static int GetSeed()
    {
        return seed.Value;
    }

    // Start is called before the first frame update
    public void StartServer()
    {
        ConnectionUI = GameObject.FindGameObjectWithTag("ConnectionUI");
        ConnectionUI.SetActive(false);

        ServerUI.SetActive(true);

        NetworkManager.Singleton.StartServer();
        seed.Value = Random.Range(1000000, 9999999); //8024669

        MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
        Generation generation = MainUniverse.GetComponent<Generation>();
        generation.BeginGeneration(ServerManager.GetSeed());

        MapSize = generation.size;

        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        vcam.transform.position = new Vector3(0, (MapSize/2) - 0.12f * (MapSize / 2), -50);
        vcam.m_Lens.OrthographicSize = (MapSize / 2) * 1.2f;
    }
}
