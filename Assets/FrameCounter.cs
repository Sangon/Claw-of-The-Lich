using UnityEngine;
using System.Collections;

public class FrameCounter : MonoBehaviour
{
    public static int frameNumber;

    void FixedUpdate()
    {
        frameNumber = Time.frameCount % 50;
        if (frameNumber == 49)
        {
            //System.GC.Collect();
        }
    }
}
