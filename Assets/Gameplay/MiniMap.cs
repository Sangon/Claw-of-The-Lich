using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour {

    public Transform Target;
    private readonly float ZoomLevel = Mathf.Sqrt(1.0f/(25*25));

    public Vector2 TransformPosition(Vector3 position)
    {
        Vector3 offset = position - Target.position;
        Vector2 newPosition = new Vector2(offset.x, offset.y);
        newPosition = newPosition * ZoomLevel;
        return newPosition;
    }
	
}
