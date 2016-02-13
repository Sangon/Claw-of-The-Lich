using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    private AstarAI astar = null;

    public void Start()
    {
        astar = GetComponent<AstarAI>();
    }

    public void moveTo(Vector2 point, uint groupID = 0)
    {
        if (astar != null)
            astar.move(point, groupID);
    }

    public void stop()
    {
        if (astar != null)
            astar.stop();
    }
}
