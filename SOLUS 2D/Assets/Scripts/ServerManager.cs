using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class ServerManager : NetworkBehaviour
{
    public NetworkVariable<int> seed = new NetworkVariable<int>();
    private CinemachineVirtualCamera vcam;

    private GameObject MainUniverse;
    private GameObject ConnectionUI;
    private GameObject ServerUI;

    private GameObject[] Players;

    private int MapSize;
    private Generation generation;
    private Movement movement;

    private void Start()
    {
        ServerUI = GameObject.FindGameObjectWithTag("ServerUI");
        ServerUI.SetActive(false);

        ConnectionUI = GameObject.FindGameObjectWithTag("ConnectionUI");

        MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
        generation = MainUniverse.GetComponent<Generation>();

        MapSize = generation.size;
    }

    public void StartServer()
    {

        ConnectionUI.SetActive(false);
        ServerUI.SetActive(true);

        NetworkManager.Singleton.StartServer();
        NewSeed();

        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        vcam.transform.position = new Vector3(0, (MapSize / 2) - 0.12f * (MapSize / 2), -50);
        vcam.m_Lens.OrthographicSize = (MapSize / 2) * 1.2f;
    }

    public void NewSeed()
    {
        seed.Value = Random.Range(1000000, 9999999); //8024669
        generation.BeginGeneration(seed.Value);
    }

    //public void ResetSeed()
    //{
    //    SetSeed();

    //    Players = GameObject.FindGameObjectsWithTag("Player");
    //    for (int i = 0; i < Players.Length; i++)
    //    {
    //        movement = Players[i].GetComponent<Movement>();
    //        movement.MapResetClientRpc(true, seed.Value);
    //    }
    //}

    public void StopServer()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
