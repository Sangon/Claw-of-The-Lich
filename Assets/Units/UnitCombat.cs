using UnityEngine;
using System.Collections;

public class UnitCombat : MonoBehaviour {


	//Unitin health
	public int health;

	//Targetin seurausta varten.
	public GameObject lockedTarget = null;

	private float attackAngle = 0;

	private float meleeRange;

	void Start () {
		health = Tuner.UNIT_BASE_HEALTH;
		meleeRange = Tuner.UNIT_BASE_MELEE_RANGE;
	}

	void FixedUpdate () {
		if(lockedTarget != null && !inRange()){
			GetComponent<UnitMovement> ().moveTo (lockedTarget.transform.position);
		}
	}

	public void attackClosestTargetToPoint(Vector2 hit){
		GameObject[] targetList = GameObject.FindGameObjectsWithTag("Hostile");
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
		//TODO: lyömisanimaatio
		//TODO: Etsi viholliset (Eri factionissa olevat) hyökätyssä suunnassa ja tee niihin damagea
	}

	public void startAttack(Vector2 hit){
		attackAngle = ((Mathf.Atan2(hit.y - gameObject.transform.position.y, hit.x - gameObject.transform.position.x) + Mathf.PI));
		Debug.Log ("attackAngle: " + attackAngle);
	}

	public bool inRange(){
		if (lockedTarget != null) {
			if (Vector2.Distance (transform.position, lockedTarget.transform.position) < meleeRange) {
				GetComponent<UnitMovement> ().stop ();
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

}
