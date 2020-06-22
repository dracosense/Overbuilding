using Godot;
using System;
using static Lib;

public class Root : Node
{

    public WeaponObj[] playerWeapon;
    public uint[] weaponTNum;
    public bool[] wActivated;
    public Random rand = new Random();
    public int menuPanel;
    public int gameLevel;
    public int mWType;
    public int mWeapon;
    public int wActive;
    public int gameTurn;
    public int playerGameScore;
    public int pRating;

    protected GUI gui;

    public bool DelGame()
    {
        Node game = GetNodeOrNull("/root/Game");
        if (game != null)
        {
            game.Free();
            return true;
        }
        return false;
    }

    public void SaveGame()
    {
        File file = new File();
        Error e = file.Open(SAVE_FILE_NAME, File.ModeFlags.Write);
        if (e == Error.Ok)
        {
            try
            {
                for (int i = 0; i < WEAPON_TYPES_NUM; i++)
                {
                    file.Store32(weaponTNum[i]);
                }
                for (int i = 0; i < WEAPON_NUM; i++)
                {
                    playerWeapon[i].StoreToFile(file);
                }
            }
            catch
            {
                GD.Print("Saving to file error.");
            }
            file.Close();
        }
        else
        {
            GD.Print("Open file to save error.");
        }
    }

    public void LoadGame()
    {
        File file = new File();
        Error e = file.Open(SAVE_FILE_NAME, File.ModeFlags.Read); // 
        if (e == Error.Ok)
        {
            try
            {
                for (int i = 0; i < WEAPON_TYPES_NUM; i++)
                {
                    weaponTNum[i] = file.Get32();
                }
                for (int i = 0; i < WEAPON_NUM; i++)
                {
                    playerWeapon[i].GetFromFile(file);
                }
            }
            catch
            {
                GD.Print("Loading from file error.");
            }
            file.Close();
        }
    }

    public void StartGame(int level)
    {
        if (menuPanel == GAME_M_PANEL)
        {
            GD.Print("Start game in game error.");
            return;
        }
        DelGame();
        mWeapon = -1;
        gameLevel = level;
        gameTurn = 0;
        playerGameScore = 0;
        for (int i = 0; i < WEAPON_NUM; i++)
        {
            wActivated[i] = false;
        }
        GetTree().Root.AddChild(gamePS.Instance());
        menuPanel = GAME_M_PANEL;
    }

    public void ExitGame()
    {
        DelGame();
        menuPanel = BATTLE_M_PANEL;
        gameLevel = -1;
        mWeapon = -1;
    }

    public void EndGame()
    {
        int stars = 0;
        for (int i = 0; i < STARS_NUM; i++)
        {
            if (STAR_SCORE[i] <= playerGameScore)
            {
                if (WEAPON_TYPES_NUM > 0)
                {
                    weaponTNum[rand.Next() % WEAPON_TYPES_NUM]++;
                }
                stars++;
            }
        }
        // ?? score ??
        playerGameScore = -1;
        gameTurn = 0;
        SaveGame();
        gui.ShowExitGamePanel(stars);
    }
    
    public override void _Ready()
    {
        playerWeapon = new WeaponObj[WEAPON_NUM];
        weaponTNum = new uint[WEAPON_TYPES_NUM];
        wActivated = new bool[WEAPON_NUM];
        for (int i = 0; i < WEAPON_NUM; i++)
        {
            playerWeapon[i] = new WeaponObj();
        }
        gui = (GUI)GetNode("/root/GUI");
        mWType = -1;
        mWeapon = -1;
        wActive = 0;
        playerGameScore = 0;
        menuPanel = START_M_PANEL;
        LoadGame();
        for (int i = 0; i < WEAPON_TYPES_NUM; i++)
        {
            weaponTNum[i] = 7; // 
        }
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("exit"))
        {
            GetTree().Quit();
        }
        if (menuPanel == GAME_M_PANEL)
        {
            if (gameTurn >= GAME_TURNS_NUM)
            {
                EndGame();
            }
        }
        else
        {
            if (menuPanel != WEAPON_M_PANEL)
            {
                mWType = -1;
            }
            else
            {
                if (!Input.IsActionPressed("action") && wActive <= 0)
                {
                    mWType = -1;
                }
            }
        }
    }

}
