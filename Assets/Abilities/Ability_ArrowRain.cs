using UnityEngine;
using System.Collections;

public class Ability_ArrowRain : Ability
{
    public Ability_ArrowRain()
    {
        abilityName = "ArrowRain";
        maxCooldown = Tuner.BASE_BLOT_OUT_COOLDOWN;
        maxCastRange = Tuner.BASE_BLOT_OUT_CAST_RANGE;
        castTime = Tuner.BASE_BLOT_OUT_CAST_TIME;
        areaRadius = Tuner.BASE_BLOT_OUT_RADIUS;
        spellBaseAI = Tuner.SpellBaseAI.arrowRain;
        targeted = true;
    }

    public override float startCast(Vector2 targetPosition)
    {
        checkForParent();
        if (currentCooldown <= 0)
        {
            castPosition = targetPosition;
            return castTime;
        }

        return 0;
    }

    public override void finishCast()
    {
        GameObject go = Instantiate(Resources.Load("Ability Prefabs/Spawner_ArrowRain"), castPosition, Quaternion.identity) as GameObject;
        go.GetComponent<Ability_ArrowRain_Spawner>().setParent(parent);
        go.GetComponent<Ability_ArrowRain_Spawner>().setAreaRadius(areaRadius);
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
