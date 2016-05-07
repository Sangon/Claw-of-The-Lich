using UnityEngine;
using System.Collections;

public class charge_skill : Skill
{
    float chargeTimer = 0;
    float maxChargeTimer = 20;
    Vector2 chargeVector;
    GameObject parent;

    public charge_skill()
    {
        spellName = "charge";
        skillIcon = null;
    }

    public override void cast(GameObject owner)
    {
        if (currentCooldown == maxCooldown)
        {
            parent = owner;

            chargeVector = new Vector2(parent.transform.position.x, parent.transform.position.y) - getCurrentMousePos();
            chargeTimer = maxChargeTimer;

            parent.GetComponent<UnitMovement>().setMovementSpeed(parent.GetComponent<UnitCombat>().getBaseMovementSpeed() * 5);
            currentCooldown = 0;
        }
    }
    public override void FixedUpdate()
    {
        if (currentCooldown < maxCooldown)
        {
            currentCooldown++;
            /*
            if (currentCooldown == maxCooldown)
            {
                Debug.Log("Ready: " + spellName);
            }
            */
        }

        if (chargeTimer > 0)
        {
            //TODO: Pistä pelaajan mahdollisuus kävellä johonkin suuntaan lukkoon.
            chargeTimer--;
            Vector2 moveVector = (new Vector2(parent.transform.position.x, parent.transform.position.y) - chargeVector);
            parent.GetComponent<UnitMovement>().moveTo(moveVector);

            //Debug.Log("VECTOR: " + chargeVector + "Charge: " + moveVector);

            if (chargeTimer == 0)
            {
                parent.GetComponent<UnitMovement>().setMovementSpeed(parent.GetComponent<UnitCombat>().getBaseMovementSpeed());
                parent.GetComponent<UnitMovement>().stop();
            }
        }
    }
}
