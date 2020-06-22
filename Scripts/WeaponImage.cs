using Godot;
using System;
using static Lib;

public class WeaponImage : TextureRect
{

    protected Root root;
    protected int num;
    protected int active;

    public void _on_mouse_entered()
    {
        active++;
        root.wActive++;
    }

    public void _on_mouse_exited()
    {
        active--;
        root.wActive--;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        num = -1;
        active = 0;
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
            x = root.playerWeapon[num].GetWType();
            if(x >= 0 && x < WEAPON_TYPES_NUM)
            {
                this.Texture = wTypeT[x];
                this.Modulate = root.playerWeapon[num].GetColor(); // GetColorByWType((uint)x);
            }
        }
        if (active > 0 && root.menuPanel == WEAPON_M_PANEL && Input.IsActionJustReleased("action") && root.mWType >= 0 && root.mWType < WEAPON_TYPES_NUM & root.weaponTNum[root.mWType] > 0)
        {
            root.playerWeapon[num].AddColor(GetColorByWType((uint)root.mWType));
            root.weaponTNum[root.mWType]--;
            root.mWType = -1;
            root.SaveGame();
        }
    }

}
