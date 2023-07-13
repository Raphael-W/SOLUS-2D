using TMPro;
using UnityEngine;
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

    public void Start()
    {
        ServerManager = GameObject.FindGameObjectWithTag("ServerManager");
        ServerManagerScript = ServerManager.GetComponent<ServerManager>();
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
        ServerManagerScript.StartHost(Address);

        StartButton.interactable = false;
    }

    public void StartClient()
    {
        Address = IPField.text.ToString();
        ServerManagerScript.StartClient(Address);

        JoinButton.interactable = false;
    }
}
