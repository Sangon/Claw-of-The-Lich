using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
	private Vector2 direction;
    public void moveTo(Vector2 point)
    {
        GetComponent<AstarAI>().move(point);
    }

    public void stop()
    {
        GetComponent<AstarAI>().path.Reset();
    }
}
