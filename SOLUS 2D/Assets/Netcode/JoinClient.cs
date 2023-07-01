using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class JoinClient : MonoBehaviour
{
    public TextMesh IPField;
    private string InputtedIP;

    public void JoinIP()
    {
        InputtedIP = IPField.text.ToString();
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = InputtedIP;

        NetworkManager.Singleton.StartClient();
    }
}