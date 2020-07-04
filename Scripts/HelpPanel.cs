using Godot;
using System;
using static Lib;

public class HelpPanel : TextureRect
{

    public int aImage = 0;

    public void _on_button_up()
    {
        aImage++;
    }

    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if (aImage < 0 || aImage >= HELP_IMAGES_N)
        {
            this.Visible = false;
        }
        else
        {
            this.Texture = helpImages[aImage];
        }
    }

}
