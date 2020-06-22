using Godot;
using System;
using static Lib;

public class GUI : Node
{

    protected Root root;
    protected Control gameUI;
    protected Control menuUI;
    protected Control collectionMenu;
    protected Control weaponMenu;
    protected Control battleMenu;
    protected Control exitGamePanel;
    protected Label turnCounter;
    protected ProgressBar playerScore;
    protected TextureProgress playerStars;

    public void _on_exit_button_down()
    {
        root.ExitGame();
    }

    public void ShowExitGamePanel(int stars)
    {
        exitGamePanel.Visible = true;
        playerStars.Value = playerStars.MaxValue * (((float)stars) / ((float)STARS_NUM));
    }

    public override void _Ready()
    {
        string s;
        root = (Root)GetNode("/root/root");
        gameUI = (Control)GetNode("GamePanel");
        menuUI = (Control)GetNode("MenuPanel");
        s = "MenuPanel/Menu/MainPanel/";
        collectionMenu = (Control)GetNode(s + "Collection");
        weaponMenu = (Control)GetNode(s + "Weapon");
        battleMenu = (Control)GetNode(s + "Battle");
        exitGamePanel = (Control)GetNode("GamePanel/ExitGamePanel");
        turnCounter = (Label)GetNode("GamePanel/TurnCounter");
        playerScore = (ProgressBar)GetNode("GamePanel/PlayerScore");
        playerStars = (TextureProgress)GetNode("GamePanel/ExitGamePanel/Stars");
    }

    public override void _Process(float delta)
    {
        menuUI.Visible = !(gameUI.Visible = (root.menuPanel == GAME_M_PANEL));
        collectionMenu.Visible = (root.menuPanel == COLLECTION_M_PANEL);
        weaponMenu.Visible = (root.menuPanel == WEAPON_M_PANEL);
        battleMenu.Visible = (root.menuPanel == BATTLE_M_PANEL);
        if (root.playerGameScore >= 0)
        {
            turnCounter.Text = root.gameTurn.ToString() + '/' + GAME_TURNS_NUM.ToString();
            playerScore.Value = playerScore.MaxValue * ((MAX_PLAYER_SCORE >= 0)?(((float)root.playerGameScore) / ((float)MAX_PLAYER_SCORE)):0);
        }
        if (root.menuPanel != GAME_M_PANEL)
        {
            exitGamePanel.Visible = false;
        } 
    }
}
