using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    private AstarAI astar = null;

	private Vector2 movementDelta;
	private	Vector2 lastPosition;
	public int direction = 0;

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
		if(new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y).magnitude >= 0.1){
			movementDelta = new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y);
		}

		if (lastPosition.x != transform.position.x && lastPosition.y != transform.position.y) {
			lastPosition = transform.position;
		}

		direction = getDirection();
	}

	public Vector2 getMovementDelta(){
		return movementDelta;
	}



	public int getDirection(){
		//Palauttaa suunnan mihin unitti on suuntaamassa.
		//	
		//		 8	 7   6
		//		  \  |  /
		//	   1-----------5
		//		  /	 |  \
		//		 2	 3   4
		//
		//
		float movementAngle = Mathf.Atan2(transform.position.y + GetComponent<UnitMovement>().getMovementDelta().y*10 - transform.position.y, transform.position.x + GetComponent<UnitMovement>().getMovementDelta().x*10 - transform.position.x) + Mathf.PI;

		float qrt = Mathf.PI*2 / 16;

		if(movementAngle > 0f && movementAngle < qrt || movementAngle > qrt*15 && movementAngle < qrt*16){
			return 1;
		}else if(movementAngle > qrt && movementAngle < qrt*3){
			return 2;
		}else if(movementAngle > qrt*3 && movementAngle < qrt*5){
			return 3;
		}else if(movementAngle > qrt*5 && movementAngle < qrt*7){
			return 4;
		}else if(movementAngle > qrt*7 && movementAngle < qrt*9){
			return 5;
		}else if(movementAngle > qrt*9 && movementAngle < qrt*11){
			return 6;
		}else if(movementAngle > qrt*11 && movementAngle < qrt*13){
			return 7;
		}else if(movementAngle > qrt*13 && movementAngle < qrt*15){
			return 8;
		}

		//Palautata oletuksena Länsisuunnan.
		return 0;

	}
}
