using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = Tuner.FPS_TARGET_FRAME_RATE;
    }

    void Update()
    {
        if (Input.GetKey("escape"))
            if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
        //Application.Quit();
    }
}
