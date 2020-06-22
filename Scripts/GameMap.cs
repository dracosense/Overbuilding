using Godot;
using System;
using System.Collections.Generic;
using static Lib;

public class GameMap : TileMap
{

    protected Queue<MapTile> bQueue;
    protected int[,] wMap; 
    protected Root root;
    protected TileMap floorMap;
    protected TileMap activePlaceMap;
    protected TileMap topFloorMap;
    protected TileMap weaponMap;
    protected Vec2I mMapPos;
    protected Vec2I pAPos;
    protected Vec2I eAPos;
    protected float timeFromBuild;
    protected float buildTimeout = 0.01f;
    protected int pAction;
    protected int eAction;
    protected int pScore;
    protected int eScore;
    protected int gamePhase; // 0 - actions, 1 - move actions to queue, 2 - actions on map, 3 - city building queue, 4 - city map buildion
    protected bool somethingBuilded;

    public bool IsPosOutOfSZone(Vec2I pos, bool player)
    {
        return (player?Mathf.Abs(pos.x - (MAP_SIZE.x - 1)) + Mathf.Abs(pos.y - (MAP_SIZE.y - 1)) > 
        START_ZONE_DIST:Mathf.Abs(pos.x + pos.y) > START_ZONE_DIST);
    }

    public void BuildFromQueue()
    {
        int x = 0;
        int y = 0;
        if (bQueue == null || bQueue.Count <= 0)
        {
            return;
        }
        if (timeFromBuild >= buildTimeout)
        {
            MapTile tile = bQueue.Dequeue();
            x = GetCell(tile.pos.x, tile.pos.y);
            y = topFloorMap.GetCell(tile.pos.x, tile.pos.y);
            if (x != BLOCKED_TILE)
            {
                if (tile.cell != P_WEAPON_TILE && tile.cell != E_WEAPON_TILE)
                {
                    wMap[tile.pos.x, tile.pos.y] = -1;
                }
                if (tile.cell == P_HOUSE_TILE)
                {
                    pScore++;
                }
                if (tile.cell == E_HOUSE_TILE)
                {
                    eScore++;
                }
                if (y == P_HOUSE_TILE)
                {
                    pScore--;
                }
                if (y == E_HOUSE_TILE)
                {
                    eScore--;
                }
                if (tile.topFloor)
                {
                    topFloorMap.SetCell(tile.pos.x, tile.pos.y, tile.cell);
                }
                else
                {
                    if (x == P_HOUSE_TILE)
                    {
                        pScore--;
                    }
                    if (x == E_HOUSE_TILE)
                    {
                        eScore--;
                    }
                    topFloorMap.SetCell(tile.pos.x, tile.pos.y, -1);
                    SetCell(tile.pos.x, tile.pos.y, tile.cell);
                }
                somethingBuilded = true;
            }
            timeFromBuild = 0.0f;
        }
    }

