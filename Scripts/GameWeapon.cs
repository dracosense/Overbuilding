using Godot;
using System;
using static Lib;

public class GameWeapon : Button
{

    protected Root root;
    protected TextureRect image;
    protected int num;

    public void _on_button_down()
    {
        if (num >= 0 && num < WEAPON_NUM)
        {
            root.mWeapon = num;
        }
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        image = (TextureRect)GetNode("Image");
        num = -1;
        this.Visible = true;
        if (this.Name[0] >= '0' && this.Name[0] <= '9')
        {
            num = (int)this.Name[0] - '0';
        }
    }

    public override void _Process(float delta)
    {
        int x = 0;
        if (num >= 0 && num < WEAPON_NUM)
        {
            this.Visible = !root.wActivated[num];
            x = root.playerWeapon[num].GetWType();
            if (x >= 0 && x < WEAPON_TYPES_NUM)
            {
                image.Texture = wTypeT[x];
                image.Modulate = GetColorByWType((uint)x);
            }
        }
    }

}
