using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
	private Vector2 direction;
    private AstarAI astar = null;

	private Vector2 movementDelta;
	private	Vector2 lastPosition;
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

	void FixedUpdate(){

		//Muodostaa deltan kahden viimesen pelitickin perusteella. Voidaan käyttää mm. suunnan kattomiseen ja animaation kääntelyyn.
		movementDelta = new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y);

		if (lastPosition.x != transform.position.x && lastPosition.y != transform.position.y) {
			lastPosition = transform.position;
		}

	}

	public Vector2 getMovementDelta(){
		return movementDelta;
	}
}
