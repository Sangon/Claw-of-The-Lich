using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitCombat : MonoBehaviour {


	//Unitin health
	public int health;

	//Targetin seurausta varten.
	public GameObject lockedTarget = null;

	//NYI: Cleavea varten tarvittava kulma.
	private float attackAngle = 0;
	private float meleeRange;
	private bool attacking = false;

	//TODO: Parempi spellilista
	private Skill[] spellList = new Skill[2];

	void Start () {
		health = Tuner.UNIT_BASE_HEALTH;
		meleeRange = Tuner.UNIT_BASE_MELEE_RANGE;

		//TODO: Parempi spellien initialisointi.
		spellList [0] = new projectile_skill();
		spellList [1] = new projectile_skill();

	}

	private int attackTimer = 60;
	void FixedUpdate () {

		if(health <= 0){
			Destroy(gameObject);
		}

		if(lockedTarget != null){
			
			if (!inRange()) {
				GetComponent<UnitMovement> ().moveTo (lockedTarget.transform.position);
			}else{
				startAttack();
			}

		}

		foreach(Skill s in spellList){
			s.FixedUpdate();

		}
	}

	public void attackClosestTargetToPoint(Vector2 hit){
		
		List<GameObject> targetList = new List<GameObject>();

		//Hakee kaikki mobit Hostile tagillä ja lisää ne potentiaalisten vihollisten listaan.
		GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
		targetList.AddRange (hostileList);

		//NYI: bugaa ihan huolella jostaki syystä
		//GameObject[] neutralList = GameObject.FindGameObjectsWithTag("Neutral");
		//targetList.AddRange (neutralList);

		//Laskee kuka potentiaalisten vihollisten listasta on lähimpänä ja lockinnaa siihen.
		float distance = Mathf.Infinity;
		foreach(GameObject g in targetList){
			float currentDistance = Vector3.Distance(g.transform.position,hit);

			if(currentDistance < distance){
				lockedTarget = g;
				distance = currentDistance;
			}

		}
			
	}
		
	public void startAttack(){
		//attackAngle = ((Mathf.Atan2(hit.y - gameObject.transform.position.y, hit.x - gameObject.transform.position.x) + Mathf.PI));
		//Debug.Log ("attackAngle: " + attackAngle);
		attacking = true;
	}

	public void stopAttack(){
		attacking = false;
		attackTimer = 60;
	}

	public bool inRange(){
		
		if (lockedTarget != null) {
			
			if (Vector2.Distance (transform.position, lockedTarget.transform.position) < meleeRange) {
				//GetComponent<UnitMovement>().stop ();
				//TODO: Korjaa pysähtyminen kun ollaan rangessa.
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}

	}

	public void takeDamage(int damage){
		health -= damage;
	}

	public void dealDamage(GameObject enemy, int amount){
		enemy.GetComponent<UnitCombat>().takeDamage(amount);
		
	}

	public void castSpellInSlot(int slot, GameObject unit){
		spellList[slot].cast(unit);
	}

}
