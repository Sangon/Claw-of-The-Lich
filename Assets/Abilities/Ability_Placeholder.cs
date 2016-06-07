using UnityEngine;
using System.Collections;
using System;

public class Ability_Placeholder : Ability
{
    public Ability_Placeholder()
    {
        abilityName = "Placeholder";
        //maxCooldown = Tuner.BASE_WHIRLWIND_COOLDOWN;
        //castTime = Tuner.CAST_TIME_INSTANT;
        //areaRadius = Tuner.BASE_WHIRLWIND_RADIUS;
        //spellBaseAI = Tuner.SpellBaseAI.whirlwind;
    }
    public override float startCast(GameObject parent, Vector2 targetPosition)
    {
        if (currentCooldown <= 0)
        {
            this.parent = parent;
            //if (!parent.tag.Equals("Player"))
                //castTime += Tuner.CAST_TIME_EXTRA_FOR_ENEMIES;
            return castTime;
        }

        return 0;
    }

    public override void finishCast()
    {
        //throw new NotImplementedException();
    }

    public override void FixedUpdate()
    {
    }
}
