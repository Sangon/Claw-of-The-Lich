using UnityEngine;
using System.Collections;

public class AIStates : MonoBehaviour
{
    enum State
    {
        Idle,
        Wander,
        Chase,
        Flee
    };

    private State currentState = State.Idle;
    private UnitMovement unitMovement;
    private UnitCombat unitCombat;
    private EnemyAI enemyAI;
    private Buffs buffs;

    Vector2 startingPoint = Vector2.zero;
    float nextIdleWanderChange;
    float chasingTime;

    GameObject target;
    Vector2 fleePos;
    float fleeingTime;
    bool startedFleeing = false;

    uint wanderBuffID;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
        enemyAI = GetComponent<EnemyAI>();
        startingPoint = transform.position;
        nextIdleWanderChange = Time.fixedTime + Random.Range(Tuner.IDLING_STATE_TIME_MIN, Tuner.IDLING_STATE_TIME_MAX);
        buffs = GetComponent<Buffs>();
    }

    public bool inCombat()
    {
        if (currentState == State.Idle || currentState == State.Wander)
            return false;
        return true;
    }

    void FixedUpdate()
    {
        bool hasTarget = false;

        if (currentState != State.Flee || target == null || !target.GetComponent<UnitCombat>().isAlive())
        {
            hasTarget = enemyAI.lookForOpponents();

            if (unitCombat.getLockedTarget() != null)
                hasTarget = true;

            if (hasTarget)
            {
                changeState(State.Chase);
                target = unitCombat.getLockedTarget();
            }
            else
            {
                //There are no hostile targets nearby
                changeState(State.Idle);
            }
        }

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
                if (target != null && target.GetComponent<UnitCombat>().isAlive())
                {
                    if (!UnitMovement.lineOfSight(transform.position, target.transform.position, false))
                    {
                        //Enemy can't see the target anymore
                        if (chasingTime >= Tuner.CHASING_TIME_MAX)
                            changeState(State.Idle); //Give up chasing after CHASING_TIME_MAX seconds
                        else
                            chasingTime += Time.fixedDeltaTime;
                    }
                    else
                    {
                        //Enemy can see the target: keep chasing
                        chasingTime = 0;
                        if (!unitCombat.isMelee() && target.GetComponent<UnitCombat>().isMelee())
                        {
                            float distanceToTarget = Ellipse.isometricDistance(transform.position, target.transform.position);
                            if (distanceToTarget <= Tuner.UNIT_BASE_RANGED_RANGE * 0.5f)
                            {
                                changeState(State.Flee);
                            }
                        }
                    }
                }
                break;
            case State.Flee:

                break;
        }

        switch (currentState)
        {
            case State.Idle:

                break;
            case State.Wander:
                if (!unitMovement.isMoving())
                {
                    //Find a good spot to wander to
                    Vector2 point = Vector2.zero;
                    bool foundSpot = false;
                    if (Ellipse.isometricDistance(transform.position, startingPoint) > Tuner.WANDERING_DISTANCE_MAX)
                    {
                        point = Ellipse.getRandomPointInsideEllipse(startingPoint, Tuner.WANDERING_DISTANCE);
                        foundSpot = true;
                    }
                    else
                        point = Ellipse.getRandomPointInsideEllipse(transform.position, Tuner.WANDERING_DISTANCE);

                    if (foundSpot || Ellipse.pointInsideEllipse(point, startingPoint, Tuner.WANDERING_DISTANCE_MAX) && UnitMovement.lineOfSight(transform.position, point, true))
                    {
                        unitMovement.moveTo(point);
                        changeState(State.Idle);
                    }
                }
                break;
            case State.Chase:

                break;
            case State.Flee:
                if (!unitCombat.isAttacking() && !unitCombat.isLockedAttack())
                {
                    if (!startedFleeing)
                    {
                        float fleeDistance = Tuner.UNIT_BASE_RANGED_RANGE * 0.5f; //TODO: change this if we allow custom ranges
                        Vector2 targetPos = target.transform.position;
                        Vector2 myPos = transform.position;
                        Vector2 end;
                        Vector2 altFleePos;
                        float realFleeDistance = 0;
                        float bestDistance = 0;
                        Vector2 bestFleePos = Vector2.zero;
                        //Find the best position to flee to
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0)
                            {
                                end = Ellipse.isometricLine(targetPos, myPos, fleeDistance) + myPos;
                                altFleePos = UnitMovement.getEndOfLine(targetPos, end);
                                realFleeDistance = Ellipse.isometricDistance(myPos, altFleePos);
                                if (realFleeDistance >= Ellipse.worldDistance(myPos, end))
                                {
                                    bestFleePos = altFleePos;
                                    break;
                                }
                                else
                                {
                                    bestFleePos = altFleePos;
                                    bestDistance = realFleeDistance;
                                }
                            }
                            else
                            {
                                Vector2 dir = (targetPos - myPos).normalized;
                                if (i == 1)
                                {
                                    dir = dir.Rotate(90f) * fleeDistance;
                                }
                                else
                                {
                                    dir = dir.Rotate(-90f) * fleeDistance;
                                }
                                end = Ellipse.isometricLine(targetPos, myPos - dir, fleeDistance) + myPos;
                                altFleePos = UnitMovement.getEndOfLine(targetPos, end);
                                realFleeDistance = Ellipse.isometricDistance(myPos, altFleePos);
                                if (realFleeDistance >= Ellipse.worldDistance(myPos, end))
                                {
                                    bestFleePos = altFleePos;
                                    break;
                                }
                                else if (bestDistance < realFleeDistance)
                                {
                                    bestFleePos = altFleePos;
                                    bestDistance = realFleeDistance;
                                }
                            }
                        }

                        realFleeDistance = Ellipse.isometricDistance(myPos, bestFleePos);
                        float minFleeDistance = (fleeDistance * 0.5f);
                        if (realFleeDistance >= minFleeDistance && (Ellipse.isometricDistance(targetPos, bestFleePos) - Ellipse.isometricDistance(targetPos, myPos)) >= (minFleeDistance * 0.5f))
                        {
                            //Move away from the target
                            fleePos = bestFleePos;

                            fleeingTime = realFleeDistance / unitCombat.getMovementSpeed();
                            unitMovement.moveTo(fleePos);
                            startedFleeing = true;
                        }
                        else
                        {
                            //Too close. No point moving. Just attack
                            changeState(State.Chase);
                        }
                    }
                    else
                    {
                        fleeingTime -= Time.fixedDeltaTime;
                        if (fleeingTime <= 0 || Ellipse.isometricDistance(transform.position, fleePos) <= 25f)
                        {
                            //Stop fleeing if we are close enough or we have been fleeing for a long time
                            changeState(State.Chase);
                        }
                    }
                }
                break;
        }

        if (inCombat())
            if (wanderBuffID > 0)
                buffs.removeBuff(wanderBuffID);
    }

    void changeState(State state)
    {
        if (state == State.Flee)
        {
            //Started fleeing
            unitCombat.stopAttackAfterAttacked();
        }
        else if ((state == State.Wander && currentState == State.Idle) || (state == State.Idle && currentState == State.Wander))
        {
            nextIdleWanderChange = Time.fixedTime + Random.Range(Tuner.IDLING_STATE_TIME_MIN, Tuner.IDLING_STATE_TIME_MAX);
            if (wanderBuffID > 0)
                buffs.removeBuff(wanderBuffID);
            wanderBuffID = buffs.addBuff(Buffs.BuffType.wander, 10000f);
        }
        else if (state == State.Idle && currentState == State.Chase)
        {
            //Give up chasing
            unitCombat.stopAttack();
            chasingTime = 0;
        }
        else if (state == State.Chase && currentState != State.Chase)
        {
            if (currentState == State.Flee)
            {
                if (target.GetComponent<UnitCombat>().isAlive())
                {
                    //Aggro back on the target after finished fleeing from it
                    unitCombat.aggro(target);
                    startedFleeing = false;
                }
            }
            else
            {
                //Call for help
                foreach (GameObject unit in UnitList.getHostileUnitsInArea(transform.position, Tuner.UNIT_AGGRO_CALLOUT_RANGE))
                {
                    if (unit != gameObject && !unit.GetComponent<AIStates>().inCombat())
                    {
                        if (UnitMovement.lineOfSight(transform.position, unit.transform.position, false))
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