    public void CityStep()
    {
        int[,] tempCMap = new int[MAP_SIZE.x, MAP_SIZE.y];
        Vec2I v = new Vec2I();
        int x = 0;
        int y = 0;
        for (int i = 0; i < MAP_SIZE.x; i++)
        {
            for (int j = 0; j < MAP_SIZE.y; j++)
            {
                tempCMap[i, j] = 0;
            }
        }
        for (int i = 0; i < MAP_SIZE.x; i++)
        {
            for (int j = 0; j < MAP_SIZE.y; j++)
            {
                x = 0;
                if (GetCell(i, j) == P_HOUSE_TILE)
                {
                    x = 1;
                }
                if (GetCell(i, j) ==  E_HOUSE_TILE)
                {
                    x = 2;
                }
                if (x != 0)
                {
                    for (int p = 0; p < D_NUM; p++)
                    {
                        v.x = i + D_WAYS[p].x;
                        v.y = j + D_WAYS[p].y;
                        if (v.x >= 0 && v.x < MAP_SIZE.x && v.y >= 0 && v.y < MAP_SIZE.y && GetCell(v.x, v.y) == -1)
                        {
                            y = tempCMap[v.x, v.y];
                            if (y != 0 && y != x)
                            {
                                tempCMap[v.x, v.y] = -1;
                            }
                            else
                            {
                                tempCMap[v.x, v.y] = x;
                            }
                        }
                    }
                }
            }
        }
        for (int i = 0; i < MAP_SIZE.x; i++)
        {
            for (int j = 0; j < MAP_SIZE.y; j++)
            {
                if (tempCMap[i, j] == 1 && root.rand.NextDouble() < HOUSE_GEN_C)
                {
                    bQueue.Enqueue(new MapTile(new Vec2I(i, j), (int)P_HOUSE_TILE));
                }
                if (tempCMap[i, j] == 2 && root.rand.NextDouble() < HOUSE_GEN_C)
                {
                    bQueue.Enqueue(new MapTile(new Vec2I(i, j), (int)E_HOUSE_TILE));
                }
            }
        }
    }

