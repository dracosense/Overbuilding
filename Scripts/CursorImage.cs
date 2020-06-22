using Godot;
using System;
using static Lib;

public class CursorImage : TextureRect
{

    protected Root root;

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");        
    }

    public override void _Process(float delta)
    {
        int x = -1;
        this.SetGlobalPosition(GetGlobalMousePosition() - this.RectSize / 2.0f);
        if (root.mWType >= 0 && root.mWType < WEAPON_TYPES_NUM)
        {
            x = root.mWType;
        }
        if (root.mWeapon >= 0 && root.mWeapon < WEAPON_NUM)
        {
            x = root.playerWeapon[root.mWeapon].GetWType();
        }
        if (x >= 0)
        {
            this.Visible = true;
            this.Texture = wTypeT[x];
            this.SelfModulate = GetColorByWType((uint)x);
        }
        else
        {
            this.Visible = false;
        }
    }

}
