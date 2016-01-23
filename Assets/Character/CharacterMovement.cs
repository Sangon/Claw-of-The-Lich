using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    public GameObject player;
    float pathfindingTimer = 0;

    void FixedUpdate()
    {
        //Vector2 playerPos = player.transform.position;
        //Vector2 playerPosOld = player.transform.position;

        if (pathfindingTimer <= 0 && Input.GetMouseButton(0))
        {
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);
            if (hit.collider != null)
            {
                player.GetComponent<AstarAI>().move(hit.point);
                //pathfindingTimer += 0.10f;
            }
        }
        //pathfindingTimer -= Time.fixedDeltaTime;
    }
}
