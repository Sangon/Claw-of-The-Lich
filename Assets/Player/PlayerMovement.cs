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
        if (pathfindingTimer <= 0 && Input.GetMouseButton(1))
        {
            unitMovement.stop();
        }
        pathfindingTimer -= Time.fixedDeltaTime;
    }
}
