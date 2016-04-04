using UnityEngine;
using System.Collections;

public class charge_skill : Skill {

    float chargeTimer = 0;
    float maxChargeTimer = 20;
    Vector2 chargeVector;
    GameObject parent;

    public charge_skill()
    {
        spellName = "charge";
        skillIcon = null;
    }

    public override void cast(GameObject unit)
    {
        if (currentCooldown == maxCooldown){
            parent = unit;
            chargeVector = (((new Vector2(unit.transform.position.x, unit.transform.position.y) - getCurrentMousePos()).normalized * (Tuner.BASE_CHARGE_SPEED*2)));

            chargeTimer = maxChargeTimer;
            parent.GetComponent<UnitMovement>().setMovementSpeed(Tuner.UNIT_BASE_SPEED);
            currentCooldown = 0;
            parent.gameObject.GetComponent<UnitMovement>().lockMovement();
        }

    }

    public override void FixedUpdate()
    {
        if (currentCooldown < maxCooldown){
            currentCooldown++;

            if (currentCooldown <= maxCooldown){
                Debug.Log("Ready: " + spellName);
            }

        }

        if (chargeTimer > 0) {

            //TODO: Pistä pelaajan mahdollisuus kävellä johonkin suuntaan lukkoon.

            Vector2 moveVector = (new Vector2(parent.transform.position.x, parent.transform.position.y) - chargeVector);

            chargeTimer--;
            Debug.Log("VECTOR: " + chargeVector + "Charge: " + (chargeVector + new Vector2(parent.transform.position.x, parent.transform.position.y)) + " PARENT POS: " + parent.transform.position);
            if (chargeTimer <= 0)
            {

                parent.GetComponent<UnitMovement>().setMovementSpeed(Tuner.UNIT_BASE_SPEED);
                parent.GetComponent<UnitMovement>().stop();
                parent.gameObject.GetComponent<UnitMovement>().unlockMovement();

            }
            else{
                parent.GetComponent<UnitMovement>().moveTo(moveVector);
            }

        }


    }
}
