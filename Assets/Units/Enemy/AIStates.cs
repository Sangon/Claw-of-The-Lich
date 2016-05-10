using UnityEngine;
using System.Collections;

public class AIStates : MonoBehaviour
{
    enum State
    {
        Idle,
        Wander,
        Chase
    };

    private State currentState = State.Idle;
    private UnitMovement unitMovement = null;
    private UnitCombat unitCombat = null;
    private EnemyAI enemyAI = null;

    Vector2 startingPoint = Vector2.zero;
    float nextIdleWanderChange;
    float chasingTime;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
        enemyAI = GetComponent<EnemyAI>();
        startingPoint = transform.position;
        nextIdleWanderChange = Time.fixedTime + Random.Range(Tuner.IDLING_STATE_TIME_MIN, Tuner.IDLING_STATE_TIME_MAX);
    }

    void FixedUpdate()
    {
        bool hasTarget = enemyAI.lookForOpponents();

        if (unitCombat.getLockedTarget() != null)
            hasTarget = true;

        if (hasTarget)
            changeState(State.Chase);

        switch (currentState)
        {
            case State.Idle:
                if (unitMovement.isMoving() && nextIdleWanderChange < (Time.fixedTime + 1f))
                    nextIdleWanderChange = Time.fixedTime + 1f; //Haven't moved for 1 second: start wandering again
                else if (nextIdleWanderChange <= Time.fixedTime)
                    changeState(State.Wander); //Time to wander a bit again
                break;
            case State.Wander:
                break;
            case State.Chase:
                if (unitCombat.getLockedTarget() != null)
                {
                    if (!unitMovement.lineOfSight(transform.position, unitCombat.getLockedTarget().transform.position))
                    {
                        if (chasingTime >= Tuner.CHASING_TIME_MAX)
                            changeState(State.Idle); //Give up chasing after CHASING_TIME_MAX seconds
                        else
                            chasingTime += Time.fixedDeltaTime;
                    }
                    else
                    {
                        //Enemy can see the player: keep chasing
                        chasingTime = 0;
                    }
                }
                break;
        }

        switch (currentState)
        {
            case State.Idle:

                break;
            case State.Wander:
                if (!unitMovement.isMoving())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 point = startingPoint + (Random.insideUnitCircle * Tuner.WANDERING_DISTANCE_MAX);
                        if (unitMovement.lineOfSight(startingPoint, point))
                        {
                            unitMovement.moveTo(point);
                            break;
                        }
                    }
                    changeState(State.Idle);
                }
                break;
            case State.Chase:

                break;
        }
    }

    void changeState(State state)
    {
        if ((state == State.Wander && currentState == State.Idle) || (state == State.Idle && currentState == State.Wander))
        {
            nextIdleWanderChange = Time.fixedTime + Random.Range(Tuner.IDLING_STATE_TIME_MIN, Tuner.IDLING_STATE_TIME_MAX);
        }
        else if (state == State.Idle && currentState == State.Chase)
        {
            //Give up chasing
            unitCombat.stopAttack();
            chasingTime = 0;
        }
        else if (state == State.Chase && currentState != State.Chase)
        {
            foreach (GameObject unit in UnitList.getHostiles())
            {
                if (unit != gameObject && !unit.GetComponent<UnitCombat>().isAttacking())
                {
                    float dis = Ellipse.isometricDistance(transform.position, unit.transform.position);
                    if (dis <= Tuner.UNIT_AGGRO_CALLOUT_RANGE)
                    {
                        if (unitMovement.lineOfSight(transform.position, unit.transform.position))
                        {
                            unit.GetComponent<UnitCombat>().aggro(unitCombat.getLockedTarget());
                        }
                    }
                }
            }
        }
        currentState = state;
    }
}
