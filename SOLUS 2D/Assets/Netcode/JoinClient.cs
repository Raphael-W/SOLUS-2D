using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
//using MLAPI.Transports.UNET;
using TMPro;

public class JoinClient : MonoBehaviour
{
    public TMP_InputField IPField;
    private string InputtedIP;

    public void JoinIP()
    {
        InputtedIP = IPField.text.ToString();
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(InputtedIP, (ushort)6666);

        NetworkManager.Singleton.StartClient();
    }
}