using System.CodeDom;
using UnityEngine;
using System.Collections;

public class Tuner : MonoBehaviour
{
    public static readonly float UNIT_BASE_SPEED = 100.0f;

	public static readonly int UNIT_BASE_HEALTH = 1000;
	public static readonly float UNIT_BASE_MELEE_RANGE = 20f;

	public static readonly float CAMERA_MIN_DISTANCE = 75;
	public static readonly float CAMERA_MAX_DISTANCE = 225;

    public static readonly float rangedEnemyAttackRange = 150.0f;
    public static readonly float meleeEnemyAttackRange = 20.0f;
    public static readonly float enemyAgroRange = 300.0f;
}
