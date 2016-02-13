using UnityEngine;
using System.Collections;

public class EnemyAIMelee : MonoBehaviour
{

    public GameObject player;
    public GameObject enemy;

    private float timeStamp;

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
        if (Vector2.Distance(playerPos, enemyPos) < Tuner.enemyAgroRange && timeStamp < Time.time)
        {
            unitMovement.moveTo(playerPos);
            timeStamp = Time.time + 0.3f;
        }

        if (Vector2.Distance(playerPos, enemyPos) <= Tuner.meleeEnemyAttackRange)
        {
            unitMovement.stop();
        }
    }
}
