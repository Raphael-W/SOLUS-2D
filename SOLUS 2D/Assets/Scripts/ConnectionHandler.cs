using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ConnectionHandler : MonoBehaviour
{
    public int MaxPlayers;
    private string DisconnectReason;

    private GameObject UIHandler;
    private UIHandler UIHandlerScript;

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        UIHandler = GameObject.FindGameObjectWithTag("UIHandler");
        UIHandlerScript = UIHandler.GetComponent<UIHandler>();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "ERROR: Server is Full";
        }

        response.Pending = false;
    }

    private void OnClientDisconnectCallback(ulong obj)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            if (NetworkManager.Singleton.DisconnectReason == string.Empty)
            {
                DisconnectReason = "ERROR: Disconnected";
            }

            else
            {
                DisconnectReason = NetworkManager.Singleton.DisconnectReason;
            }

            UIHandlerScript.Error(DisconnectReason, out GameObject message);
        }
    }

    public void DismissMessage()
    {

    }
}
