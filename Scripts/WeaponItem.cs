using Godot;
using System;
using static Lib;

public class WeaponItem : Button
{

    protected Root root;
    protected TextureRect image;
    protected int num;

    public void _on_button_down()
    {
        root.mWType = num;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        image = (TextureRect)GetNode("Image");
        num = -1;
        if (this.Name[0] >= '0' && this.Name[0] <= '9')
        {
            num = (int)this.Name[0] - '0';
        }
    }

    public override void _Process(float delta)
    {
        if (num >= 0 && num < WEAPON_TYPES_NUM && root.weaponTNum[num] > 0)
        {
            this.Visible = true;
            image.Texture = wTypeT[num];
            image.Modulate = GetColorByWType((uint)num);
            this.Text = root.weaponTNum[num].ToString();
            if (image.Modulate.r + image.Modulate.g + image.Modulate.b >= 1.5f)
            {
                this.SelfModulate = Colors.Black;
            }
            else
            {
                this.SelfModulate = Colors.White;
            }
        }
        else
        {
            this.Visible = false;
            // ?? clear all ??
        }
    }

}
