using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float fps;
    public TextMeshProUGUI fpsText;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        CalculateFps();
    }

    async void CalculateFps()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = "FPS :" + fps;
        await UniTask.Delay(1000);
        CalculateFps();
    } 
   
}
