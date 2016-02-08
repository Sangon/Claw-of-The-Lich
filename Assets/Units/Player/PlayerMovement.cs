using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    float pathfindingTimer = 0;
    private UnitMovement unitMovement = null;
	private UnitCombat unitCombat = null;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
		unitCombat = GetComponent<UnitCombat>();
    }

    // Update is called once per frame
    void FixedUpdate () {

		//Hakee hiiren kohdan world spacessa.
		Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

		//Hiiren vasen nappi.
        if (pathfindingTimer <= 0 && Input.GetMouseButton(0)){



			//Pysäyttää hahmon ja lyö ilmaa jos vasen shift on pohjassa, muuten liikkuu kohteeseen.
			if(Input.GetKey(KeyCode.LeftShift)){
				unitCombat.startAttack();
			}else{
				//Liikkuu hiiren kohtaan.
	            if (hit.collider != null){
	                unitMovement.moveTo(hit.point);
	                pathfindingTimer = 0.1f;
	            }
			}
        }


		//Hiiren oikea nappi.
        if (pathfindingTimer <= 0 && Input.GetMouseButton(1)){
            //unitMovement.stop();

			//TODO: Etsii lähimmän kohteen ja lockkaa siihen.

			unitCombat.attackClosestTargetToPoint (hit.point);
        }
        pathfindingTimer -= Time.fixedDeltaTime;


		//Rullaa kameraa kauemmas.
		if(Input.GetAxis("Mouse ScrollWheel") < 0){
			Camera.main.orthographicSize += 10;
		}
		//Rulla kameraa lähemmäs.
		if(Input.GetAxis("Mouse ScrollWheel") > 0){
			Camera.main.orthographicSize -= 10;
		}
		//Rajoittaa kameran max- ja minimietäisyydet.
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize,Tuner.CAMERA_MIN_DISTANCE, Tuner.CAMERA_MAX_DISTANCE);


		//////////////////////////////////////
		/// SPELLIT
		/////////////////////////////////////

		if(Input.GetKeyDown(KeyCode.Q)){
			GetComponent<UnitCombat> ().castSpellInSlot(0,hit.point,gameObject);
		}

		if(Input.GetKeyDown(KeyCode.W)){}

		if(Input.GetKeyDown(KeyCode.E)){}

		if(Input.GetKeyDown(KeyCode.A)){}

		if(Input.GetKeyDown(KeyCode.S)){}

		if(Input.GetKeyDown(KeyCode.D)){}

    }


}
