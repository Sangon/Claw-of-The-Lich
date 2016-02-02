using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    private AstarAI astar = null;

    public void Start()
    {
        astar = GetComponent<AstarAI>();
    }

    public void moveTo(Vector2 point)
    {
        if (astar != null)
            astar.move(point);
    }

    public void stop()
    {
        if (astar != null && astar.path != null)
            astar.path.Reset();
    }
}
