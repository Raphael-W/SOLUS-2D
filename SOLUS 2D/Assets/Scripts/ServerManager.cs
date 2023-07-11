using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class ServerManager : NetworkBehaviour
{
    public NetworkVariable<int> seed = new NetworkVariable<int>();
    private GameObject MainUniverse;
    private Generation generation;

    public TMP_InputField IPField;
    private string Address;

    private void Start()
    {
        MainUniverse = GameObject.FindGameObjectWithTag("MainUniverseTag");
        generation = MainUniverse.GetComponent<Generation>();
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene("StartGame", LoadSceneMode.Additive);
    }

    public void JoinGameButton()
    {
        SceneManager.LoadScene("JoinGame", LoadSceneMode.Additive);
    }

    public void StartHost()
    {
        Address = IPField.text.ToString();
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(Address, (ushort)6666);
        NetworkManager.Singleton.StartHost();
        NewSeed();
    }

    public void NewSeed()
    {
        if (IsServer)
        {
            seed.Value = Random.Range(1000000, 9999999);
        }
        
        generation.BeginGeneration(seed.Value);
    }

    public void StopServer()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
