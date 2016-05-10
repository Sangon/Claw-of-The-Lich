using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buffs : MonoBehaviour
{
    public enum BuffType
    {
        knockback
    }

    private List<Buff> buffs = null;

    void Start()
    {
        buffs = new List<Buff>();
    }

    public bool isStunned()
    {
        foreach (Buff b in buffs)
        {
            if (b.hasEffect(Buff.Effect.stun))
                return true;
        }
        return false;
    }

    public void addBuff(BuffType buffType, float duration)
    {
        switch(buffType)
        {
            case BuffType.knockback:
                List<Buff.Effect> effects = new List<Buff.Effect>();
                effects.Add(Buff.Effect.stun);
                buffs.Add(new Buff(effects, duration));
                break;
        }
    }

    void FixedUpdate()
    {
        if (isStunned())
        {
            gameObject.GetComponent<UnitCombat>().stopAttack();
            gameObject.GetComponent<UnitMovement>().stop(true);
            //print("Stunned: " + transform.name);
        }
        buffs.RemoveAll(b => b.tick() == true);
    }
}
