using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    private AstarAI astar = null;
    private Animator animator = null;
	
    public enum Direction
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    };
	
	private Vector2 movementDelta = Vector2.zero;
	private	Vector2 lastPosition = Vector2.zero;
	public Direction direction = Direction.NE;

<<<<<<< HEAD
=======
	private Vector2 movementDelta;
	private	Vector2 lastPosition;
	public int direction = 0;

>>>>>>> origin/master
    public void Start()
    {
        astar = GetComponent<AstarAI>();
        animator = GetComponent<Animator>();
    }

    public void moveTo(Vector2 point, int groupID = 0)
    {
        if (astar != null)
            astar.move(point, groupID);
    }

    public void stop()
    {
        if (astar != null)
            astar.stop();
    }

    void Update()
    {
		if (animator != null && astar != null)
        {
            if (astar.path != null)
            {
                switch (direction)
                {
                    case Direction.NE:
					case Direction.N:
                        animator.Play("Walk_NE");
                        break;
                    case Direction.SE:
					case Direction.E:
                        animator.Play("Walk_SE");
                        break;
                    case Direction.SW:
					case Direction.S:
                        animator.Play("Walk_SW");
                        break;
                    case Direction.NW:
					case Direction.W:
                        animator.Play("Walk_NW");
                        break;
                }
            }
			else
            {
				switch (direction)
                {
				case Direction.NE:
				case Direction.N:
                        animator.Play("Idle_NE");
                        break;
				case Direction.SE:
				case Direction.E:
                        animator.Play("Idle_SE");
                        break;
				case Direction.SW:
				case Direction.S:
                        animator.Play("Idle_SW");
                        break;
				case Direction.NW:
				case Direction.W:
                        animator.Play("Idle_NW");
                        break;
                }
            }
        }
    }
	
	void FixedUpdate(){

<<<<<<< HEAD
		//Muodostaa deltan kahden viimesen pelitickin perusteella. Voidaan käyttää mm. suunnan kattomiseen ja animaation kääntelyyn.
		/*
		if(new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y).magnitude >= 0.1){
			movementDelta = new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y);
		}
		
=======
		//Muodostaa deltan kahden viimesen pelitickin perusteella. Voidaan kÃ¤yttÃ¤Ã¤ mm. suunnan kattomiseen ja animaation kÃ¤Ã¤ntelyyn.
		if(new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y).magnitude >= 0.1){
			movementDelta = new Vector2(transform.position.x - lastPosition.x, transform.position.y - lastPosition.y);
		}

>>>>>>> origin/master
		if (lastPosition.x != transform.position.x && lastPosition.y != transform.position.y) {
			lastPosition = transform.position;
		}
		*/

<<<<<<< HEAD
		if (astar != null && astar.path != null) {
			Vector2 newPosition = astar.getNextPathPoint();
			//print ("New: " + newPosition + " Current: " + transform.position);
			movementDelta = new Vector2(newPosition.x - transform.position.x, newPosition.y - transform.position.y);
		}

=======
>>>>>>> origin/master
		direction = getDirection();
	}

	public Vector2 getMovementDelta(){
		return movementDelta;
	}



<<<<<<< HEAD
	public Direction getDirection(){

=======
	public int getDirection(){
>>>>>>> origin/master
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
<<<<<<< HEAD
			return Direction.W;
		}else if(movementAngle > qrt && movementAngle < qrt*3){
			return Direction.SW;
		}else if(movementAngle > qrt*3 && movementAngle < qrt*5){
			return Direction.S;
		}else if(movementAngle > qrt*5 && movementAngle < qrt*7){
			return Direction.SE;
		}else if(movementAngle > qrt*7 && movementAngle < qrt*9){
			return Direction.E;
		}else if(movementAngle > qrt*9 && movementAngle < qrt*11){
			return Direction.NE;
		}else if(movementAngle > qrt*11 && movementAngle < qrt*13){
			return Direction.N;
		}else if(movementAngle > qrt*13 && movementAngle < qrt*15){
			return Direction.NW;
		}

		//Palauttaa oletuksena Länsisuunnan.
		return Direction.W;
=======
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

		//Palautata oletuksena LÃ¤nsisuunnan.
		return 0;
>>>>>>> origin/master

	}
}
