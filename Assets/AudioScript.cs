using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour
{
    public static FMOD.ATTRIBUTES_3D get3DAudioPosition(Vector3 pos)
    {
        Vector3 camPos = Camera.main.transform.position;
        pos.z = camPos.z;
        Vector3 norm = (camPos - pos).normalized * (Vector2.Distance(camPos, pos) / 200f);
        camPos -= norm;
        FMOD.ATTRIBUTES_3D fmodPos = FMODUnity.RuntimeUtils.To3DAttributes(camPos);
        return fmodPos;
    }

    public static Vector3 get3DAudioPositionVector3(Vector3 pos)
    {
        Vector3 camPos = Camera.main.transform.position;
        pos.z = camPos.z;

        //pos.z = 0;
        //camPos.z = 0;

        Vector3 norm = (camPos - pos).normalized * (Vector2.Distance(camPos, pos) / 200f);
        camPos -= norm;
        return camPos;
    }
}
