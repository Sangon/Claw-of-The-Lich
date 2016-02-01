using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public GameObject player;
    public GameObject enemy;
    private float timeStamp;
    private float timeStamp2;
    private Vector2 playerPosOld;
    private Vector2 playerPos;
    private Vector2 enemyPos;
    private Vector2 enemyPosOld;

    private UnitMovement unitMovement = null;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
    }

    void FixedUpdate()
    {
        playerPos = player.transform.position;
        
        enemyPos = enemy.transform.position;

        // print(Vector2.Distance(playerPos, enemyPos));
        if (Vector2.Distance(playerPos, enemyPos) < 200 && Vector2.Distance(playerPos, playerPosOld) >10 && timeStamp2 < Time.time)
        {

            unitMovement.moveTo(playerPos);
            timeStamp2 = Time.time + 0.3f;


            playerPosOld = player.transform.position;
        }
        if(Vector2.Distance(playerPos, enemyPos) <= 20.0f)
        {
            enemy.transform.position = playerPos;
            enemy.transform.position = enemyPosOld;

        }
        if(timeStamp < Time.time)
        {
            timeStamp = Time.time + 0.016f;
            enemyPosOld = enemy.transform.position;
            
        }



    }
}
