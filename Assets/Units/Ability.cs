using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Ability : ScriptableObject
{
    protected float maxCooldown = Tuner.DEFAULT_SKILL_COOLDOWN;
    protected float maxCastRange = Tuner.DEFAULT_SKILL_CAST_RANGE;
    protected float areaRadius = Tuner.DEFAULT_SKILL_RADIUS;
    protected float currentCooldown = 0;
    protected float castTime = Tuner.DEFAULT_SKILL_CAST_TIME;
    protected Vector2 castPosition;
    protected Tuner.SpellBaseAI spellBaseAI = Tuner.SpellBaseAI.selfHeal;
    protected string abilityName = "Placeholder";
    protected GameObject parent;

    //Jokanen spelli toteuttaa oman casti ja metodinsa.
    public abstract float startCast(GameObject parent, Vector2 targetPosition);
    public abstract void finishCast();
    public abstract void FixedUpdate();

    public Tuner.SpellBaseAI getSpellBaseAI()
    {
        return spellBaseAI;
    }

    public float getCurrentCooldown()
    {
        return currentCooldown;
    }

    public void setCurrentCooldown(float value)
    {
        currentCooldown = value;
    }

    public bool isOnCooldown()
    {
        if (currentCooldown > 0)
            return true;
        return false;
    }

    public float getMaxCooldown()
    {
        return maxCooldown;
    }

    public float getMaxCastRange()
    {
        return maxCastRange;
    }

    public float getAreaRadius()
    {
        return areaRadius;
    }

    public string getAbilityName()
    {
        return abilityName;
    }
}

