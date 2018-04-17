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
    public GameObject redCube;
    public GameObject greenCube;

    public int gridX = 5;
    public int gridZ = 10;
    public static cell[,] grid;
    public int seedNumber = 0;
    public bool randomSeed = false;

    private static direction NORTHDir = new direction(-1,0);
    private static direction SOUTHDir = new direction(1,0);
    private static direction EASTDir = new direction(0, 1);
    private static direction WESTDir = new direction(0, -1);

    public class cell
    {
        public bool SolverVisited = false;
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
        public direction(int inX, int inZ)
        {
            this.X = inX;
            this.Z = inZ;
        }
    }

    // Use this for initialization
    void Start() {

        visualizedMaze = new GameObject[4 * gridX * gridZ];
        //recursiveBackTrack(0, 0);
        //visualizeMaze();

    }
    float startSolveTime;
    bool flipFlop = true;
    void Update()
    {
        float temp = Time.realtimeSinceStartup;
        //recursive backtracker
        if (Input.GetKeyDown("q"))
        {
            InitializeMazeCreationVariables();
            recursiveBackTrack(0, 0);
            visualizeMaze();
            solveMaze(0, 0, new List<direction>(), new List<direction>());
            visualizeMazeSolver();
        }
        //Prim's algorithm
        if (Input.GetKeyDown("e"))
        {
            InitializeMazeCreationVariables();
            primsAlgorithm(0, 0);
            visualizeMaze();
            startSolveTime = Time.realtimeSinceStartup;
            solveMaze(0, 0, new List<direction>(), new List<direction>());

            visualizeMazeSolver();
        }
        //Recursive Division
        if (Input.GetKeyDown("r"))
        {
            InitializeRecursiveDivisionMazeCreationVariables();
            recursiveDivision(0, 0, gridX, gridZ, chooseOrientation(gridX, gridZ));
            visualizeMaze();
            solveMaze(0, 0, new List<direction>(), new List<direction>());
            visualizeMazeSolver();
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
                if (i == 0)
                {
                    grid[i, j].NorthWall = true;
                }
                if (i == grid.GetLength(0) - 1)
                {
                    grid[i, j].SouthWall = true;
                }
                if (j == 0)
                {
                    grid[i, j].WestWall = true;
                }
                if (j == grid.GetLength(1) - 1)
                {
                    grid[i, j].EastWall = true;
                }
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
    List<direction> solvedPath;
    List<direction> triedPath;
    bool solveMaze(int X, int Z, List<direction> SolvingPath, List<direction> VisitedPath)
    {
        // List<direction> new_shit = VisitedPath.co


        //ref no difference

        //is VisitedPath a pointer? Would explain why all paths taken is added. 
        List<direction> localSolvingPath = new List<direction>(SolvingPath);

        localSolvingPath.Add(new direction(X, Z));
        VisitedPath.Add(new direction(X, Z));
        

        direction goalCoordinate = new direction(gridX - 1, gridZ - 1);

        if (X == goalCoordinate.X && Z == goalCoordinate.Z)
        {
            
            solvedPath = localSolvingPath;
            triedPath = VisitedPath;

            print(Time.realtimeSinceStartup - startSolveTime);
            //Solver finished
            return true;
        }
        List<direction> directionCheck = new List<direction>();

        directionCheck.Add(NORTHDir);
        directionCheck.Add(SOUTHDir);
        directionCheck.Add(WESTDir);
        directionCheck.Add(EASTDir);

        //enumerator - is thread on computer. Can make enumerator delayed, so that we can see the pathfinder working, stepping through the paths. Looks cool on video.

        shuffleDirections(directionCheck);
        grid[X, Z].SolverVisited = true;
        foreach (direction dir in directionCheck)
        {
            if ((X + dir.X >= 0 && X + dir.X < gridX) && (Z + dir.Z >= 0 && Z + dir.Z < gridZ))
            {
                if (!grid[X + dir.X, Z + dir.Z].SolverVisited)
                {
                    if (dir == NORTHDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].SouthWall && !grid[X, Z].NorthWall)
                        {
                            if (solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath))
                                return true;
                        }
                    }
                    if (dir == SOUTHDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].NorthWall && !grid[X, Z].SouthWall)
                        {
                            if (solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath))
                                return true;
                        }
                    }
                    if (dir == WESTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].EastWall && !grid[X, Z].WestWall)
                        {
                            if (solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath))
                                return true;
                        }
                    }
                    if (dir == EASTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].WestWall && !grid[X, Z].EastWall)
                        {
                            if (solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath))
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    List<GameObject> visualizedSolverCubes = new List<GameObject>();
    List<GameObject> visualizedTriedCubes = new List<GameObject>();

    void visualizeMazeSolver()
    {
        foreach (GameObject obj in visualizedSolverCubes)
            Destroy(obj);
        foreach (GameObject obj in visualizedTriedCubes)
        {
            Destroy(obj);
        }

        Vector3 solverPos = new Vector3();
        foreach (direction dir in solvedPath)
        {
            solverPos.x = posOffset * dir.X;
            solverPos.z = posOffset * dir.Z;
            visualizedSolverCubes.Add(Instantiate(greenCube, solverPos, Quaternion.identity));
        }
        foreach (direction dir in triedPath)
        {
            solverPos.x = posOffset * dir.X;
            solverPos.z = posOffset * dir.Z;
            visualizedTriedCubes.Add(Instantiate(redCube, solverPos, Quaternion.identity));
        }
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
                //print("i: " + i);
                //print("j: " + j);
                //print("offset: " + wallOffset);
                //print(4 * gridX * gridZ);
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

    int chooseOrientation(int width, int height)
    {
        if (width < height)
            return HORIZONTAL;
        else if (height < width)
            return VERTICAL;
        else
            return ((Random.Range(0, 1) > 0) ? HORIZONTAL : VERTICAL);
    }

    int HORIZONTAL = 0x10;
    int VERTICAL = 0x20;
    int SOUTH = 0x30;
    int EAST = 0x40;

    void recursiveDivision(int X, int Z, int width, int height, int orientation)
    {
        if (width < 2 || height < 2)
            return;

        bool horizontal = (orientation == HORIZONTAL) ? true : false;

        int wx = X + (horizontal ? 0 : Random.Range(0, width - 2));
        int wz = Z + (horizontal ? Random.Range(0, height - 2) : 0);

        int px = wx + (horizontal ? Random.Range(0, width) : 0);
        int pz = wz + (horizontal ? 0 : Random.Range(0, height));

        int dx = horizontal ? 1 : 0;
        int dz = horizontal ? 0 : 1;

        int length = horizontal ? width : height;

        int dir = horizontal ? SOUTH : EAST;

        for (int i = 0; i < length; i++)
        {
            if (wx != px || wz != pz)
            {
                if (dir == SOUTH)
                {
                    grid[wx, wz].EastWall = true;
                }
                if (dir == EAST)
                {
                    grid[wx, wz].SouthWall = true;
                }
            }
            wx += dx;
            wz += dz;
        }

        int nx = X;
        int nz = Z;

        int w = horizontal ? width : wx - X + 1;
        int h = horizontal ? wz - Z + 1 : height;

        recursiveDivision(nx, nz, w, h, chooseOrientation(w, h));

        nx = horizontal ? X : wx + 1;
        nz = horizontal ? wz + 1 : Z;

        w = horizontal ? width : X + width - wx - 1;
        h = horizontal ? Z + height - wz - 1 : height;
        recursiveDivision(nx, nz, w, h, chooseOrientation(w, h));
        //Make path in random spot in new wall.
        //int makeDoorIdx = Random.Range(Z, splitZ);
        //grid[splitX, makeDoorIdx].SouthWall = false;
        //
        ////Ready variables for recursive call
        //int newX = splitX - X + 1;
        //int newZ = Z;
        //
        //recursiveDivision(newX, newZ, width-newX, height);



        //X = top left corner of quadrant
        //Y = top left corner of quadrant
        //width = abs(diff(newX, oldX))
        //height = abs(diff(newY, oldY))
        //recursiveDivision()
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
