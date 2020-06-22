using Godot;
using System;

public class PlayButton : Button
{

    protected Root root;

    public void _on_button_up()
    {
        root.StartGame(0); // ?? level ??
        // ?? network game ??
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
    }

}
