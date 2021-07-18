using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace allgameColor
{
    public class CGameColors
    {
        public enum GameColors
        {
            GC_RED = 0,
            GC_BLUE,
            GC_YELLOW,
            GC_GREEN,
            GC_PURPLE,
            GC_ORANGE,
            GC_BROWN,
            GC_DARKGREEN,
            GC_TURQUOISE,
            GC_LIGHTGREEN,
            GC_TOTAL
        }

        public static Color getDefColor(GameColors c)
        {
            switch (c)
            {
                case GameColors.GC_RED:
                    return Color.red;
                case GameColors.GC_BLUE:
                    return Color.blue;
                case GameColors.GC_GREEN:
                    return Color.green;
                case GameColors.GC_YELLOW:
                    return Color.yellow;
                case GameColors.GC_BROWN:
                    return new Color(165f / 255f, 42f / 255f, 42f / 255f);
                case GameColors.GC_PURPLE:
                    return new Color(128f / 255f, 0, 128f / 255f);
                case GameColors.GC_ORANGE:
                    return new Color(1, 52f / 255f, 3f/255f);
                case GameColors.GC_TURQUOISE:
                    return new Color(64f / 255f, 224f / 255f, 208f / 255f);
                case GameColors.GC_DARKGREEN:
                    return new Color(0, 100f / 255f, 0);
                case GameColors.GC_LIGHTGREEN:
                    return new Color(144f / 255f, 238f / 255f, 144f / 255f);
                default:
                    return Color.black;

            }
        }

        public static GameColors mixColor(GameColors first, GameColors second)
        {
            /*
            red + blue - purple
            red + yellow - orange
            red + green - brown
            */
            //Debug.Log("first: " + first + "second: " + second);
            if (first == GameColors.GC_RED && second == GameColors.GC_BLUE
                || first == GameColors.GC_BLUE && second == GameColors.GC_RED)
            {
                return GameColors.GC_PURPLE;
            }
            if (first == GameColors.GC_RED && second == GameColors.GC_YELLOW
                || first == GameColors.GC_YELLOW && second == GameColors.GC_RED)
            {
                return GameColors.GC_ORANGE;
            }
            if (first == GameColors.GC_RED && second == GameColors.GC_GREEN
                || first == GameColors.GC_GREEN && second == GameColors.GC_RED)
            {
                return GameColors.GC_BROWN;
            }
            /*
            blue + yellow - dark green
            blue + green - turquoise
            */
            if (first == GameColors.GC_BLUE && second == GameColors.GC_YELLOW
                || first == GameColors.GC_YELLOW && second == GameColors.GC_BLUE)
            {
                return GameColors.GC_DARKGREEN;
            }
            if (first == GameColors.GC_BLUE && second == GameColors.GC_GREEN
                || first == GameColors.GC_GREEN && second == GameColors.GC_BLUE)
            {
                return GameColors.GC_TURQUOISE;
            }
            // yellow + green - light green
            if (first == GameColors.GC_YELLOW && second == GameColors.GC_GREEN
                || first == GameColors.GC_GREEN && second == GameColors.GC_YELLOW)
            {
                return GameColors.GC_LIGHTGREEN;
            }
            return first;
        }
    }
}

public class GameScript
{
    public const string PLAYER_LIVES = "PlayerLives";
    public const string PLAYER_READY = "IsPlayerReady";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    public const string PLAYER_LOADED_GAME = "PlayerLoadedGame";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}
