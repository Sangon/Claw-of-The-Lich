using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff
{
    public enum Effect
    {
        stun
    }

    private float duration;
    private List<Effect> effects = null;

    public float getDuration()
    {
        return duration;
    }

    public Buff(List<Effect> effects, float duration)
    {
        this.effects = effects;
        this.duration = duration;
    }

    public bool hasEffect(Effect effect)
    {
        foreach (Effect e in effects)
        {
            if (e == effect)
                return true;
        }
        return false;
    }

    public bool tick()
    {
        duration -= Time.fixedDeltaTime;
        if (duration <= 0)
        {

            return true;
        }
        return false;
    }
}
