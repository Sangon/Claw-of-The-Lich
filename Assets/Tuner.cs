using System.CodeDom;
using UnityEngine;
using System.Collections;

public class Tuner : MonoBehaviour
{
    //GAME SETTINGS DEFAULT VALEUS
    public static readonly int FPS_TARGET_FRAME_RATE = 120;
    public static readonly bool SHOW_HEALTHBARS = true;
    public static readonly int LEVEL_HEIGHT_IN_TILES = 25;
    public static readonly int LEVEL_WIDTH_IN_TILES = 25;

    //PLAYER DEFAULT VALUES
    public static readonly float UNIT_BASE_SPEED = 500f;

	//UNIT DEFAULT VALUES
	public static readonly float UNIT_BASE_HEALTH = 10f;
	public static readonly float UNIT_BASE_RANGED_RANGE = 750f;
	public static readonly float UNIT_BASE_MELEE_RANGE = 22*5f;
    public static readonly float UNIT_BASE_MELEE_DAMAGE = 0.75f;

    //SPELL DEFAULT VALUES
	public static readonly float DEFAULT_SPELL_RANGE = 150f;
	public static readonly float DEFAULT_SKILL_CAST_TIME = 1f;
	public static readonly int DEFAULT_SKILL_COOLDOWN = 2 * 10;

    public static readonly float DEFAULT_PROJECTILE_VELOCITY = 15f;
    public static readonly float DEFAULT_PROJECTILE_DAMAGE = 1.0f;
    public static readonly float DEFAULT_PROJECTILE_RANGE = 30f;

    public static readonly float DEFAULT_PROJECTILE_OFFSET = 100f; //Y-axis, from bottom of the sprite
    public static readonly float DEFAULT_PROJECTILE_HITBOX_RADIUS = 50f;

    public static readonly float DEFAULT_WHIRLWIND_RADIUS = UNIT_BASE_MELEE_RANGE * 2;
    public static readonly float BASE_WHIRLWIND_DAMAGE = UNIT_BASE_MELEE_DAMAGE * 2;

    public static readonly float BASE_CHARGE_DAMAGE = UNIT_BASE_MELEE_DAMAGE * 2;
    public static readonly float BASE_CHARGE_SPEED = UNIT_BASE_MELEE_DAMAGE * 2;

    public static readonly float DEAULT_BLOT_OUT_DURATION = 2 + 2;
    public static readonly float DEFAULT_BLOT_OUT_RADIUS = 440f;
    public static readonly float BASE_BLOT_OUT_DAMAGE = 0.2f;

    public static readonly float DEFAULT_MELEE_ATTACK_CONE_DEGREES = 45f;

    //CAMERA DEFAULT VALUES
    public static readonly float CAMERA_MIN_DISTANCE = 100;
	public static readonly float CAMERA_MAX_DISTANCE = 6000;
    public static readonly float CAMERA_SCROLLING_SPEED = 5f;
    public static readonly float CAMERA_ZOOM_SPEED = 15f;

    //PARTYSYSTEM DEFAULT VALUES
    public static readonly float PARTY_SPACING = 100f;

    //PATHFINDING & MOVEMENT DEFAULT VALUES
    public static readonly float PATHFINDING_MINIMUM_DISTANCE_FROM_UNIT = 10f;
    public static readonly float ATTACKMOVE_MAX_SEARCH_DISTANCE_FROM_CLICK_POINT = 200f;

    //ENEMY AI DEFAULT VALUES
    public static readonly float UNIT_AGGRO_RANGE = 1000f;
    public static readonly float UNIT_AGGRO_CALLOUT_RANGE = 500f; //Enemy units within this range (of the aggroing unit) get aggroed too
    public static readonly float IDLING_STATE_TIME_MIN = 3f; //The minimum time in seconds the enemy spends in idle mode before it wanders
    public static readonly float IDLING_STATE_TIME_MAX = 10f; //The maximum time in seconds the enemy spends in idle mode before it wanders
    public static readonly float WANDERING_DISTANCE_MAX = 200f; //The maximum distance the enemy can wander from its starting position
    public static readonly float CHASING_TIME_MAX = 3f; //The maximum time in seconds the enemy spends chasing the player without seeing him before giving up and returning to its starting position

    //UNITY EDITOR DEFAULT VALUES
    public static readonly int LAYER_OBSTACLES = 1 << 8;
    public static readonly int LAYER_UNITS = 1 << 9;
    public static readonly int LAYER_SELECTION = 1 << 10;
    public static readonly int LAYER_GROUND = 1 << 11;
    public static readonly int LAYER_FLOOR = 1 << 12;
}
