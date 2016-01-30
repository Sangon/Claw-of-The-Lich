using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    public void moveTo(Vector2 point)
    {
        GetComponent<AstarAI>().move(point);
    }

    public void stop()
    {
        GetComponent<AstarAI>().path.Reset();
    }
}
