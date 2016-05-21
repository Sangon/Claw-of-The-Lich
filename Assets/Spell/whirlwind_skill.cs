using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class whirlwind_skill : Skill
{
    public whirlwind_skill()
    {
        spellName = "whirlwind";
        skillIcon = null;
        maxCooldown = Tuner.BASE_WHIRLWIND_COOLDOWN;
    }

    public override void cast(GameObject parent)
    {
        if (currentCooldown <= 0)
        {
            this.parent = parent;

            List<GameObject> targets = null;

            if (parent.tag.Equals("Player"))
            {
                targets = UnitList.getHostileUnitsInArea(parent.transform.position, Tuner.BASE_CHARGE_RADIUS);
            }
            else
            {
                targets = UnitList.getPlayerUnitsInArea(parent.transform.position, Tuner.DEFAULT_WHIRLWIND_RADIUS);
            }
            foreach (GameObject unit in targets)
            {
                //Check for line of sight before dealing damage
                if (UnitMovement.lineOfSight(parent.transform.position, unit.transform.position, false))
                    unit.GetComponent<UnitCombat>().takeDamage(Tuner.BASE_WHIRLWIND_DAMAGE, parent, Tuner.DamageType.melee);
            }
            currentCooldown = maxCooldown;
        }
    }

    public override void FixedUpdate()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }
    }
}
