using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private GameObject ServerManager;
    private ServerManager ServerManagerScript;

    public TMP_InputField IPField;
    private string Address;

    public Button StartButton;
    public Button JoinButton;

    private GameObject MapGenerator;
    private Generation generation;

    public void ConnectServerManager()
    {
        ServerManager = GameObject.FindGameObjectWithTag("ServerManager");
        ServerManagerScript = ServerManager.GetComponent<ServerManager>();
    }

    public void ConnectGeneration()
    {
        MapGenerator = GameObject.FindGameObjectWithTag("MainUniverseTag");
        generation = MapGenerator.GetComponent<Generation>();
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
        ConnectServerManager();

        Address = IPField.text.ToString();
        ServerManagerScript.StartHost(Address);

        StartButton.interactable = false;
    }

    public void StartClient()
    {
        ConnectServerManager();

        Address = IPField.text.ToString();
        ServerManagerScript.StartClient(Address);

        JoinButton.interactable = false;
    }

    public void Home()
    {
        ConnectServerManager();
        ConnectGeneration();

        if (SceneManager.sceneCount == 1)
        {
            ServerManagerScript.Disconnect();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
            generation.ClearTiles();
        }

        else
        {
            var SceneIndex = SceneManager.sceneCount - 1;
            var SceneName = SceneManager.GetSceneAt(SceneIndex);
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}
