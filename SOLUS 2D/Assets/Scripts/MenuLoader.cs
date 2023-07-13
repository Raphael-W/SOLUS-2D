using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
}
