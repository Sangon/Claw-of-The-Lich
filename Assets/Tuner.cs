using System.CodeDom;
using UnityEngine;
using System.Collections;

public class Tuner : MonoBehaviour
{
	//PLAYER DEFAULT VALUES
    public static readonly float UNIT_BASE_SPEED = 100.0f;

	//UNIT DEFAULT VALUES
	public static readonly int UNIT_BASE_HEALTH = 10;
	public static readonly float UNIT_BASE_RANGED_RANGE = 200f;
	public static readonly float UNIT_BASE_MELEE_RANGE = 20f;

	//SPELL DEFAULT VALUES
	public static readonly float DEFAULT_SPELL_RANGE = 150f;
	public static readonly float DEFAULT_SKILL_CAST_TIME = 1f;
	public static readonly int DEFAULT_SKILL_COOLDOWN = 2 * 50;

	public static readonly float DEFAULT_PROJECTILE_BLAST_RADIUS = 10f;
	public static readonly float DEFAULT_PROJECTILE_VELOCITY = 2.5f;
	public static readonly int DEFAULT_PROJECTILE_DAMAGE = 10;

	//CAMERA DEFAULT VALUES
	public static readonly float CAMERA_MIN_DISTANCE = 75;
	public static readonly float CAMERA_MAX_DISTANCE = 175;

	public static readonly float CAMERA_MAX_DISTANCE = 225;

    public static readonly float rangedEnemyAttackRange = 150.0f;
    public static readonly float meleeEnemyAttackRange = 20.0f;
    public static readonly float enemyAgroRange = 300.0f;
}
