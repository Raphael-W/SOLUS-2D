using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Netcode;

public class Ping : MonoBehaviour
{
    public TMP_Text PingText;
    private float CurrentPing;
    public float fpsRefreshTime = 1f;


    private WaitForSecondsRealtime _waitForSecondsRealtime;

    private void OnValidate()
    {

        SetWaitForSecondsRealtime();
    }

    private IEnumerator Start()
    {
        SetWaitForSecondsRealtime();

        while (true)
        {
            CurrentPing = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.LocalClientId);
            PingText.text = ((int)CurrentPing + "ms").ToString();
            yield return _waitForSecondsRealtime;
        }
    }

    private void SetWaitForSecondsRealtime()
    {

        _waitForSecondsRealtime = new WaitForSecondsRealtime(fpsRefreshTime);
    }
}
