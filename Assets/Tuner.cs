using System.CodeDom;
using UnityEngine;
using System.Collections;

public class Tuner : MonoBehaviour
{
    //GAME SETTINGS DEFAULT VALEUS
    public static readonly int FPS_TARGET_FRAME_RATE = 1200;
    public static readonly bool SHOW_HEALTHBARS = true;
    public static readonly int LEVEL_WIDTH_IN_TILES = 75;
    public static readonly int LEVEL_HEIGHT_IN_TILES = 75;
    public static readonly int LEVEL_WIDTH_IN_WORLD_UNITS = LEVEL_WIDTH_IN_TILES * 512;
    public static readonly int LEVEL_HEIGHT_IN_WORLD_UNITS = LEVEL_HEIGHT_IN_TILES * 256;
    public static readonly int LEVEL_AREA_DIVISIONS_WIDTH = 4;
    public static readonly int LEVEL_AREA_DIVISIONS_HEIGHT = 4;
    public static readonly int LEVEL_AREA_DIVISIONS = LEVEL_AREA_DIVISIONS_WIDTH * LEVEL_AREA_DIVISIONS_HEIGHT;
    public static readonly int DEFAULT_LAYER_ORDER_UNITS = 1;


    //KEYBOARD CONTROLS
    public static readonly KeyCode[] KEYS_CHARACTER_ABILITY = new KeyCode[8] { KeyCode.Q, KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.E, KeyCode.D, KeyCode.R, KeyCode.F };

    //UNIT DEFAULT COLORS
    public static readonly Color ENEMY_RANGED_COLOR = new Color(0.8f, 0.8f, 0.5f);
    public static readonly Color ENEMY_MELEE_COLOR = new Color(1.0f, 0.5f, 0.5f);

    //UNIT DEFAULT VALUES
    public static readonly float UNIT_BASE_HEALTH = 10f;
    public static readonly float UNIT_BASE_STAMINA = 100f;
    public static readonly float UNIT_BASE_RANGED_RANGE = 750f;
    public static readonly float UNIT_BASE_MELEE_RANGE = 25 * 5f;
    public static readonly float UNIT_BASE_MELEE_DAMAGE = 0.75f;
    public static readonly float UNIT_BASE_RANGED_DAMAGE = 1.5f;
    public static readonly float UNIT_BASE_MELEE_ATTACK_SPEED = 1.33f;
    public static readonly float UNIT_BASE_RANGED_ATTACK_SPEED = 2.0f;
    public static readonly float DEFAULT_MELEE_ATTACK_CONE_DEGREES = 135f;
    public static readonly float UNIT_BASE_SPEED = 500f;

    //MELEE ATTACK DEFAULT VALUES
    public static readonly float KNOCKBACK_DISTANCE = UNIT_BASE_MELEE_RANGE * 0.5f;
    public static readonly float KNOCKBACK_STUN_DURATION = 0.666f;

    //SPELL DEFAULT VALUES
    public static readonly float DEFAULT_SPELL_RANGE = 150f; //?
    public static readonly float DEFAULT_SKILL_CAST_TIME = 1f; //?
    public static readonly float DEFAULT_SKILL_COOLDOWN = 1f; //?

    public static readonly float DEFAULT_PROJECTILE_VELOCITY = 15f;
    public static readonly float DEFAULT_PROJECTILE_DAMAGE = 1.0f;
    public static readonly float DEFAULT_PROJECTILE_RANGE = 30f;

    public static readonly float DEFAULT_PROJECTILE_OFFSET = 100f; //Y-axis, from the bottom of the sprite
    public static readonly float DEFAULT_PROJECTILE_HITBOX_RADIUS = 50f;

    public static readonly float DEFAULT_WHIRLWIND_RADIUS = UNIT_BASE_MELEE_RANGE * 2f;
    public static readonly float BASE_WHIRLWIND_DAMAGE = 10f;
    public static readonly float BASE_WHIRLWIND_COOLDOWN = 5.0f;

    public static readonly float BASE_CHARGE_DAMAGE = 15f;
    public static readonly float BASE_CHARGE_SPEED_MULTIPLIER = 4f;
    public static readonly float BASE_CHARGE_COOLDOWN = 10.0f;
    public static readonly float BASE_CHARGE_DURATION = 0.55f;
    public static readonly float BASE_CHARGE_RADIUS = 150f; //Units that are inside this range are damaged by the charger
    public static readonly float CHARGE_MAX_ANGLE = 90f; //If the charging unit would be turning more than this (in degrees), stop charging

    public static readonly int DEFAULT_BLOT_OUT_DURATION = 3;
    public static readonly float DEFAULT_BLOT_OUT_RADIUS = 440f;
    public static readonly float BASE_BLOT_OUT_DAMAGE = 15f;
    public static readonly float BASE_BLOT_OUT_COOLDOWN = 20.0f;

    //CAMERA DEFAULT VALUES
    public static readonly float CAMERA_MIN_DISTANCE = 100f;
    public static readonly float CAMERA_MAX_DISTANCE = 6000f;
    public static readonly float CAMERA_SCROLLING_SPEED = 5f;
    public static readonly float CAMERA_ZOOM_SPEED = 15f;

    //PARTYSYSTEM DEFAULT VALUES
    public static readonly float PARTY_SPACING = 200f;

    //PATHFINDING & MOVEMENT DEFAULT VALUES
    public static readonly float PATHFINDING_MINIMUM_DISTANCE_FROM_UNIT = 10f;
    public static readonly float ATTACKMOVE_MAX_SEARCH_DISTANCE_FROM_CLICK_POINT = 300f;
    public static readonly float WANDERING_MOVEMENT_SPEED = 250f;

    //ENEMY AI DEFAULT VALUES
    public static readonly float UNIT_AGGRO_RANGE = 1500f;
    public static readonly float UNIT_AGGRO_CALLOUT_RANGE = 1000f; //Enemy units within this range (of the aggroing unit) get aggroed too
    public static readonly float IDLING_STATE_TIME_MIN = 3f; //The minimum time in seconds the enemy spends in idle mode before it wanders
    public static readonly float IDLING_STATE_TIME_MAX = 10f; //The maximum time in seconds the enemy spends in idle mode before it wanders
    public static readonly float WANDERING_DISTANCE_MAX = 1000f; //The maximum distance the enemy can wander from its starting position
    public static readonly float WANDERING_DISTANCE = 750f; //The maximum distance the enemy wanders at a time
    public static readonly float CHASING_TIME_MAX = 3f; //The maximum time in seconds the enemy spends chasing the player without seeing him before giving up and returning to its starting position

    //UNITY EDITOR DEFAULT VALUES
    public static readonly int LAYER_OBSTACLES_INT = 8;
    public static readonly int LAYER_OBSTACLES = 1 << LAYER_OBSTACLES_INT;
    public static readonly int LAYER_UNITS_INT = 9;
    public static readonly int LAYER_UNITS = 1 << LAYER_UNITS_INT;
    public static readonly int LAYER_WATER_INT = 4;
    public static readonly int LAYER_WATER = 1 << LAYER_WATER_INT;
    //public static readonly int LAYER_GROUND_INT = 11;
    //public static readonly int LAYER_GROUND = 1 << LAYER_GROUND_INT;
    public static readonly int LAYER_FLOOR_INT = 12;
    public static readonly int LAYER_FLOOR = 1 << LAYER_FLOOR_INT;

    //DIFFERENT DAMAGE TYPES (used for sounds)
    public enum DamageType
    {
        none,
        melee,
        ranged,
        spell
    }
}
