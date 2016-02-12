using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
	private Vector2 direction;

	private Vector2 movementDelta;
	private	Vector2 lastPosition;

    public void moveTo(Vector2 point)
    {
        GetComponent<AstarAI>().move(point);
    }

    public void stop()
    {
        GetComponent<AstarAI>().path.Reset();
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
