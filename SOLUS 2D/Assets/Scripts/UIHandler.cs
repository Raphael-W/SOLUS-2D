using TMPro;
using UnityEngine;
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
    public Slider SoundsSlider;

    public Canvas SettingsUI;
    public Canvas GameUI;

    public Canvas[] AllUI;
    private bool PauseUIChange;
    public TMP_Text UISliderText;
    public TMP_Text SoundsSliderText;

    public void Awake()
    {
        if (SceneManager.sceneCount == 1)
        {
            SettingsUI.enabled = false;
            GameUI.enabled = false;
        }

        else if (SceneManager.sceneCount == 2)
        {
            SettingsUI.enabled = false;
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
        UISlider.value = PlayerPrefs.GetFloat("UISize", 10);

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

    public void SoundsChanged()
    {
        PlayerPrefs.SetFloat("Sounds", SoundsSlider.value);
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
        UISliderText.enabled = true;
    }

    public void ResumeUIChanges()
    {
        PauseUIChange = false;
        UISliderText.enabled = false;
    }

    public void PauseSoundsChanges()
    {
        SoundsSliderText.enabled = true;
    }

    public void ResumeSoundsChanges()
    {
        SoundsSliderText.enabled = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Update()
    {
        if (!PauseUIChange)
        {
            SetUISize(PlayerPrefs.GetFloat("UISize", 10));
        }

        else
        {
            UISliderText.text = UISlider.value.ToString();
        }

        SoundsSliderText.text = SoundsSlider.value.ToString();
    }
}
