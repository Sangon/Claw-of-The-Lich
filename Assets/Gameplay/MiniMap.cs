using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour
{
    public Transform Target;
    private readonly float ZoomLevel = Mathf.Sqrt(1.0f / (Tuner.LEVEL_HEIGHT_IN_TILES * Tuner.LEVEL_WIDTH_IN_TILES));

    public Vector2 TransformPosition(Vector3 position)
    {
        Vector3 offset = position - Target.position;
        Vector2 newPosition = new Vector2(offset.x, offset.y);
        newPosition = newPosition * ZoomLevel;
        return newPosition;
    }
}
