using Godot;
using System;
using static Lib;

public class Help : Button
{

    private Root root;
    private HelpPanel helpPanel;

    public void _on_button_up()
    {
        helpPanel.Visible = true;
        helpPanel.aImage = 0;
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        helpPanel = (HelpPanel)GetNode("../HelpPanel");
    }

    public override void _Process(float delta)
    {

    }

}
