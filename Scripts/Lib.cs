using Godot;
using System;

public class Lib
{

    public struct Vec2I
    {

        public int x;
        public int y;

        public Vec2I(int _x = 0, int _y = 0)
        {
            x = _x;
            y = _y;
        }

    }

    public struct MapTile
    {

        public Vec2I pos;
        public int cell;
        public bool topFloor;

        public MapTile(Vec2I _pos = new Vec2I(), int _cell = -1, bool _topFloor = false)
        {
            pos = _pos;
            cell = _cell;
            topFloor = _topFloor;
        }

    }

    public struct WeaponObj
    {

        private Color color;
        private float power;
        public WeaponObj(Color _color = new Color(), float _power = 0.5f) // ?? new color ?? (?? black ??)
        {
            color = _color;
            power = _power;
        }

        public void AddColor(Color c)
        {
            float x = Mathf.Min(power, MAX_W_MIX_C - 1.0f);
            float y = Mathf.Min(power + 1, MAX_W_MIX_C);
            color.r = (color.r * x + c.r) / y;
            color.g = (color.g * x + c.g) / y;
            color.b = (color.b * x + c.b) / y;
            power += 1.0f;
        }

        public Color GetColor()
        {
            color.a = 1.0f;
            return NormWColor(color);
        }

        public int GetWType()
        {
            int x = (int)WEAPON_PER_COLOR_C;
            if (x <= 1)
            {
                return 0;
            }
            return Mathf.RoundToInt(color.r / ((float)(x - 1))) + x * Mathf.RoundToInt(color.g / ((float)(x - 1))) + x * x * Mathf.RoundToInt(color.b / ((float)(x - 1)));
        }

        public void StoreToFile(File file)
        {
            if (file == null)
            {
                GD.Print("Weapon obj store file is null.");
                return;
            }
            try
            {
                file.StoreFloat(power);
                file.StoreFloat(color.r);
                file.StoreFloat(color.g);
                file.StoreFloat(color.b);
            }
            catch
            {
                GD.Print("Store Weapon obj error.");
            }
        }

        public void GetFromFile(File file)
        {
            if (file == null)
            {
                GD.Print("Weapon obj get file is null.");
                return;
            }
            try
            {
                power = file.GetFloat();
                color.r = file.GetFloat();
                color.g = file.GetFloat();
                color.b = file.GetFloat();
            }
            catch
            {
                GD.Print("Load Weapon Obj error.");
            }
        }

    }

    public const string SAVE_FILE_NAME = "save";
    public const float MAX_W_MIX_C = 3.5f;
    public const float GEN_WALL_C = 0.06f; 
    public const float HOUSE_GEN_C = 0.7f; 
    public const int GAME_M_PANEL = -1;
    public const int COLLECTION_M_PANEL = 0;
    public const int WEAPON_M_PANEL = 1;
    public const int BATTLE_M_PANEL = 2;
    public const int START_M_PANEL = BATTLE_M_PANEL;
    public const int WEAPON_NUM = 5;
    public const int HOUSE_W = 0;
    public const int TANK_W = 1;
    public const int TRUCK_W = 2;
    public const int DRONE_W = 3;
    public const int BULLDOZER_W = 4;
    public const int WALL_W = 5;
    public const int CRANE_W = 6;
    public const int VEHICLE_W = 7;
    public const int START_ZONE_DIST = 3;
    public const int STARS_NUM = 3;
    public const int VEHICLE_SCORE_C = 4;
    public const int HELP_IMAGES_N = 5;
    public const uint D_NUM = 4;
    public const uint P_HOUSE_TILE = 8;
    public const uint E_HOUSE_TILE = 9; 
    public const uint P_WEAPON_TILE = 14;
    public const uint E_WEAPON_TILE = 15;
    public const uint BLOCKED_TILE = 4;
    public const uint WALL_TILE = 2;
    public const uint GAME_TURNS_NUM = 10;
    public const uint WEAPON_PER_COLOR_C = 2;
    public const uint WEAPON_TYPES_NUM = WEAPON_PER_COLOR_C * WEAPON_PER_COLOR_C * WEAPON_PER_COLOR_C;

    public static readonly int[] STAR_SCORE = {20, 35, 50};
    public static readonly Vec2I[] D_WAYS = {new Vec2I(1, 0), new Vec2I(0, 1), new Vec2I(-1, 0), new Vec2I(0, -1)};
    public static readonly Vec2I MAP_SIZE = new Vec2I(8, 8);
    public static readonly int MAX_PLAYER_SCORE = MAP_SIZE.x * MAP_SIZE.y;

    public static Texture[] wTypeT = {LoadT("GUI/Icons/weapon/house"), LoadT("GUI/Icons/weapon/tank"), 
    LoadT("GUI/Icons/weapon/truck"), LoadT("GUI/Icons/weapon/drone"), LoadT("GUI/Icons/weapon/bulldozer"), 
    LoadT("GUI/Icons/weapon/wall"), LoadT("GUI/Icons/weapon/crane"), LoadT("GUI/Icons/weapon/vehicle")};
    public static Texture[] helpImages = {LoadT("Help/0"), LoadT("Help/1"), LoadT("Help/2"), LoadT("Help/3"), LoadT("Help/4")};
    public static PackedScene gamePS = LoadPS("game");

    public static PackedScene LoadPS(string psName)
    {
        PackedScene ps = ResourceLoader.Load("res://Scenes/" + psName + ".tscn") as PackedScene;
        if (ps == null)
        {
            GD.Print("Packed Scene loading error.");
        }
        return ps;
    }

    public static Texture LoadT(string tName)
    {
        Texture t = ResourceLoader.Load("res://Sprites/" + tName + ".png") as Texture;
        if (t == null)
        {
            GD.Print("Texture loading error.");
        }
        return t;
    }

    public static Color NormWColor(Color color)
    {
        Color c = color;
        c.r = Mathf.Max(c.r, 0.1f);
        c.g = Mathf.Max(c.g, 0.1f);
        c.b = Mathf.Max(c.b, 0.1f);
        c.a = 1.0f;
        return c;
    }

    public static Color GetColorByWType(uint type)
    {
        Color c = new Color();
        uint x = WEAPON_PER_COLOR_C;
        float y = 0.0f;
        if (x <= 1)
        {
            return c;
        }
        y = 1.0f / ((float)(x - 1));
        c.r = y * (type % x);
        c.g = y * ((type / x) % x);
        c.b = y * ((type / (x * x)) % x);
        c.a = 1.0f;
        return NormWColor(c);
    }

}
