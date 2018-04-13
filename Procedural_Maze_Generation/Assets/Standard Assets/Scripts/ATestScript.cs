using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ATestScript : MonoBehaviour {
    private int[,] arrlok = new int[5, 5] { {0,0,0,0,1}, { 0, 0, 0, 1, 0 }, { 0, 0, 1, 0, 0 },{ 0, 1, 0, 0, 0 }, { 1, 0, 0, 0, 0 } };
    Vector3 cubeInstancePos;
    float posOffset = 2;
    public GameObject cube;
    // Use this for initialization
    void Start () {

        startRecursiveBacktracker();
        visualizeMaze();
        cubeInstancePos.Set(0, 0, 0);
        //GameObject aids = (GameObject)Instantiate(cube, cubeInstancePos, transform.rotation);
        for (int i = 0; i < arrlok.GetLength(0); i++)
        {
            for (int j = 0; j < arrlok.GetLength(1); j++)
            {
                if (arrlok[i, j] == 1)
                {
                    cubeInstancePos.Set(posOffset * i, posOffset * j, 0);
                    GameObject anObject = (GameObject)Instantiate(cube, cubeInstancePos, transform.rotation);
                }
            }   
        }
    }
    //support both walls and corridors. Have ability to GET multidimensional array from somewhere else. 
	
	// Update is called once per frame
	void Update () {
        
	}


    public static int NONE = 0;
    public static int NORTH = 1;
    public static int SOUTH = 2;
    public static int WEST = 4;
    public static int EAST = 8;

    public static int gridX = 10;
    public static int gridY = 10;

    public static List<int> directionList = new List<int>();
    public static cell[,] grid;

    public class cell
    {
        public bool Visited = false;
        public bool NorthWall = true;
        public bool SouthWall = true;
        public bool EastWall = true;
        public bool WestWall = true;
    }

    void shuffleDirections()
    {
        //Shuffles directionList
        int n = directionList.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n);
            n--;
            int value = directionList[k];
            directionList[k] = directionList[n];
            directionList[n] = value;
        }
    }

    void startRecursiveBacktracker()
    {
        //initializing grid
        grid = new cell[gridX, gridY];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new cell();
            }
        }

        //initializing list of directions 
        directionList.Add(NORTH);
        directionList.Add(SOUTH);
        directionList.Add(WEST);
        directionList.Add(EAST);

        Random.InitState(51267735);

        recursiveBackTrack(NONE, 0, 0);
    }

    void recursiveBackTrack(int fromDirection, int gridXPos, int gridYPos)
    {
        shuffleDirections();

        //set current cell as visited
        grid[gridXPos, gridYPos].Visited = true;

        //remove wall from direction traveled from
        if (fromDirection == NORTH)
            grid[gridXPos, gridYPos].NorthWall = false;
        if (fromDirection == SOUTH)
            grid[gridXPos, gridYPos].SouthWall = false;
        if (fromDirection == WEST)
            grid[gridXPos, gridYPos].WestWall = false;
        if (fromDirection == EAST)
            grid[gridXPos, gridYPos].EastWall = false;

        //Now loop through neighbours. If found neighbour, call self, with neighbours position
        foreach (int dir in directionList)
        {
            //north is +Y, south is -Y, west is -X, east is +X

            if (dir == NORTH)
            {
                if (gridYPos + 1 >= gridY)
                { }
                else if (grid[gridXPos, (gridYPos + 1)].Visited == true)
                { }
                else
                {
                    recursiveBackTrack(SOUTH, gridXPos, gridYPos + 1);
                }
            }
            else if (dir == SOUTH)
            {
                if (gridYPos - 1 < 0 )
                { }
                else if (grid[gridXPos, (gridYPos - 1)].Visited == true)
                { }
                else
                {
                    recursiveBackTrack(NORTH, gridXPos, gridYPos - 1);
                }
            }
            else if (dir == WEST)
            {
                if (gridXPos - 1 < 0)
                { }
                else if (grid[gridXPos - 1, gridYPos].Visited == true)
                { }
                else
                {
                    recursiveBackTrack(EAST, gridXPos - 1, gridYPos);
                }
            }
            else if (dir == EAST)
            {
                if (gridXPos + 1 >= gridX)
                {
                }
                else if (grid[gridXPos + 1, gridYPos].Visited == true)
                { }
                else
                {
                    recursiveBackTrack(WEST, gridXPos + 1, gridYPos);
                }
            }
        }
    }

    void visualizeMaze()
    {
        int cnt = 0;
        foreach (cell cel in grid)
        {
            print(cnt);
            cnt++;

            print(cnt);
            cnt++;
        }
    }
}
