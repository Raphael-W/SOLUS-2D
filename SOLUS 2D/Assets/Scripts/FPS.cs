using UnityEngine;
using TMPro;
using System.Collections;

public class FPS : MonoBehaviour
{
    public TMP_Text FPSText;
    public float fpsRefreshTime = 1f;


    private WaitForSecondsRealtime _waitForSecondsRealtime;

    private void OnValidate()
    {

        SetWaitForSecondsRealtime();
    }

    private IEnumerator Start()
    {
        Application.targetFrameRate = 120;

        SetWaitForSecondsRealtime();

        while (true)
        {

            FPSText.text = ((int)(1 / Time.unscaledDeltaTime)) + "fps".ToString();
            yield return _waitForSecondsRealtime;
        }
    }

    private void SetWaitForSecondsRealtime()
    {

        _waitForSecondsRealtime = new WaitForSecondsRealtime(fpsRefreshTime);
    }
}
