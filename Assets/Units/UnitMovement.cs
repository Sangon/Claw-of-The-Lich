using UnityEngine;
using System.Collections;
using System;

public class UnitMovement : MonoBehaviour
{
    private AstarAI astar = null;
    private Animator animator = null;
    private UnitCombat unitCombat = null;
    private Traps traps = null;

    private float movementSpeed = Tuner.UNIT_BASE_SPEED;

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

    private Vector3 relativePosition = Vector3.zero;
    private Direction direction = Direction.NE;

    private float facingAngle = 0;

    private bool canTurn = true;
    private bool moving = false;

    private FMOD.Studio.EventInstance footStepsAudio;

    public void Start()
    {
        astar = GetComponent<AstarAI>();
        animator = GetComponent<Animator>();
        unitCombat = GetComponent<UnitCombat>();
        traps = GameObject.Find("Traps").GetComponent<Traps>();
        footStepsAudio = FMODUnity.RuntimeManager.CreateInstance("event:/sfx/walk");
    }

    public bool isMoving()
    {
        return moving;
    }

    public void moveTo(Vector2 point, int groupID = 0)
    {
        if (astar != null)
            astar.move(point, groupID);
    }

    public float getFacingAngle()
    {
        return facingAngle;
    }

    internal void setMovementSpeed(float value)
    {
        movementSpeed = value;
    }

    internal float getMovementSpeed()
    {
        return movementSpeed;
    }

    public void stop()
    {
        if (astar != null)
            astar.stop();
    }

    private void checkTriggerCollisions(bool debug = false)
    {
        traps.checkTrigger(transform.position, transform.tag);

        if (gameObject.tag.Equals("Player"))
        {
            Vector2 pos = gameObject.transform.position;
            Vector2 end = pos + new Vector2(0, 1f);
            if (debug)
                Debug.DrawLine(pos, end, Color.magenta);
            RaycastHit2D[] hits = Physics2D.LinecastAll(pos, end, Tuner.LAYER_FLOOR);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    hits[i].collider.gameObject.GetComponent<HouseTransparency>().trigger();
                }
            }
        }
    }

    public bool lineOfSight(Vector2 start, Vector2 end, bool debug = false)
    {
        Vector2 dir = (start - end).normalized;
        start -= dir;
        end += dir;
        RaycastHit2D hit = Physics2D.Linecast(start, end, Tuner.LAYER_OBSTACLES);
        if (debug)
            Debug.DrawLine(start, end, Color.yellow, 1.0f);
        if (hit.collider == null)
            return true;
        return false;
    }

    void Update()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        footStepsAudio.getPlaybackState(out state);
        if (isMoving())
        {
            //FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/walk", Camera.main.transform.position);
            FMOD.ATTRIBUTES_3D attributes = FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform.position);
            footStepsAudio.set3DAttributes(attributes);
            footStepsAudio.setParameterValue("Surface", 1f); // 0 = grass, 1f = sand road

            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                // Start looping sound if play condition is met and sound not already playing
                footStepsAudio.start();
            }
        }
        else if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            // Stop looping sound if already playing and play condition is no longer true
            footStepsAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if (unitCombat.isAttacking())
        {
            switch (direction)
            {
                case Direction.NE:
                case Direction.N:
                    animator.Play("Attack_NE");
                    break;
                case Direction.SE:
                case Direction.E:
                    animator.Play("Attack_SE");
                    break;
                case Direction.NW:
                case Direction.W:
                    animator.Play("Attack_NW");
                    break;
                case Direction.S:
                case Direction.SW:

                    //Käyttäkää SX SW sijaan. Älkää kysykö miks ja älkää yrittäkö vaihtaa takas.
                    animator.Play("Attack_SW");
                    break;
            }
            canTurn = true; //????
        }
        else {
            canTurn = true;
            if (animator != null && astar != null)
            {
                if (isMoving())
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
    }

    void FixedUpdate()
    {
        if (astar != null && astar.path != null)
        {
            //newPosition = astar.getNextPathPoint();
            relativePosition = astar.getMovementDirection();
            moving = true;
        }
        else
            moving = false;

        calculateDirection();
        checkTriggerCollisions();
    }

    public Direction getDirection()
    {
        return direction;
    }
    public void calculateDirection()
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
            return;

        if (isMoving() || unitCombat == null || !unitCombat.isAttacking() || (unitCombat.isLockedAttack() && !unitCombat.inRange(unitCombat.getLockedTarget())))
            ; // Do nothing
        else if (!unitCombat.hasAttacked())
        {
            Vector3 newPosition;
            // The unit is attacking; turn the unit towards its target
            if (!unitCombat.isLockedAttack())
                newPosition = PlayerMovement.getCurrentMousePos();
            else
                newPosition = unitCombat.getLockedTarget().transform.position;
            relativePosition = transform.InverseTransformPoint(newPosition);
            //print("attacking! " + relative + " facingAngle: " + Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg);
        }

        facingAngle = Mathf.Atan2(relativePosition.x, relativePosition.y) * Mathf.Rad2Deg;

        //8 directions
        float a = 22.5f;

        if (facingAngle >= -a * 5 && facingAngle < -a * 3)
        {
            direction = Direction.W;
        }
        else if (facingAngle >= -a * 7 && facingAngle < -a * 5)
        {
            direction = Direction.SW;
        }
        else if (facingAngle >= a * 7 || facingAngle < -a * 7)
        {
            direction = Direction.S;
        }
        else if (facingAngle >= a * 5 && facingAngle < a * 7)
        {
            direction = Direction.SE;
        }
        else if (facingAngle >= a * 3 && facingAngle < a * 5)
        {
            direction = Direction.E;
        }
        else if (facingAngle >= a && facingAngle < a * 3)
        {
            direction = Direction.NE;
        }
        else if (facingAngle >= -a && facingAngle < a)
        {
            direction = Direction.N;
        }
        else if (facingAngle >= -a * 3 && facingAngle < -a)
        {
            direction = Direction.NW;
        }
    }
}
