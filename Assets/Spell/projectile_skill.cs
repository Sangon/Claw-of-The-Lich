using UnityEngine;
using System.Collections;

public class projectile_skill : Skill {

    private int spellOffSet = 155;

	public projectile_skill(){
		spellName = "testSpell";
		skillIcon = null;
	}

	public override void cast (GameObject unit){
		
		if (currentCooldown == maxCooldown) {
			Instantiate (Resources.Load (spellName), unit.transform.position + new Vector3(0,spellOffSet,0), Quaternion.identity);
			currentCooldown = 0;
		}

	}
	public override void FixedUpdate(){

        if (currentCooldown < maxCooldown)
        {
            currentCooldown++;
            if (currentCooldown == maxCooldown)
            {
                Debug.Log("Ready: " + spellName);
            }
        }
    }

}
