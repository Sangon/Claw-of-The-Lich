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
	private float attackRange;
	private bool attacking = false;

	//TODO: Parempi spellilista
	private Skill[] spellList = new Skill[2];

	void Start () {
		health = Tuner.UNIT_BASE_HEALTH;
		attackRange = Tuner.UNIT_BASE_MELEE_RANGE;

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
			
			if (!inRange(lockedTarget)) {
				GetComponent<UnitMovement>().moveTo(lockedTarget.transform.position);
			}else{
				GetComponent<UnitMovement>().stop();
				startAttack();
			}

		}

		//Melee attackin hahmottelua
		Debug.DrawLine(new Vector3(transform.position.x - 30,transform.position.y - 5,transform.position.z),new Vector3(transform.position.x - 5,transform.position.y - 30,transform.position.z));

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

	public GameObject getClosestTargetToPoint(Vector2 hit){

		List<GameObject> targetList = new List<GameObject>();
		GameObject[] hostileList = GameObject.FindGameObjectsWithTag("Hostile");
		targetList.AddRange (hostileList);

		GameObject target = null;

		float distance = Mathf.Infinity;
		foreach(GameObject g in targetList){
			float currentDistance = Vector3.Distance(g.transform.position,hit);

			if(currentDistance < distance){
				target = g;
				distance = currentDistance;
			}
		}

		return target;
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

	//Hakee etäisyyden tämän ja parametrinä annetun unitin välillä.
	public float getRange(GameObject target){
		return Vector2.Distance(transform.position, lockedTarget.transform.position);
	}

	//Tarkastaa nyt vain jos kyseinen kohde on attack rangen sisällä. Tarkistukset siitä jos vihollinen on liian kaukana enään seuraamiseen pitää tehdä itse.
	public bool inRange(GameObject target){
		//Asettaa vaan ranged unitille pidemmän rangen, LOS tarkistukset voi tehdä vaikka jokasen mobin omassa scriptissä, tai luoda uuden function tänne tarkistusta varten.
		if (target != null) {
			if (Vector2.Distance (transform.position, lockedTarget.transform.position) < attackRange) {
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
