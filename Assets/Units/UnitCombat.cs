using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitCombat : MonoBehaviour {


	//Unitin health
	public int health;

	//Targetin seurausta varten.
	public GameObject lockedTarget = null;

	private float attackAngle = 0;

	private float meleeRange;

	private bool attacking = false;
	private Skill[] spellList = new Skill[2];

	//TODO: Parempi spellilista


	void Start () {
		health = Tuner.UNIT_BASE_HEALTH;
		meleeRange = Tuner.UNIT_BASE_MELEE_RANGE;
		spellList [0] = new projectile_skill ();
	}

	int attackTimer = 60;
	void FixedUpdate () {
		
		if(lockedTarget != null && !inRange()){
			GetComponent<UnitMovement> ().moveTo (lockedTarget.transform.position);
		}

		if (attacking)
			attackTimer--;

		if(attackTimer >= 30 && attackTimer <= 40){
			attack();
		}else{
		}

		if (attackTimer <= 0) {
			stopAttack ();

		}
	}

	public void attackClosestTargetToPoint(Vector2 hit){
		
		List<GameObject> targetList = new List<GameObject>();

		GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
		//GameObject[] neutralList = GameObject.FindGameObjectsWithTag("Neutral");


		targetList.AddRange (hostileList);
	//	targetList.AddRange (neutralList);

		float distance = Mathf.Infinity;

		foreach(GameObject g in targetList){
			float currentDistance = Vector3.Distance(g.transform.position,hit);

			if(currentDistance < distance){
				lockedTarget = g;
				distance = currentDistance;
			}
		}
			
	}


	public void attack(){
		//GetComponent<SpriteRenderer> ().color = new Color (0,0,0);
		//TODO: lyömisanimaatio
		//TODO: Etsi viholliset (Eri factionissa olevat) hyökätyssä suunnassa ja tee niihin damagea
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
				startAttack();
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

	public void castSpellInSlot(int slot, Vector2 point, GameObject unit){
		spellList[slot].cast(point,unit);
	}

}
