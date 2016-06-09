using UnityEngine;
using System.Collections.Generic;

public class Ability_Charge : Ability
{
    private float chargeTimer = 0;
    private float maxChargeTimer = Tuner.BASE_CHARGE_DURATION;
    private Vector2 chargeVector;
    private Vector2 oldDir;
    private Vector2 targetPosition;
    private List<GameObject> hits = new List<GameObject>();

    private uint buffID;

    private int pathNumber = -1;

    public Ability_Charge()
    {
        abilityName = "Charge";
        maxCooldown = Tuner.BASE_CHARGE_COOLDOWN;
        maxCastRange = Tuner.BASE_CHARGE_MOVEMENT_SPEED * Tuner.BASE_CHARGE_DURATION;
        areaRadius = Tuner.BASE_CHARGE_RADIUS;
        spellBaseAI = Tuner.SpellBaseAI.charge;
    }

    public override float startCast(Vector2 targetPosition)
    {
        checkForParent();
        if (currentCooldown <= 0)
        {
            if (!parent.tag.Equals("Player"))
            {
                this.targetPosition = targetPosition;
                castTime += Tuner.CAST_TIME_EXTRA_FOR_ENEMIES;
            }
            else
                this.targetPosition = CameraScripts.getCurrentMousePos();
            return castTime;
        }

        return 0;
    }

    public override void finishCast()
    {
        chargeVector = (new Vector2(parent.transform.position.x, parent.transform.position.y) - targetPosition).normalized * 200f; //TODO: Make isometric

        chargeTimer = maxChargeTimer;

        buffID = parent.GetComponent<Buffs>().addBuff(Buffs.BuffType.charge, Tuner.BASE_CHARGE_DURATION);

        parent.GetComponent<UnitCombat>().resetAttack();

        pathNumber = parent.GetComponent<AstarAI>().getPathNumber();

        currentCooldown = maxCooldown;
    }

    public override void FixedUpdate()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }

        if (chargeTimer > 0)
        {
            List<GameObject> targets = null;

            if (parent.tag.Equals("Player"))
            {
                targets = UnitList.getHostileUnitsInArea(parent.transform.position, areaRadius);
            }
            else
            {
                targets = UnitList.getPlayerUnitsInArea(parent.transform.position, areaRadius);
            }

            foreach (GameObject target in targets)
            {
                if (!hits.Contains(target))
                {
                    hits.Add(target);
                    parent.GetComponent<UnitCombat>().dealDamage(target, parent.GetComponent<UnitCombat>().calculateDamage(Tuner.DamageType.melee, Tuner.BASE_CHARGE_DAMAGE_MULTIPLIER), Tuner.DamageType.melee);
                }
            }

            Vector2 moveVector = (new Vector2(parent.transform.position.x, parent.transform.position.y) - chargeVector);
            parent.GetComponent<UnitMovement>().moveTo(moveVector, true);
            Vector2 movementDirection = parent.GetComponent<AstarAI>().getMovementDirection();

            if (chargeTimer != maxChargeTimer && oldDir != Vector2.zero)
            {
                float distance = Ellipse.isometricDistance(parent.transform.position, parent.GetComponent<AstarAI>().getNextWaypoint());
                //Debug.Log("Dir: " + movementDirection + " oldDir: " + oldDir + " distance: " + distance + " path: " + pathNumber);
                float angle = Vector2.Angle(oldDir, movementDirection);
                //Debug.Log("Angle: " + angle);

                //Stop charging if about to hit a wall or the turn is too steep
                if (angle > Tuner.CHARGE_MAX_ANGLE || distance < 10f)
                    chargeTimer = 0;
            }
            //Wait for AstarAI to calculate a path for us
            if (pathNumber < parent.GetComponent<AstarAI>().getPathNumber())
            {
                chargeTimer -= Time.fixedDeltaTime;
                if (chargeTimer <= 0)
                {
                    parent.GetComponent<UnitMovement>().stop();
                    parent.GetComponent<Buffs>().removeBuff(buffID);
                    hits.Clear();
                } else
                {
                    oldDir = movementDirection;
                }
            }
        }
    }
}
