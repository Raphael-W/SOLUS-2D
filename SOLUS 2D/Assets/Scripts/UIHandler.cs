using Newtonsoft.Json.Bson;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private GameObject ServerManager;
    private ServerManager ServerManagerScript;

    public TMP_InputField IPField;
    private string Address;

    public Button StartButton;
    public Button JoinButton;

    private GameObject MapGenerator;
    private Generation generation;

    public Slider UISlider;

    public Canvas SettingsUI;
    public Canvas GameUI;

    public Canvas[] AllUI;
    private bool PauseUIChange;
    private TMP_Text SliderText;

    public void Awake()
    {
        if (SceneManager.sceneCount == 1)
        {
            SettingsUI.enabled = false;
            GameUI.enabled = false;
        }
    }

    private void ConnectServerManager()
    {
        ServerManager = GameObject.FindGameObjectWithTag("ServerManager");
        ServerManagerScript = ServerManager.GetComponent<ServerManager>();
    }

    private void ConnectGeneration()
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
            CloseSettings();
        }

        else
        {
            ServerManagerScript.Disconnect();
            var SceneIndex = SceneManager.sceneCount - 1;
            var SceneName = SceneManager.GetSceneAt(SceneIndex);
            SceneManager.UnloadSceneAsync(SceneName);
        }
    }

    public void OpenSettings()
    {
        UISlider.value = PlayerPrefs.GetFloat("UISize");

        SettingsUI.enabled = true;
    }

    public void CloseSettings()
    {
        SettingsUI.enabled = false;
    }

    public void UISizeChanged()
    {
        PlayerPrefs.SetFloat("UISize", UISlider.value);
    }

    public void SetUISize(float size)
    {
        for (int i = 0; i < AllUI.Length; i++)
        {
            AllUI[i].scaleFactor = size/10;
        }
    }

    public void PauseUIChanges()
    {
        PauseUIChange = true;
        SliderText = GameObject.Find("Value").GetComponent<TMP_Text>();

        SliderText.enabled = true;
    }

    public void ResumeUIChanges()
    {
        PauseUIChange = false;
        SliderText = GameObject.Find("Value").GetComponent<TMP_Text>();

        SliderText.enabled = false;
    }

    public void Update()
    {
        if (!PauseUIChange)
        {
            SetUISize(PlayerPrefs.GetFloat("UISize"));
        }

        else
        {
            SliderText.text = UISlider.value.ToString();
        }
    }
}
