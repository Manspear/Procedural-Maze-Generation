using System.Collections;
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
    }

    public class direction
    {
        public int XMod, ZMod;
    }

    // Use this for initialization
    void Start () {
        
        visualizedMaze = new GameObject[4 * gridX * gridZ];
        //recursiveBackTrack(0, 0);
        //visualizeMaze();

    }
    bool flipFlop = true;
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            InitializeMazeCreationVariables();
            recursiveBackTrack(0, 0);
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
                grid[i, j] = new cell();
            }
        }

        NORTHDir.XMod = -1;
        NORTHDir.ZMod = 0;
        SOUTHDir.XMod = 1;
        SOUTHDir.ZMod = 0;
        WESTDir.XMod = 0;
        WESTDir.ZMod = -1;
        EASTDir.XMod = 0;
        EASTDir.ZMod = 1;
 
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
                    visualizedMaze[wallOffset] = Instantiate(westWall, wallPos, transform.rotation);
                if (grid[i, j].SouthWall)
                    visualizedMaze[wallOffset + 1] = Instantiate(southWall, wallPos, transform.rotation);
                if (grid[i, j].NorthWall)
                    visualizedMaze[wallOffset + 2] = Instantiate(northWall, wallPos, transform.rotation);
                if (grid[i, j].EastWall)
                    visualizedMaze[wallOffset + 3] = Instantiate(eastWall, wallPos, transform.rotation);
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
            if ((gridXPos + dir.XMod >= 0 && gridXPos + dir.XMod < gridX) && (gridZPos + dir.ZMod >= 0 && gridZPos + dir.ZMod < gridZ) && grid[gridXPos + dir.XMod, gridZPos + dir.ZMod].Visited == false)
            {
                if (dir == NORTHDir)
                {
                    grid[gridXPos, gridZPos].NorthWall = false;
                    grid[gridXPos + dir.XMod, gridZPos + dir.ZMod].SouthWall = false;
                }
                if (dir == SOUTHDir) //my hombre
                {
                    grid[gridXPos, gridZPos].SouthWall = false;
                    grid[gridXPos + dir.XMod, gridZPos + dir.ZMod].NorthWall = false;
                }
                if (dir == WESTDir)
                {
                    grid[gridXPos, gridZPos].WestWall = false;
                    grid[gridXPos + dir.XMod, gridZPos + dir.ZMod].EastWall = false;
                }
                if (dir == EASTDir)
                {
                    grid[gridXPos, gridZPos].EastWall = false;
                    grid[gridXPos + dir.XMod, gridZPos + dir.ZMod].WestWall = false;
                }
                recursiveBackTrack(gridXPos + dir.XMod, gridZPos + dir.ZMod);
            }
        }
    }
}
