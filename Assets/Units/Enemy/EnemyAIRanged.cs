using UnityEngine;
using System.Collections;

public class EnemyAIRanged : MonoBehaviour
{


    public GameObject player;
    public GameObject enemy;

    private bool directLine;

    private float timeStamp;

    private Vector2 playerPos;
    private Vector2 enemyPos;
    private Vector2 enemyPosOld;

    private UnitMovement unitMovement = null;

    void Start()
    {
        unitMovement = GetComponent<UnitMovement>();
        directLine = false;
    }

    void FixedUpdate()
    {
        playerPos = player.transform.position;
        enemyPos = enemy.transform.position;

        RaycastHit2D hit = Physics2D.Linecast(enemyPos, playerPos, 1 << 9);
        if (hit.collider == null)
        {
            directLine = true;
        }
        else
        {
            directLine = false;
        }

        if (Vector2.Distance(playerPos, enemyPos) <= Tuner.rangedEnemyAttackRange && directLine == true)
        {
            //enemy.transform.position = enemyPosOld;
            unitMovement.stop();

        }
        else if (Vector2.Distance(playerPos, enemyPos) < Tuner.enemyAggroRange && timeStamp < Time.time)
        {
            unitMovement.moveTo(playerPos);
            timeStamp = Time.time + 0.3f;
        }

        enemyPosOld = enemy.transform.position;
    }
}

