using System.CodeDom;
using UnityEngine;
using System.Collections;

public class Tuner : MonoBehaviour
{
    //GAME LOGIC DEFAULT VALEUS
    public static readonly int FPS_TARGET_FRAME_RATE = 120;

	//PLAYER DEFAULT VALUES
    public static readonly float UNIT_BASE_SPEED = 500f;

	//UNIT DEFAULT VALUES
	public static readonly float UNIT_BASE_HEALTH = 10f;
	public static readonly float UNIT_BASE_RANGED_RANGE = 750f;
	public static readonly float UNIT_BASE_MELEE_RANGE = 22*5f;

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
    public static readonly float CAMERA_SCROLLING_SPEED = 35f;
    public static readonly float CAMERA_ZOOM_SPEED = 15f;

    public static readonly float rangedEnemyAttackRange = 750f;
    public static readonly float meleeEnemyAttackRange = 150f;
    public static readonly float enemyAggroRange = 1000f;

    public static readonly float PARTY_SPACING = 100f;

    public static readonly float PATHFINDING_MINIMUM_DISTANCE = 20f;

    public static readonly int LAYER_OBSTACLES = 1 << 8;
    public static readonly int LAYER_UNITS = 1 << 9;

    public static readonly bool SHOW_HEALTHBARS = true;
}
