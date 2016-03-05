using UnityEngine;
using System.Collections;

public class UnitMovement : MonoBehaviour
{
    private AstarAI astar = null;
    private Animator animator = null;
    private UnitCombat unitCombat = null;

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

    private Vector3 newPosition = Vector3.zero;
    private Vector2 relative = Vector2.zero;
    public Direction direction = Direction.NE;

    private float facingAngle = 0;

    private bool canTurn = true;
    private bool isMoving = false;

    public void Start()
    {
        astar = GetComponent<AstarAI>();
        animator = GetComponent<Animator>();
        unitCombat = GetComponent<UnitCombat>();
    }

    public void moveTo(Vector2 point, int groupID = 0)
    {
        if (astar != null)
            astar.move(point, groupID);
    }

    public float getFacingAngle()
    {
        getDirection();
        return facingAngle;
    }

    public void stop()
    {
        if (astar != null)
            astar.stop();
    }

    void Update()
    {
        if (unitCombat.isAttacking())
        {
            //if (canTurn)
                //animator.Play("Attack");
            canTurn = false;
        }
        else
            canTurn = true;
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

    void FixedUpdate()
    {
        if (astar != null && astar.path != null)
        {
            newPosition = astar.getNextPathPoint();
            isMoving = true;
        }
        else
            isMoving = false;

        direction = getDirection();
    }



    public Direction getDirection()
    {
        //Palauttaa suunnan mihin unitti on suuntaamassa.
        //	
        //		 8	 1   2
        //		  \  |  /
        //	   7-----------3
        //		  /	 |  \
        //		 6	 5   4
        //
        //
        if (!canTurn)
            return direction;

        if (isMoving || unitCombat == null || unitCombat.getLockedTarget() == null || !unitCombat.inRange(unitCombat.getLockedTarget()))
            relative = transform.InverseTransformPoint(newPosition);
        else if (unitCombat.getLockedTarget() != null && unitCombat.inRange(unitCombat.getLockedTarget()))
        {
            relative = transform.InverseTransformPoint(unitCombat.getLockedTarget().transform.position);
            //print("attacking! " + relative + " facingAngle: " + Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg);
        }
        else
            print("BUG");
        facingAngle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;

        //8 directions
        float a = 22.5f;

        if (facingAngle >= -a * 5 && facingAngle < -a * 3)
        {
            return Direction.W;
        }
        else if (facingAngle >= -a * 7 && facingAngle < -a * 5)
        {
            return Direction.SW;
        }
        else if (facingAngle >= a * 7 || facingAngle < -a * 7)
        {
            return Direction.S;
        }
        else if (facingAngle >= a * 5 && facingAngle < a * 7)
        {
            return Direction.SE;
        }
        else if (facingAngle >= a * 3 && facingAngle < a * 5)
        {
            return Direction.E;
        }
        else if (facingAngle >= a && facingAngle < a * 3)
        {
            return Direction.NE;
        }
        else if (facingAngle >= -a && facingAngle < a)
        {
            return Direction.N;
        }
        else if (facingAngle >= -a * 3 && facingAngle < -a)
        {
            return Direction.NW;
        }

        //Palauttaa oletuksena pohjoisen.
        return Direction.N;

    }
}
