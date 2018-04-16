﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ATestScript : MonoBehaviour {

    float posOffset = 8.6f;
    public GameObject northWall;
    public GameObject southWall;
    public GameObject westWall;
    public GameObject eastWall;

    public int gridX = 5;
    public int gridZ = 10;
    public static cell[,] grid;
    public int seedNumber = 0;
    public bool randomSeed = false;

    private static direction NORTHDir = new direction();
    private static direction SOUTHDir = new direction();
    private static direction EASTDir = new direction();
    private static direction WESTDir = new direction();

    public class cell
    {
        public bool Visited = false;
        public bool NorthWall = true;
        public bool SouthWall = true;
        public bool EastWall = true;
        public bool WestWall = true;
        public cell(bool isVisited, bool startWithActiveWalls)
        {
            this.Visited = isVisited;
            this.NorthWall = startWithActiveWalls;
            this.SouthWall = startWithActiveWalls;
            this.WestWall = startWithActiveWalls;
            this.EastWall = startWithActiveWalls;
        }
    }

    public class direction
    {
        public int X, Z;
    }

    // Use this for initialization
    void Start() {

        visualizedMaze = new GameObject[4 * gridX * gridZ];
        //recursiveBackTrack(0, 0);
        //visualizeMaze();

    }
    bool flipFlop = true;
    void Update()
    {
        //recursive backtracker
        if (Input.GetKeyDown("q"))
        {
            InitializeMazeCreationVariables();
            recursiveBackTrack(0, 0);
            visualizeMaze();
        }
        //Prim's algorithm
        if (Input.GetKeyDown("e"))
        {
            InitializeMazeCreationVariables();
            primsAlgorithm(0, 0);
            visualizeMaze();
        }
        //Recursive Division
        if (Input.GetKeyDown("r"))
        {
            InitializeRecursiveDivisionMazeCreationVariables();
            recursiveDivision(0, 0, gridX, gridZ);
            visualizeMaze();
        }
        if (Input.GetKeyDown("f"))
        {

            if (flipFlop)
            {
                Vector3 aods = new Vector3(-20, -20, -20);
                visualizedMaze[0] = Instantiate(eastWall, aods, transform.rotation);
                flipFlop = false;
            }
            else
            {
                foreach (GameObject obj in visualizedMaze)
                {
                    Destroy(obj);
                }
                //Destroy(visualizedMaze[1]);
                flipFlop = true;
            }
        }
    }
    void InitializeMazeCreationVariables()
    {
        //initializing grid
        grid = new cell[gridX, gridZ];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new cell(false, true);
            }
        }

        NORTHDir.X = -1;
        NORTHDir.Z = 0;
        SOUTHDir.X = 1;
        SOUTHDir.Z = 0;
        WESTDir.X = 0;
        WESTDir.Z = -1;
        EASTDir.X = 0;
        EASTDir.Z = 1;

        if (randomSeed)
            seedNumber = Random.Range(0, 999999999);
        Random.InitState(seedNumber);
    }

    void InitializeRecursiveDivisionMazeCreationVariables()
    {
        //initializing grid
        grid = new cell[gridX, gridZ];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new cell(false, false);
            }
        }

        NORTHDir.X = -1;
        NORTHDir.Z = 0;
        SOUTHDir.X = 1;
        SOUTHDir.Z = 0;
        WESTDir.X = 0;
        WESTDir.Z = -1;
        EASTDir.X = 0;
        EASTDir.Z = 1;

        if (randomSeed)
            seedNumber = Random.Range(0, 999999999);
        Random.InitState(seedNumber);
    }

    GameObject[] visualizedMaze;

    void visualizeMaze()
    {
        foreach (GameObject obj in visualizedMaze)
        {
            Destroy(obj);
        }

        Vector3 wallPos = new Vector3(0, 0, 0);
        int wallOffset = 0;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                print("i: " + i);
                print("j: " + j);
                print("offset: " + wallOffset);
                print(4 * gridX * gridZ);
                wallPos.Set(posOffset * i, 0, posOffset * j);

                if (grid[i, j].WestWall)
                    visualizedMaze[wallOffset] = Instantiate(westWall, wallPos, Quaternion.identity);
                if (grid[i, j].SouthWall)
                    visualizedMaze[wallOffset + 1] = Instantiate(southWall, wallPos, Quaternion.identity);
                if (grid[i, j].NorthWall)
                    visualizedMaze[wallOffset + 2] = Instantiate(northWall, wallPos, Quaternion.identity);
                if (grid[i, j].EastWall)
                    visualizedMaze[wallOffset + 3] = Instantiate(eastWall, wallPos, Quaternion.identity);
                wallOffset += 4;
            }
        }
    }

    void shuffleDirections(List<direction> directionList)
    {
        //Shuffles directionList
        int n = directionList.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n);
            n--;
            direction value = directionList[k];
            directionList[k] = directionList[n];
            directionList[n] = value;
        }
    }

    public struct mazeLoc
    {
        public int XPos, ZPos;

        public mazeLoc(int X, int Z)
        {
            this.XPos = X;
            this.ZPos = Z;
        }
    }

    void carveInDirection(direction dir, int gridXPos, int gridZPos)
    {
        if ((gridXPos + dir.X >= 0 && gridXPos + dir.X < gridX) && (gridZPos + dir.Z >= 0 && gridZPos + dir.Z < gridZ))
        {
            if (dir == NORTHDir)
            {
                grid[gridXPos, gridZPos].NorthWall = false;
                grid[gridXPos + dir.X, gridZPos + dir.Z].SouthWall = false;
            }
            if (dir == SOUTHDir)
            {
                grid[gridXPos, gridZPos].SouthWall = false;
                grid[gridXPos + dir.X, gridZPos + dir.Z].NorthWall = false;
            }
            if (dir == WESTDir)
            {
                grid[gridXPos, gridZPos].WestWall = false;
                grid[gridXPos + dir.X, gridZPos + dir.Z].EastWall = false;
            }
            if (dir == EASTDir)
            {
                grid[gridXPos, gridZPos].EastWall = false;
                grid[gridXPos + dir.X, gridZPos + dir.Z].WestWall = false;
            }
            recursiveBackTrack(gridXPos + dir.X, gridZPos + dir.Z);
        }
    }

    void recursiveDivision(int X, int Z, int width, int height)
    {
        //First try always dividing horizontally. So Y is unchanging
        int splitX = Random.Range(X, width-1);
        int splitZ = Z + height;

        //Now I got the position to "split to". Split all in-betweens.
        for (int i = Z; i < splitZ; i++)
        {
            grid[splitX, i].EastWall = true;
        }
    }

    void primsAlgorithm(int startX, int startZ)
    {
        //start from a nodes, all adjacent nodes are frontier.
        //Pick a random frontier node, add it to the maze. Now add the adjacent nodes to the new node as frontier too.
        //rinse repeat until no more frontier cells. 

        List<mazeLoc> frontier = new List<mazeLoc>();

        //Add directions
        List<direction> directionList = new List<direction>();
        //initializing list of directions 
        directionList.Add(NORTHDir);
        directionList.Add(SOUTHDir);
        directionList.Add(WESTDir);
        directionList.Add(EASTDir);

        //Current frontier index
        int CFI;

        //Add start cell to frontier
        grid[startX, startZ].Visited = true;
        frontier.Add(new mazeLoc(startX, startZ));
        frontier.TrimExcess();

        bool hasCarved = false;

        while (frontier.Count > 0)
        {
            //frontier.RemoveAt();
            CFI = Random.Range(0, frontier.Count - 1);
            grid[frontier[CFI].XPos, frontier[CFI].ZPos].Visited = true;
            shuffleDirections(directionList);
            
            foreach (direction dir in directionList)
            {
                //Add valid adjacent cells to frontier.
                if ((frontier[CFI].XPos + dir.X >= 0 && frontier[CFI].XPos + dir.X < gridX) && (frontier[CFI].ZPos + dir.Z >= 0 && frontier[CFI].ZPos + dir.Z < gridZ) && grid[frontier[CFI].XPos + dir.X, frontier[CFI].ZPos + dir.Z].Visited == false)
                {
                    frontier.Add(new mazeLoc(frontier[CFI].XPos + dir.X, frontier[CFI].ZPos + dir.Z));
                }
                //If cell adjacent to this frontier has already been visited, carve path to it. (only once per frontier-loop)
                if (!hasCarved && (frontier[CFI].XPos + dir.X >= 0 && frontier[CFI].XPos + dir.X < gridX) && (frontier[CFI].ZPos + dir.Z >= 0 && frontier[CFI].ZPos + dir.Z < gridZ) && grid[frontier[CFI].XPos + dir.X, frontier[CFI].ZPos + dir.Z].Visited == true)
                {
                    carveInDirection(dir, frontier[CFI].XPos, frontier[CFI].ZPos);
                    hasCarved = true;
                }
            }
            //Reset hasCarved so that new adjacent 
            hasCarved = false;

            //Must be done last or algorithm doesnt work as it is. 
            frontier.RemoveAt(CFI);
        }
    }
       

    void recursiveBackTrack(int gridXPos, int gridZPos)
    {
        List<direction> directionList = new List<direction>();
        //initializing list of directions 
        directionList.Add(NORTHDir);
        directionList.Add(SOUTHDir);
        directionList.Add(WESTDir);
        directionList.Add(EASTDir);

        shuffleDirections(directionList);

        //set current cell as visited
        grid[gridXPos, gridZPos].Visited = true;

        //Now loop through neighbours. If found neighbour, call self, with neighbours position
        foreach (direction dir in directionList)
        {
            if ((gridXPos + dir.X >= 0 && gridXPos + dir.X < gridX) && (gridZPos + dir.Z >= 0 && gridZPos + dir.Z < gridZ) && grid[gridXPos + dir.X, gridZPos + dir.Z].Visited == false)
            {
                if (dir == NORTHDir)
                {
                    grid[gridXPos, gridZPos].NorthWall = false;
                    grid[gridXPos + dir.X, gridZPos + dir.Z].SouthWall = false;
                }
                if (dir == SOUTHDir) //my hombre
                {
                    grid[gridXPos, gridZPos].SouthWall = false;
                    grid[gridXPos + dir.X, gridZPos + dir.Z].NorthWall = false;
                }
                if (dir == WESTDir)
                {
                    grid[gridXPos, gridZPos].WestWall = false;
                    grid[gridXPos + dir.X, gridZPos + dir.Z].EastWall = false;
                }
                if (dir == EASTDir)
                {
                    grid[gridXPos, gridZPos].EastWall = false;
                    grid[gridXPos + dir.X, gridZPos + dir.Z].WestWall = false;
                }
                recursiveBackTrack(gridXPos + dir.X, gridZPos + dir.Z);
            }
        }
    }
}
