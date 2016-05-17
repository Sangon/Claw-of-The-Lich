using UnityEngine;
using System.Collections;
using System;

public class UnitMovement : MonoBehaviour
{
    private AstarAI astar;
    private Animator animator;
    private UnitCombat unitCombat;
    private Buffs buffs;
    private Traps traps;

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

    private enum Animations
    {
        none,
        attack,
        idle,
        move,
        death
    }

    private Animations lastAnimation;

    private Vector3 relativePosition = Vector3.zero;
    private Direction direction = Direction.NE;

    private float facingAngle = 0;

    private bool canTurn = true;
    private bool moving = false;

    private FMOD.Studio.EventInstance footStepsAudio;

    public void Awake()
    {
        astar = GetComponent<AstarAI>();
        animator = GetComponent<Animator>();
        unitCombat = GetComponent<UnitCombat>();
        buffs = GetComponent<Buffs>();
        traps = GameObject.Find("Traps").GetComponent<Traps>();
        footStepsAudio = FMODUnity.RuntimeManager.CreateInstance("event:/sfx/walk");
    }

    public bool isMoving()
    {
        return moving;
    }

    public void moveTo(Vector2 point, bool force = false, int groupID = 0)
    {
        if (astar != null && unitCombat.isAlive() && (force || !buffs.isUncontrollable()))
            astar.move(point, groupID);
    }

    public float getFacingAngle()
    {
        return facingAngle;
    }

    public void stop(bool stopAnimation = false)
    {
        if (astar != null)
            astar.stop();
        if (stopAnimation)
        {
            //playAnimation(Animations.idle);
        }
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

    public static bool lineOfSight(Vector2 start, Vector2 end, bool collideOnWater = true, bool debug = false)
    {
        Vector2 dir = (start - end).normalized;
        start -= dir;
        end += dir;
        RaycastHit2D hit;
        if (collideOnWater)
            hit = Physics2D.Linecast(start, end, Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);
        else
            hit = Physics2D.Linecast(start, end, Tuner.LAYER_OBSTACLES);
        if (debug)
            Debug.DrawLine(start, end, Color.yellow, 1.0f);
        if (hit.collider == null)
            return true;
        return false;
    }

    public static Vector2 getEndOfLine(Vector2 start, Vector2 end, bool debug = false)
    {
        RaycastHit2D hit = Physics2D.Linecast(start, end, Tuner.LAYER_OBSTACLES | Tuner.LAYER_WATER);
        if (debug)
            Debug.DrawLine(start, end, Color.yellow, 0.5f);
        if (hit.collider != null)
        {
            return hit.point;
        }
        return end;
    }

    public void knockback(GameObject source, float distance)
    {
        Vector2 start = source.transform.position;
        Vector2 characterPos = gameObject.transform.position;
        Vector2 end = Ellipse.isometricLine(start, characterPos, distance) + characterPos;
        Vector2 knockbackPos = getEndOfLine(start, end);
        gameObject.transform.position = knockbackPos;
        buffs.addBuff(Buffs.BuffType.knockback, Tuner.KNOCKBACK_STUN_DURATION);
    }

    void Update()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        footStepsAudio.getPlaybackState(out state);
        if (isMoving())
        {
            //FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/walk", AudioScript.get3DAudioPositionVector3(transform.position));
            //FMOD.ATTRIBUTES_3D attributes = FMODUnity.RuntimeUtils.To3DAttributes(Camera.main.transform.position);
            footStepsAudio.set3DAttributes(AudioScript.get3DAudioPosition(transform.position));
            footStepsAudio.setParameterValue("Surface", 1f); // 0 = grass, 1f = sand road

            if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                //Start looping sound if play condition is met and sound not already playing
                footStepsAudio.start();
            }
        }
        else if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            //Stop looping sound if already playing and play condition is no longer true
            footStepsAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if (!unitCombat.isAlive())
        {
            canTurn = false;
            playAnimation(Animations.death);
        }
        else if (buffs.isStunned())
        {
            canTurn = false;
            playAnimation(Animations.idle);
        }
        else if (unitCombat.isAttacking())
        {
            canTurn = true;
            playAnimation(Animations.attack);
        }
        else {
            canTurn = true;
            if (isMoving())
            {
                playAnimation(Animations.move);
            }
            else
            {
                playAnimation(Animations.idle);
            }
        }
    }

    private void playAnimation(Animations animation)
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        float playbackTime = currentState.normalizedTime % 1;
        switch (animation)
        {
            case Animations.attack:
                String animationName = "";
                switch (direction)
                {
                    case Direction.NE:
                    case Direction.N:
                        animationName = "Attack_NE";
                        break;
                    case Direction.SE:
                    case Direction.E:
                        animationName = "Attack_SE";
                        break;
                    case Direction.NW:
                    case Direction.W:
                        animationName = "Attack_NW";
                        break;
                    case Direction.S:
                    case Direction.SW:
                        animationName = "Attack_SW";
                        break;
                }
                //Continue playing the last attack animation from the same time for smoother animation changes
                if (lastAnimation == Animations.attack && !currentState.IsName(animationName))
                    animator.Play(animationName, 0, playbackTime);
                else
                    animator.Play(animationName, 0);
                lastAnimation = Animations.attack;
                animator.speed = 1.0f / unitCombat.getMaxAttackTimer();
                break;
            case Animations.move:
                switch (direction)
                {
                    case Direction.NE:
                    case Direction.N:
                        animator.Play("Walk_NE", 0);
                        break;
                    case Direction.SE:
                    case Direction.E:
                        animator.Play("Walk_SE", 0);
                        break;
                    case Direction.SW:
                    case Direction.S:
                        animator.Play("Walk_SW", 0);
                        break;
                    case Direction.NW:
                    case Direction.W:
                        animator.Play("Walk_NW", 0);
                        break;
                }
                lastAnimation = Animations.move;
                animator.speed = 1.5f * (unitCombat.getMovementSpeed() / Tuner.UNIT_BASE_SPEED);
                break;
            case Animations.idle:
                switch (direction)
                {
                    case Direction.NE:
                    case Direction.N:
                        animator.Play("Idle_NE", 0);
                        break;
                    case Direction.SE:
                    case Direction.E:
                        animator.Play("Idle_SE", 0);
                        break;
                    case Direction.SW:
                    case Direction.S:
                        animator.Play("Idle_SW", 0);
                        break;
                    case Direction.NW:
                    case Direction.W:
                        animator.Play("Idle_NW", 0);
                        break;
                }
                lastAnimation = Animations.idle;
                break;
            case Animations.death:
                if (lastAnimation != Animations.death)
                    animator.Play("Death", 0);

                animator.speed = 2.0f;

                lastAnimation = Animations.death;
                break;
        }
    }

    void FixedUpdate()
    {
        if (!unitCombat.isAlive())
            return;
        if (astar != null && astar.path != null)
        {
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
        calculateDirection();
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
        {
        }
        else //if (!unitCombat.hasAttacked())
        {
            Vector3 newPosition = Vector3.zero;

            if (unitCombat.isLockedAttack())
            {
                //The unit is attacking; turn the unit towards its target
                newPosition = unitCombat.getLockedTarget().transform.position;
            }
            else if (tag.Equals("Player"))
            {
                //Turn the unit towards mouse click position
                newPosition = gameObject.GetComponent<PlayerMovement>().getClickPosition();
            }

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