    public Vec2I FindFreeTileNear(Vec2I pos) // 
    {
        Vec2I ans = pos;
        int d = MAP_SIZE.x + MAP_SIZE.y - 1; // more the max dist
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if ((i == 0 && j == 0) || pos.x + i < 0 || pos.y + j < 0 || pos.x + i >= MAP_SIZE.x || pos.y + j >= MAP_SIZE.y || GetCell(pos.x + i, pos.y + j) != -1)
                {
                    continue;
                }
                if (d > Mathf.Abs(i) + Mathf.Abs(j))
                {
                    ans = new Vec2I(pos.x + i, pos.y + j);
                    d = Mathf.Abs(i) + Mathf.Abs(j);
                }
            }
        }
        return ans;
    }

    public void BuildWeapon(int w, Vec2I pos, bool player)
    {
        Vec2I v = new Vec2I(0, 0);
        bool b = false;
        if (w < 0)
        {
            return;
        }
        if (GetCell(pos.x, pos.y) != (player?P_WEAPON_TILE:E_WEAPON_TILE) && w != WALL_W && w != VEHICLE_W)
        {
            wMap[pos.x, pos.y] = w;
            bQueue.Enqueue(new MapTile(pos, (int)(player?P_WEAPON_TILE:E_WEAPON_TILE)));
        }
        switch(w)
        {
            case HOUSE_W: // house
                v = FindFreeTileNear(pos);
                if (v.x != pos.x || v.y != pos.y)
                {
                    bQueue.Enqueue(new MapTile(v, (int)(player?P_HOUSE_TILE:E_HOUSE_TILE)));
                }
                break;
            case TANK_W: // tank
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if ((i == 0 && j == 0) || pos.x + i < 0 || pos.y + j < 0 || pos.x + i >= MAP_SIZE.x || pos.y + j >= MAP_SIZE.y)
                        {
                            continue;
                        }
                        bQueue.Enqueue(new MapTile(new Vec2I(pos.x + i, pos.y + j), -1));
                    }
                }
                break;
            case TRUCK_W: // truck
                v = FindFreeTileNear(pos);
                if (v.x != pos.x || v.y != pos.y)
                {
                    bQueue.Enqueue(new MapTile(v, (int)WALL_TILE));
                }
                break;
            case DRONE_W: // drone
                bQueue.Enqueue(new MapTile(pos, (int)(player?P_HOUSE_TILE:E_HOUSE_TILE)));
                break;
            case BULLDOZER_W: // bulldozer
                for (int i = 0; i < MAP_SIZE.x; i++)
                {
                    if (i == pos.x)
                    {
                        continue;
                    }
                    bQueue.Enqueue(new MapTile(new Vec2I(i, pos.y), -1));
                }
                for (int i = 0; i < MAP_SIZE.y; i++)
                {
                    if (i == pos.y)
                    {
                        continue;
                    }
                    bQueue.Enqueue(new MapTile(new Vec2I(pos.x, i), -1));
                }
                break;
            case WALL_W: // wall
                bQueue.Enqueue(new MapTile(pos, (int)BLOCKED_TILE));
                break;
            case CRANE_W: // zeppelin
                b = false;
                for (int i = -1; i <= 1 && !b; i++)
                {
                    for (int j = -1; j <= 1 && !b; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            continue;
                        }
                        if (topFloorMap.GetCell(pos.x + i, pos.y + j) == -1 && 
                        GetCell(pos.x + i, pos.y + j) == (player?P_HOUSE_TILE:E_HOUSE_TILE))
                        {
                            bQueue.Enqueue(new MapTile(new Vec2I(pos.x + i, pos.y + j), (int)(player?P_HOUSE_TILE:E_HOUSE_TILE), true));
                            b = true;
                        }
                    }
                }
                // nearest free top tile ?? other algorithm ?? 
                break;
            case VEHICLE_W: // vehicle 
                if (player)
                {
                    pScore += VEHICLE_SCORE_C;
                }
                else
                {
                    eScore += VEHICLE_SCORE_C;
                }
                // bQueue.Enqueue(new MapTile(pos, (int)(player?P_HOUSE_TILE:E_HOUSE_TILE)));
                break;
            default:
                break;
        }
    }

    public void CheckPlayerAction()
    {
        int x = 0;
        if (Input.IsActionJustReleased("action"))
        {
            if (mMapPos.x >= 0 && mMapPos.y >= 0 && mMapPos.x < MAP_SIZE.x && 
            mMapPos.y < MAP_SIZE.y && pAction == -1 && gamePhase == 0 && IsPosOutOfSZone(mMapPos, false))
            {
                x = GetCell(mMapPos.x, mMapPos.y);
                if (root.mWeapon >= 0 && root.mWeapon < WEAPON_NUM && 
                (x == -1 || root.playerWeapon[root.mWeapon].GetWType() == DRONE_W || 
                (root.playerWeapon[root.mWeapon].GetWType() == CRANE_W && x == P_HOUSE_TILE)) && 
                x != BLOCKED_TILE)
                {
                    x = root.playerWeapon[root.mWeapon].GetWType();
                    // wMap[mMapPos.x, mMapPos.y] = x;
                    pAction = x;
                    pAPos = mMapPos;
                    root.wActivated[root.mWeapon] = true;
                }
                else
                {
                    if (wMap[mMapPos.x, mMapPos.y] >= 0)
                    {
                        pAction = wMap[mMapPos.x, mMapPos.y];
                        pAPos = mMapPos;
                    }
                }
            }
            root.mWeapon = -1;
        }
    }

    public void GetEAction()
    {
        List<Vec2I> freeTiles = new List<Vec2I>();
        List<Vec2I> weaponTiles = new List<Vec2I>();
        if (eAction == -1 && gamePhase == 0)
        {
            for (int i = 0; i < MAP_SIZE.x; i++)
            {
                for (int j = 0; j < MAP_SIZE.y; j++)
                {
                    if (GetCell(i, j) == -1 && IsPosOutOfSZone(new Vec2I(i, j), true))
                    {
                        freeTiles.Add(new Vec2I(i, j));
                    }
                    if (GetCell(i, j) == E_WEAPON_TILE)
                    {
                        weaponTiles.Add(new Vec2I(i, j));
                    }
                }
            }
            if (root.gameTurn < WEAPON_NUM)
            {
                if (freeTiles.Count > 0)
                {
                    eAction = (int)(root.rand.Next() % WEAPON_TYPES_NUM);
                    eAPos = freeTiles[root.rand.Next() % freeTiles.Count];
                    // wMap[eAPos.y, eAPos.y] = eAction;
                }
                else
                {
                    eAction = -2;
                }
            }
            else
            {
                if (weaponTiles.Count > 0)
                {
                    eAPos = weaponTiles[root.rand.Next() % weaponTiles.Count];
                    eAction = wMap[eAPos.x, eAPos.y];
                    if (eAction == -1)
                    {
                        eAction = -2;
                    }
                }
                else
                {
                    eAction = -2;
                }
            }
        }
    }

    public override void _Ready()
    {
        root = (Root)GetNode("/root/root");
        floorMap = (TileMap)GetNode("../FloorMap");
        activePlaceMap = (TileMap)GetNode("../ActivePlaceMap");
        topFloorMap = (TileMap)GetNode("../TopFloorMap");
        weaponMap = (TileMap)GetNode("../WeaponMap");
        bQueue = new Queue<MapTile>();
        wMap = new int[MAP_SIZE.x, MAP_SIZE.y];
        for (int i = 0; i < MAP_SIZE.x; i++)
        {
            for (int j = 0; j < MAP_SIZE.y; j++)
            {
                wMap[i, j] = -1;
            }
        }
        mMapPos = new Vec2I(0, 0);
        pAPos = new Vec2I(0, 0);
        eAPos = new Vec2I(0, 0);
        pAction = -1;
        eAction = -1;
        pScore = 1; // 0 
        eScore = 1; // 0
        gamePhase = 0; 
        for (int i = 0; i < MAP_SIZE.x; i++)
        {
            for (int j = 0; j < MAP_SIZE.y; j++)
            {
                if (IsPosOutOfSZone(new Vec2I(i, j), true) && IsPosOutOfSZone(new Vec2I(i, j), false) && 
                root.rand.NextDouble() < GEN_WALL_C) // ?? non abstract check ??
                {
                    SetCell(i, j, (int)WALL_TILE);
                    SetCell(MAP_SIZE.x - i - 1, MAP_SIZE.y - j - 1, (int)WALL_TILE);
                }
            }
        }
    }

    public override void _Process(float delta)
    {
        if (root.playerGameScore < 0)
        {
            return;
        }
        Vector2 v = WorldToMap(GetLocalMousePosition()); //floorMap.WorldToMap(floorMap.GetLocalMousePosition());
        mMapPos.x = (int)v.x;
        mMapPos.y = (int)v.y;
        root.playerGameScore = pScore;
        if (activePlaceMap.GetCell(mMapPos.x, mMapPos.y) == -1) // ??
        {
            activePlaceMap.Clear();
            activePlaceMap.SetCell(mMapPos.x, mMapPos.y, 0);
        }
        for (int i = 0; i < MAP_SIZE.x; i++)
        {
            for (int j = 0; j < MAP_SIZE.y; j++)
            {
                weaponMap.SetCell(i, j, ((GetCell(i, j) == P_WEAPON_TILE)?wMap[i, j]:-1));
            }
        }
        timeFromBuild += delta;
        GetEAction();
        CheckPlayerAction();
        switch(gamePhase)
        {   
            case 0:
                if (pAction != -1 && eAction != -1)
                {
                    gamePhase = 1;
                }
                somethingBuilded = false; 
                break;
            case 1:
                CityStep();
                gamePhase = 2;
                timeFromBuild = 0.0f;
                break;
            case 2:
                if (bQueue.Count <= 0)
                {
                    gamePhase = 3;
                    break;
                }
                BuildFromQueue();
                break;
            case 3:
                BuildWeapon(eAction, eAPos, false);
                BuildWeapon(pAction, pAPos, true);
                gamePhase = 4;
                timeFromBuild = 0.0f;
                break;
            case 4:
                if (bQueue.Count <= 0)
                {
                    pAction = -1;
                    eAction = -1;
                    gamePhase = 0;
                    root.gameTurn++;
                    if (!somethingBuilded)
                    {
                        root.EndGame();
                    }
                    break;
                }
                BuildFromQueue();
                break;
            default:
                break;
        }
    }

}
