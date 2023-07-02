using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class JoinClient : MonoBehaviour
{
    public TMP_InputField IPInputField;
    private string InputtedIP;

    public void JoinIP()
    {
        InputtedIP = IPInputField.text.ToString();
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(InputtedIP, (ushort)6666);

        NetworkManager.Singleton.StartClient();
    }
}