using UnityEngine;
using System.Collections;

public class AIStates : MonoBehaviour
{
    enum State
    {
        idle,
        wander,
        chase,
        flee
    };

    private State currentState = State.idle;
    private UnitMovement unitMovement;
    private UnitCombat unitCombat;
    private EnemyAI enemyAI;
    private Buffs buffs;

    Vector2 startingPoint = Vector2.zero;
    float nextIdleWanderChange;
    float chasingTime;
    float combatTime;
    float startUsingAbilitiesTime;

    GameObject target;
    Vector2 fleePos;
    float fleeingTime;
    bool startedFleeing;

    uint wanderBuffID;

    private GameObject closestUnit;

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
        if (currentState == State.idle || currentState == State.wander)
            return false;
        return true;
    }

    public bool canUseAbilities()
    {
        if (combatTime >= startUsingAbilitiesTime)
            return true;
        return false;
    }

    void FixedUpdate()
    {
        bool hasTarget = false;

        if (inCombat())
        {
            if (combatTime == 0) //Just got into combat
                startUsingAbilitiesTime = Random.Range(Tuner.ENEMY_ABILITY_START_COOLDOWN_MIN, Tuner.ENEMY_ABILITY_START_COOLDOWN_MAX);
            combatTime += Time.fixedDeltaTime;
        }
        else
            combatTime = 0;

        if (currentState != State.flee || target == null || !target.GetComponent<UnitCombat>().isAlive())
        {
            if (inCombat() || FrameCounter.frameNumber % 25 == 0)
                if (unitCombat.isMelee())
                    hasTarget = enemyAI.searchForOpponents(ref closestUnit, EnemyAI.SearchFilter.byDistance);
                else
                    hasTarget = enemyAI.searchForOpponents(ref closestUnit, EnemyAI.SearchFilter.byHealth);

            if (unitCombat.getLockedTarget() != null)
                hasTarget = true;

            if (hasTarget)
            {
                changeState(State.chase);
                target = unitCombat.getLockedTarget();
            }
            else
            {
                //There are no hostile targets nearby
                changeState(State.idle);
            }
        }

        switch (currentState)
        {
            case State.idle:
                if (unitMovement.isMoving() && nextIdleWanderChange < (Time.fixedTime + 1f))
                    nextIdleWanderChange = Time.fixedTime + 1f; //Haven't moved for 1 second: start wandering again
                else if (nextIdleWanderChange <= Time.fixedTime)
                    changeState(State.wander); //Time to wander a bit again
                break;
            case State.wander:
                break;
            case State.chase:
                if (target != null && target.GetComponent<UnitCombat>().isAlive())
                {
                    //Casting doesn't count as chasing: don't increase chasing timer while casting
                    if (!unitCombat.isCasting() && !UnitMovement.lineOfSight(transform.position, target.transform.position, false))
                    {
                        //Enemy can't see the target anymore
                        if (chasingTime >= Tuner.CHASING_TIME_MAX)
                            changeState(State.idle); //Give up chasing after CHASING_TIME_MAX seconds
                        else
                            chasingTime += Time.fixedDeltaTime;
                    }
                    else if (!unitCombat.isCasting())
                    {
                        //Enemy can see the target and is not casting: keep chasing and reset timer
                        chasingTime = 0;
                        if (canUseAbilities())
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (unitCombat.canCastAbility(i, Vector2.zero, false))
                                {
                                    //Try to cast an ability
                                    Ability ability = unitCombat.getAbilityList()[i];
                                    Tuner.SpellBaseAI spellBaseAI = ability.getSpellBaseAI();

                                    switch (spellBaseAI)
                                    {
                                        case Tuner.SpellBaseAI.arrowRain:
                                            //Cast under/towards the target if it is close enough
                                            if (Ellipse.isometricDistance(transform.position, target.transform.position) < ability.getMaxCastRange())
                                            {
                                                unitCombat.castAbilityInSlot(i, target.transform.position);
                                            }
                                            break;
                                        case Tuner.SpellBaseAI.charge:
                                            //Cast under/towards the target if it is close enough but not too close
                                            float dis = Ellipse.isometricDistance(transform.position, target.transform.position);
                                            if (dis <= ability.getMaxCastRange() * 0.90f && dis >= ability.getMaxCastRange() * 0.80f)
                                            {
                                                unitCombat.castAbilityInSlot(i, target.transform.position);
                                            }
                                            break;
                                        case Tuner.SpellBaseAI.selfHeal:
                                            //Cast right under/on itself
                                            unitCombat.castAbilityInSlot(i, transform.position);
                                            break;
                                        case Tuner.SpellBaseAI.whirlwind:
                                            //Cast right under/on itself if the target is close enough to skill's area of effect
                                            if (Ellipse.isometricDistance(transform.position, target.transform.position) < ability.getAreaRadius())
                                            {
                                                unitCombat.castAbilityInSlot(i, transform.position);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        if (!unitCombat.isMelee() && closestUnit != null)
                        {
                            //Ranged enemy units flee from player characters
                            float distanceToClosestUnit = Ellipse.isometricDistance(transform.position, closestUnit.transform.position);
                            if (distanceToClosestUnit <= Tuner.UNIT_BASE_MELEE_RANGE)
                            {
                                changeState(State.flee);
                            }
                        }
                    }
                }
                break;
            case State.flee:

                break;
        }

        switch (currentState)
        {
            case State.idle:

                break;
            case State.wander:
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
                        changeState(State.idle);
                    }
                }
                break;
            case State.chase:

                break;
            case State.flee:
                if (!unitCombat.isAttacking() && !unitCombat.isLockedAttack())
                {
                    Vector2 closestUnitPos = closestUnit.transform.position;
                    Vector2 myPos = transform.position;
                    float distanceToClosestUnit = Ellipse.isometricDistance(closestUnitPos, myPos);
                    if (distanceToClosestUnit >= unitCombat.getAttackRange())
                    {
                        //Stop fleeing if the closest enemy unit is far enough from us
                        changeState(State.chase);
                    }
                    else if (!startedFleeing || (startedFleeing && !unitMovement.isMoving()))
                    {
                        Vector2 end;
                        Vector2 altFleePos;
                        float fleeDistance = Tuner.UNIT_BASE_RANGED_RANGE - distanceToClosestUnit; //TODO: change this if we allow custom ranges
                        float realFleeDistance = 0;
                        float bestDistance = 0;
                        Vector2 bestFleePos = Vector2.zero;
                        //Find the best position to flee to
                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 0)
                            {
                                end = Ellipse.isometricLine(closestUnitPos, myPos, fleeDistance) + myPos;
                                altFleePos = UnitMovement.getEndOfLine(closestUnitPos, end);
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
                                Vector2 dir = (closestUnitPos - myPos).normalized;
                                dir = dir.Rotate(90f * i) * fleeDistance;
                                end = Ellipse.isometricLine(closestUnitPos, myPos - dir, fleeDistance) + myPos;
                                altFleePos = UnitMovement.getEndOfLine(closestUnitPos, end);
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
                            Debug.DrawLine(myPos, altFleePos, Color.cyan, 3.0f);
                        }
                        Debug.DrawLine(myPos, bestFleePos, Color.red, 3.0f);

                        float realFleeDistanceFromClosestUnit = Ellipse.isometricDistance(closestUnitPos, bestFleePos);
                        realFleeDistance = Ellipse.isometricDistance(myPos, bestFleePos);
                        float minFleeDistance = Tuner.UNIT_BASE_MELEE_RANGE * 2f;
                        if (realFleeDistance >= minFleeDistance && realFleeDistanceFromClosestUnit >= minFleeDistance)
                        {
                            //Move away from the closest enemy unit
                            fleePos = bestFleePos;

                            unitMovement.moveTo(fleePos);
                            startedFleeing = true;
                        }
                        else
                        {
                            //No point in fleeing if the best position to flee to is too close to the fleeing unit or to the closest enemy unit
                            changeState(State.chase);
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
        if (state == State.flee)
        {
            //Started fleeing
            unitCombat.stopAttackAfterAttacked();
        }
        else if ((state == State.wander && currentState == State.idle) || (state == State.idle && currentState == State.wander))
        {
            nextIdleWanderChange = Time.fixedTime + Random.Range(Tuner.IDLING_STATE_TIME_MIN, Tuner.IDLING_STATE_TIME_MAX);
            if (wanderBuffID > 0)
                buffs.removeBuff(wanderBuffID);
            wanderBuffID = buffs.addBuff(Buffs.BuffType.wander, 10000f);
        }
        else if (state == State.idle && currentState == State.chase)
        {
            //Give up chasing
            unitCombat.stopAttack();
            chasingTime = 0;
        }
        else if (state == State.chase && currentState != State.chase)
        {
            if (currentState == State.flee)
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
