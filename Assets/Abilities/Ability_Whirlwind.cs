using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Ability_Whirlwind : Ability
{
    public Ability_Whirlwind()
    {
        abilityName = "Whirlwind";
        maxCooldown = Tuner.BASE_WHIRLWIND_COOLDOWN;
        castTime = Tuner.CAST_TIME_INSTANT;
        areaRadius = Tuner.BASE_WHIRLWIND_RADIUS;
        spellBaseAI = Tuner.SpellBaseAI.whirlwind;
    }

    public override float startCast(GameObject parent, Vector2 targetPosition)
    {
        if (currentCooldown <= 0)
        {
            this.parent = parent;
            if (!parent.tag.Equals("Player"))
                castTime += Tuner.CAST_TIME_EXTRA_FOR_ENEMIES;
            return castTime;
        }

        return 0;
    }

    public override void finishCast()
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
            //Check for line of sight before dealing damage
            if (UnitMovement.lineOfSight(parent.transform.position, target.transform.position, false))
                target.GetComponent<UnitCombat>().takeDamage(parent.GetComponent<UnitCombat>().calculateDamage(Tuner.DamageType.melee, Tuner.BASE_WHIRLWIND_DAMAGE_MULTIPLIER), parent, Tuner.DamageType.melee);
        }
        currentCooldown = maxCooldown;
    }

    public override void FixedUpdate()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }
    }
}
