using System.CodeDom;
using UnityEngine;
using System.Collections;

public class Tuner : MonoBehaviour
{
	//PLAYER DEFAULT VALUES
    public static readonly float UNIT_BASE_SPEED = 500.0f;

	//UNIT DEFAULT VALUES
	public static readonly int UNIT_BASE_HEALTH = 10;
	public static readonly float UNIT_BASE_RANGED_RANGE = 200f;
	public static readonly float UNIT_BASE_MELEE_RANGE = 20f;

	//SPELL DEFAULT VALUES
	public static readonly float DEFAULT_SPELL_RANGE = 150f;
	public static readonly float DEFAULT_SKILL_CAST_TIME = 1f;
	public static readonly int DEFAULT_SKILL_COOLDOWN = 2 * 10;

	public static readonly float DEFAULT_PROJECTILE_BLAST_RADIUS = 10f;
	public static readonly float DEFAULT_PROJECTILE_VELOCITY = 10f;
	public static readonly int DEFAULT_PROJECTILE_DAMAGE = 10;

	//CAMERA DEFAULT VALUES
	public static readonly float CAMERA_MIN_DISTANCE = 100;
	public static readonly float CAMERA_MAX_DISTANCE = 6000;
    public static readonly float CAMERA_SCROLLING_SPEED = 35.0f;
    public static readonly float CAMERA_ZOOM_SPEED = 15.0f;

    public static readonly float rangedEnemyAttackRange = 750.0f;
    public static readonly float meleeEnemyAttackRange = 150.0f;
    public static readonly float enemyAgroRange = 1000.0f;

    public static readonly float PARTY_SPACING = 100.0f;

    public static readonly float PATHFINDING_MINIMUM_DISTANCE = 20.0f;
}
