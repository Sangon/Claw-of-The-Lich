using UnityEngine;
using System.Collections;
using System;

public class whirlwind_skill : Skill
{
    public whirlwind_skill()
    {
        spellName = "whirlwind";
        skillIcon = null;
        maxCooldown = Tuner.BASE_WHIRLWIND_COOLDOWN;
    }

    public override void cast(GameObject owner)
    {
        if (currentCooldown <= 0)
        {
            foreach (GameObject g in getUnitsAtPoint(owner.transform.position, Tuner.DEFAULT_WHIRLWIND_RADIUS))
            {
                //Check for line of sight before dealing damage
                if (UnitMovement.lineOfSight(owner.transform.position, g.transform.position, false))
                    g.GetComponent<UnitCombat>().takeDamage(Tuner.BASE_WHIRLWIND_DAMAGE, owner, Tuner.DamageType.melee);
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
