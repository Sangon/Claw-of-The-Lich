using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff
{
    public enum Effect
    {
        stun,
        uncontrollable,
        movementspeedlimit,
        movementspeedmultiplier,
        layerorder
    }

    private uint buffID;
    private float duration;
    private float value;
    private Effect effect;

    public float getDuration()
    {
        return duration;
    }

    public float getValue()
    {
        return value;
    }

    public uint getBuffID()
    {
        return buffID;
    }

    public Buff(uint buffID, Effect effect, float duration, float value = 0f)
    {
        this.buffID = buffID;
        this.effect = effect;
        this.duration = duration;
        this.value = value;
    }

    public Effect getEffect()
    {
        return effect;
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
