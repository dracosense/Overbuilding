using Godot;
using System;
using static Lib;

public class PageButton : Button
{

    protected Root root;
    protected int num;

    public void _on_button_up()
    {
        root.menuPanel = num;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        num = START_M_PANEL;
        if (this.Name[0] >= '0' && this.Name[0] <= '9')
        {
            num = this.Name[0] - '0';
        }
    }

    public override void _Process(float delta)
    {
        if (root.menuPanel == num)
        {
            this.Modulate = Colors.DarkGray;
        }
        else
        {
            this.Modulate = Colors.White;
        }
    }

}
