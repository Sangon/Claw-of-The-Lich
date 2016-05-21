using UnityEngine;
using System.Collections;

public class FrameCounter : MonoBehaviour
{
    public static int frameNumber;

    void FixedUpdate()
    {
        frameNumber = Time.frameCount % 60;
        if (frameNumber == 59)
        {
            //System.GC.Collect();
        }
    }
}
