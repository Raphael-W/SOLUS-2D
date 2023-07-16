using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ConnectionHandler : MonoBehaviour
{
    public int MaxPlayers;
    public GameObject ErrorMessage;
    private GameObject InstantiatedErrorMessage;
    private GameObject ErrorMessageText;
    private string DisconnectReason;

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "Server is Full";
        }

        response.Pending = false;
    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            if (NetworkManager.Singleton.DisconnectReason == string.Empty)
            {
                DisconnectReason = "Disconnected";
            }

            else
            {
                DisconnectReason = NetworkManager.Singleton.DisconnectReason;
            }

            InstantiatedErrorMessage = Instantiate(ErrorMessage, new Vector3(0, 0, 0), Quaternion.identity);
            ErrorMessageText = InstantiatedErrorMessage.transform.Find("Message").gameObject;
            ErrorMessageText.GetComponent<TMP_Text>().text = ("ERROR: " + DisconnectReason);
        }
    }

    public void DismissMessage()
    {

    }
}
