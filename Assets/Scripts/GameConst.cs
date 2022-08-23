public static class GameConst
{
    /// <summary>
    /// Time for each move of unit. If unit move 3 cell, total time needed is x3
    /// </summary>
    public const float k_gridUnit_move_item = 0.25f;
    public const float k_time_delay_to_disable_gift_box = 3.5f;

    public const int k_max_opponent_per_turn = 3; //maximum opponent can spawn each turn;
    public const int k_max_level_opponent = 15; //maximum level of opponent can spawn each turn;
    public const int k_max_level_ally_spawn = 8; //maximum level of ally can spawn each turn;
    public const int k_max_level_skill_passive = 20; //maximum level of skill can reach (some skills may less)



    public const string k_LEADERBOARD_PRIVATE_CODE = "fxcJlwJGYUa_LzW_4sEIHwVpG-CKpI8kCxJu7T1WL-mw";
    public const string k_LEADERBOARD_PUBLIC_CODE = "5d6739f0e6a81b07f016e034";

    public const string k_LEADERBOARD_URL = "http://dreamlo.com/lb/";
}

////////////////////////////////////////////////////////////////////////////////
public enum EGameState
{
    PREPARE,
    BATTLE,
    PAUSE,
    STOPPED
};

public enum EDirection
{
    Left,
    Right,
    Up,
    Down
}