using UnityEngine;
using System.Collections;

public class projectile_skill :  Skill{

	public projectile_skill(){
		spellName = "testSpell";
	}

	public override void cast (Vector2 point, GameObject unit){
		Instantiate(Resources.Load(spellName),unit.transform.position, Quaternion.identity);
	}


}
