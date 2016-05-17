using UnityEngine;
using System.Collections.Generic;

public class charge_skill : Skill
{
    float chargeTimer = 0;
    float maxChargeTimer = Tuner.BASE_CHARGE_DURATION;
    Vector2 chargeVector;
    GameObject parent;
    Vector2 oldDir;
    List<GameObject> hits = new List<GameObject>();

    uint buffID;

    int pathNumber;

    public charge_skill()
    {
        spellName = "charge";
        skillIcon = null;
        maxCooldown = Tuner.BASE_CHARGE_COOLDOWN;
    }

    public override void cast(GameObject owner)
    {
        if (currentCooldown <= 0)
        {
            parent = owner;

            chargeVector = (new Vector2(parent.transform.position.x, parent.transform.position.y) - getCurrentMousePos()).normalized * 200f; //TODO: Make isometric
            chargeTimer = maxChargeTimer;

            buffID = parent.GetComponent<Buffs>().addBuff(Buffs.BuffType.charge, Tuner.BASE_CHARGE_DURATION);

            parent.GetComponent<UnitCombat>().resetAttack();

            pathNumber = parent.GetComponent<AstarAI>().getPathNumber();

            currentCooldown = maxCooldown;
        }
    }
    public override void FixedUpdate()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }

        if (chargeTimer > 0)
        {
            List<GameObject> targets = getUnitsAtPoint(parent.transform.position, Tuner.BASE_CHARGE_RADIUS);

            foreach (GameObject target in targets)
            {
                if (!hits.Contains(target))
                {
                    hits.Add(target);
                    parent.GetComponent<UnitCombat>().dealDamage(target, Tuner.BASE_CHARGE_DAMAGE, Tuner.DamageType.melee);
                }
            }

            Vector2 moveVector = (new Vector2(parent.transform.position.x, parent.transform.position.y) - chargeVector);
            parent.GetComponent<UnitMovement>().moveTo(moveVector, true);
            Vector2 movementDirection = parent.GetComponent<AstarAI>().getMovementDirection();

            if (chargeTimer != maxChargeTimer && oldDir != Vector2.zero)
            {
                float distance = Ellipse.isometricDistance(parent.transform.position, parent.GetComponent<AstarAI>().getNextWaypoint());
                //Debug.Log("Dir: " + movementDirection + " oldDir: " + oldDir + " distance: " + Ellipse.isometricDistance(parent.transform.position, parent.GetComponent<AstarAI>().getNextWaypoint()));
                float angle = Vector2.Angle(oldDir, movementDirection);
                //Debug.Log("Angle: " + angle);
                if (angle > Tuner.CHARGE_MAX_ANGLE || distance < 50f)
                    chargeTimer = 0;
            }
            if (pathNumber < parent.GetComponent<AstarAI>().getPathNumber())
            {
                //Wait for AstarAI to calculate a path for us
                oldDir = movementDirection;
                chargeTimer -= Time.fixedDeltaTime;
                if (chargeTimer <= 0)
                {
                    parent.GetComponent<UnitMovement>().stop();
                    parent.GetComponent<Buffs>().removeBuff(buffID);
                    hits.Clear();
                }
            }
        }
    }
}
