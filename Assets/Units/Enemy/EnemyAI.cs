using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private float timeStamp;

    //private UnitMovement unitMovement = null;
    private UnitCombat unitCombat = null;
    private PartySystem partySystem = null;

    void Start()
    {
        //unitMovement = GetComponent<UnitMovement>();
        unitCombat = GetComponent<UnitCombat>();
        partySystem = GameObject.Find("PartySystem").GetComponent<PartySystem>();
    }

    void FixedUpdate()
    {
        if (partySystem.noneAlive())
            return;

        bool ranged = false;
        bool directLine = true;

        if (unitCombat.getAttackRange() > Tuner.UNIT_BASE_MELEE_RANGE)
            ranged = true;

        float dis = 0;
        GameObject target = null;

        foreach (GameObject character in partySystem.characters)
        {
            dis = Vector2.Distance(character.transform.position, transform.position);
            if (dis < Tuner.enemyAggroRange)
            {
                if (target != null && dis < Vector2.Distance(target.transform.position, transform.position))
                    target = character;
                else if (target == null)
                    target = character;
            }
        }
        if (target != null)
        {
            dis = Vector2.Distance(target.transform.position, transform.position);
            if (ranged)
            {
                RaycastHit2D hit = Physics2D.Linecast(transform.position, target.transform.position, Tuner.LAYER_OBSTACLES);
                if (hit.collider != null)
                    directLine = false;
            }

            if (directLine)
            {
                unitCombat.setLockedTarget(target);
                /*if (timeStamp < Time.time && dis > unitCombat.getAttackRange() || (timeStamp < Time.time && !directLine))
                {
                    unitMovement.moveTo(target.transform.position);
                    timeStamp = Time.time + 0.3f;
                }
                else if (dis <= unitCombat.getAttackRange() && directLine)
                {
                    unitMovement.stop();
                }
                */
            }
        }
    }
}

