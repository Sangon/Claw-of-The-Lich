using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buffs : MonoBehaviour
{
    public enum BuffType
    {
        knockback,
        charge,
        attack,
        wander,
        layerOrder,
        casting
    }

    private uint nextBuffID;
    private List<Buff> buffs = null;

    void Awake()
    {
        buffs = new List<Buff>();
    }

    public bool isStunned()
    {
        foreach (Buff b in buffs)
        {
            if (b.getEffect() == Buff.Effect.stun)
                return true;
        }
        return false;
    }

    public int getLayerOrder()
    {
        int order = Tuner.DEFAULT_LAYER_ORDER_UNITS;
        foreach (Buff b in buffs)
        {
            if (b.getEffect() == Buff.Effect.layerOrder)
                order = (int)b.getValue();
        }
        return order;
    }

    public bool isUncontrollable()
    {
        foreach (Buff b in buffs)
        {
            if (b.getEffect() == Buff.Effect.uncontrollable)
                return true;
        }
        return false;
    }

    public float getMovementSpeedMultiplier()
    {
        float lowest = -1f;
        foreach (Buff b in buffs)
        {
            if (b.getEffect() == Buff.Effect.movementSpeedMultiplier)
                if (lowest == -1f || (lowest != -1f && b.getValue() < lowest))
                    lowest = b.getValue();
        }
        return lowest;
    }

    public float getMovementSpeedConstant()
    {
        float lowest = -1f;
        foreach (Buff b in buffs)
        {
            if (b.getEffect() == Buff.Effect.movementSpeedConstant)
                if (lowest == -1f || (lowest != -1f && b.getValue() < lowest))
                    lowest = b.getValue();
        }
        return lowest;
    }

    public void removeBuff(uint buffID)
    {
        buffs.RemoveAll(b => b.getBuffID() == buffID);
    }

    public uint addBuff(BuffType buffType, float duration)
    {
        nextBuffID++;
        switch (buffType)
        {
            case BuffType.knockback:
                buffs.Add(new Buff(nextBuffID, Buff.Effect.stun, duration));
                break;
            case BuffType.attack:
                break;
            case BuffType.charge:
                buffs.Add(new Buff(nextBuffID, Buff.Effect.uncontrollable, duration));
                buffs.Add(new Buff(nextBuffID, Buff.Effect.movementSpeedConstant, duration, Tuner.BASE_CHARGE_MOVEMENT_SPEED));
                break;
            case BuffType.wander:
                buffs.Add(new Buff(nextBuffID, Buff.Effect.movementSpeedConstant, duration, Tuner.WANDERING_MOVEMENT_SPEED));
                break;
            case BuffType.layerOrder:
                buffs.Add(new Buff(nextBuffID, Buff.Effect.layerOrder, duration, Tuner.DEFAULT_LAYER_ORDER_UNITS - 1));
                break;
            case BuffType.casting:
                buffs.Add(new Buff(nextBuffID, Buff.Effect.uncontrollable, duration));
                break;
        }
        return nextBuffID;
    }

    public void addDuration(uint buffID, float extraDuration)
    {
        foreach (Buff b in buffs)
        {
            if (b.getBuffID() == buffID)
            {
                b.addDuration(extraDuration);
                break;
            }
        }
    }

    void FixedUpdate()
    {
        UnitCombat unitCombat = gameObject.GetComponent<UnitCombat>();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = getLayerOrder();
        if (isStunned())
        {
            unitCombat.stopAttack();
            gameObject.GetComponent<UnitMovement>().stop();
        }
        float movementSpeedConstant = getMovementSpeedConstant();
        if (movementSpeedConstant != -1f)
        {
            unitCombat.setMovementSpeed(movementSpeedConstant);
        }
        else
        {
            float desiredMovementSpeed = unitCombat.getBaseMovementSpeed();
            float multiplier = getMovementSpeedMultiplier();

            if (multiplier != -1f)
                desiredMovementSpeed *= multiplier;

            unitCombat.setMovementSpeed(desiredMovementSpeed);
        }
        buffs.RemoveAll(b => b.tick() == true);
    }
}
