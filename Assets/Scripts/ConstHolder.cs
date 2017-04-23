using UnityEngine;

public class ConstHolder : MonoBehaviour
{
    public const float      MUTATION_RATE = 0.1f;
    public const float      MAX_PERPETUATION = 0.3f;
    public const float      LEGEND_SPAWN_CHANCE = 0.65f;
    public const float      CROSSOVER_CHANCE = 1.0f;

    public const int        MAX_ALPHAS = 2;
    public const int        MAX_TOP_GENOMES = 3;

    //Unit consts
    public const float      UNIT_MIN_SPEED = 30.0f;
    public const float      UNIT_MAX_SPEED = 60.0f;
    public const float      UNIT_MAX_ROTATION = 180.0f;

    public const float      UNIT_LINE_OF_SIGHT = 45.0f;

    //transition inside menu
    public const float      TRANSITION_ANIMATION = 0.5f;

    //camera
    public const float      CAMERA_SCROLL_SPEED = 50.0f;

    public const float      CAMERA_MIN_ORTHOGRAPHIC_SIZE = 5.0f;
    public const float      CAMERA_MAX_ORTHOGRAPHIC_SIZE = 25.0f;

    //camera and spot light
    public const float      SWITCH_TARGET_TIMER = 0.1f;
    public const float      TRANSITION_TIMER = 1.0f;

    //world generation
    public const int        MIN_OBSTACLES = 1;
    public const int        MAX_OBSTACLES = 2;
    public const int        MAX_YELLOWBALLS = 10;
    public const int        MAX_AGENTS = 11;

    //unit positions
    public const float      INITIAL_POSITION_X = 6.5f;
    public const float      INITIAL_POSITION_Y = 0.0f;
    public const float      INITIAL_POSITION_Z = 5.0f;

    //unit angle
    public const float      INITIAL_ANGLE_X = 0;
    public const float      INITIAL_ANGLE_Y = 270;
    public const float      INITIAL_ANGLE_Z = 0;

    //spacing and last position
    public const float      END_POSITION_X = 40.0f + INITIAL_POSITION_X;
    public const float      SPACING_BETWEEN_AGENTS = (END_POSITION_X - INITIAL_POSITION_X) / (MAX_AGENTS-1);

    //time until evolution.
    public const float      TIME_TILL_EVOLUTION = 180.0f;
    public const float      TIME_TILL_MAX_DIFFICULTY = 60.0f;
    public const float      TIME_TILL_GAME_STARTS = 3.0f;
}
