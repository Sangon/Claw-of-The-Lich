using UnityEngine;
using System.Collections;

public class charge_skill : Skill {

    float chargeTimer = 0;
    float maxChargeTimer = 50;
    Vector2 chargeVector;
    GameObject parent;

    public charge_skill()
    {
        spellName = "charge";
        skillIcon = null;
    }

    public override void cast(GameObject unit)
    {
        if (currentCooldown == maxCooldown)
        {
            parent = unit;
            chargeVector = (((new Vector2(unit.transform.position.x, unit.transform.position.y) - getCurrentMousePos()).normalized * (Tuner.BASE_CHARGE_SPEED)));

            chargeTimer = maxChargeTimer;
            parent.GetComponent<UnitMovement>().setMovementSpeed(Tuner.UNIT_BASE_SPEED * 5);
            currentCooldown = 0;
        }

    }
    public override void FixedUpdate()
    {
        if (currentCooldown < maxCooldown){
            currentCooldown++;
            if (currentCooldown == maxCooldown)
            {
                Debug.Log("Ready: " + spellName);
            }
        }

        if (chargeTimer > 0) {

            //TODO: Pistä pelaajan mahdollisuus kävellä johonkin suuntaan lukkoon.
            chargeTimer--;
            Vector2 moveVector = (new Vector2(parent.transform.position.x, parent.transform.position.y) - chargeVector);
            parent.GetComponent<UnitMovement>().moveTo(moveVector);

            //Debug.Log("VECTOR: " + chargeVector + "Charge: " + (chargeVector + new Vector2(parent.transform.position.x, parent.transform.position.y)));
            if (chargeTimer == 0){
                parent.GetComponent<UnitMovement>().setMovementSpeed(Tuner.UNIT_BASE_SPEED);
                parent.GetComponent<UnitMovement>().stop();
            }

        }


    }
}
