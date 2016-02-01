using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    float pathfindingTimer = 0;
    private UnitMovement unitMovement = null;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
    }

    // Update is called once per frame
    void FixedUpdate () {
		//Hiiren vasen nappi.
        if (pathfindingTimer <= 0 && Input.GetMouseButton(0))
        {
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
            if (hit.collider != null)
            {
                unitMovement.moveTo(hit.point);
                pathfindingTimer = 0.1f;
            }
        }

		//Hiiren oikea nappi.
        if (pathfindingTimer <= 0 && Input.GetMouseButton(1))
        {
            unitMovement.stop();
        }
        pathfindingTimer -= Time.fixedDeltaTime;


		//////////////////////////////////////
		/// SPELLIT
		/////////////////////////////////////

		if(Input.GetKeyDown(KeyCode.Q)){
			Debug.Log("Q PRESSED");
		}

		if(Input.GetKeyDown(KeyCode.W)){}

		if(Input.GetKeyDown(KeyCode.E)){}

		if(Input.GetKeyDown(KeyCode.A)){}

		if(Input.GetKeyDown(KeyCode.S)){}

		if(Input.GetKeyDown(KeyCode.D)){}

    }

}
