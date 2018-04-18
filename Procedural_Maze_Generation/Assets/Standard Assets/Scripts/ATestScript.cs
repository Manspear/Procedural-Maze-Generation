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

    private static IntVector2 NORTHDir = new IntVector2(-1,0);
    private static IntVector2 SOUTHDir = new IntVector2(1,0);
    private static IntVector2 EASTDir = new IntVector2(0, 1);
    private static IntVector2 WESTDir = new IntVector2(0, -1);

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

    public class IntVector2
    {
        public int X, Z;
        public IntVector2(int inX, int inZ)
        {
            this.X = inX;
            this.Z = inZ;
        }
    }

    // Use this for initialization
    void Start() {

        gridResolutions.Add(new IntVector2(32, 32));
        gridResolutions.Add(new IntVector2(64, 64));
        gridResolutions.Add(new IntVector2(128, 128));
        gridResolutions.Add(new IntVector2(256, 256));

        visualizedMaze = new GameObject[4 * gridX * gridZ];
        //recursiveBackTrack(0, 0);
        //visualizeMaze();

    }

    public List<IntVector2> gridResolutions = new List<IntVector2>();
    public int numberOfTestsPerResolution = 30;
    

    void writeDataToFile()
    {

    }
    void doTest()
    {
        //Create 1 maze, extract data, create new maze, extract data, etc...
        for (int j = 0; j < 3; j++)
        {
            foreach (IntVector2 item in gridResolutions)
            {
                //0 == recursive Division
                //
                //
                gridX = item.X;
                gridZ = item.Z;
                
                for (int i = 0; i < numberOfTestsPerResolution; i++)
                {
                    if (j == 0)
                        InitializeRecursiveDivisionMazeCreationVariables();
                    else
                        InitializeMazeCreationVariables();

                    if(j == 0)
                        recursiveDivision(0, 0, gridX, gridZ, chooseOrientation(gridX, gridZ));
                    if(j == 1)
                        recursiveBackTrack(0, 0);
                    if (j == 2)
                        primsAlgorithm(0, 0);

                    startSolveTime = Time.realtimeSinceStartup;
                    solverBack solveTime = solveMaze(0, 0, new List<IntVector2>(), new List<IntVector2>());
                    AddPathLengths();
                }
                AnalyzePathLengthData();
            }
        }
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
            startSolveTime = Time.realtimeSinceStartup;
            solveMaze(0, 0, new List<IntVector2>(), new List<IntVector2>());
            visualizeMazeSolver();
            AddPathLengths();
        }
        //Prim's algorithm
        if (Input.GetKeyDown("e"))
        {
            InitializeMazeCreationVariables();
            primsAlgorithm(0, 0);
            visualizeMaze();
            startSolveTime = Time.realtimeSinceStartup;
            solveMaze(0, 0, new List<IntVector2>(), new List<IntVector2>());

            visualizeMazeSolver();
        }
        //Recursive Division
        if (Input.GetKeyDown("r"))
        {
            InitializeRecursiveDivisionMazeCreationVariables();
            recursiveDivision(0, 0, gridX, gridZ, chooseOrientation(gridX, gridZ));
            visualizeMaze();
            startSolveTime = Time.realtimeSinceStartup;
            solveMaze(0, 0, new List<IntVector2>(), new List<IntVector2>());
            visualizeMazeSolver();
        }
    }

    
    List<int> pathLengths = new List<int>();

    void AddPathLengths()
    {
        int longestPath = 0;
        int tempPath = 0;
        

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                //Horizontal Path
                if (grid[i, j].EastWall)
                {
                    if (tempPath != 0)
                    {
                        tempPath++;
                        pathLengths.Add(tempPath);
                    }
                        
                    tempPath = 0;
                }
                else
                {
                    tempPath++;
                    if (tempPath > longestPath)
                        longestPath = tempPath;
                }
            }
        }

        tempPath = 0;

        for (int i = 0; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                //Vertical Path
                if (grid[i, j].SouthWall)
                {
                    if (tempPath != 0)
                    {
                        tempPath++;
                        pathLengths.Add(tempPath);
                    }
                    tempPath = 0;
                }
                else
                {
                    tempPath++;
                    if (tempPath > longestPath)
                        longestPath = tempPath;
                }
                  
            }
        }
    }


    void AnalyzePathLengthData()
    {
        float meanPathLength = 0;
        float medianPathLength = 0;

        pathLengths.Sort();

        float totPathLengths = 0;

        foreach (int path in pathLengths)
        {
            totPathLengths += path;
        }
        meanPathLength = totPathLengths / pathLengths.Count;
        medianPathLength = pathLengths[pathLengths.Count / 2];

        float stdDeviation = 0;
        for (int i = 0; i < pathLengths.Count; i++)
        {
            stdDeviation += Mathf.Pow(pathLengths[i] - meanPathLength, 2);
        }
        stdDeviation = Mathf.Sqrt(stdDeviation / pathLengths.Count);

        print("Average Path Length: " + meanPathLength);
        print("Median Path Length: " + medianPathLength);
        print("Standard Deviation: " + stdDeviation);
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

    class solverBack
    {
        public bool isSolved = false;
        public float solveTime = 0;
        
        public solverBack(bool isSolved, float solveTime)
        {
            this.isSolved = isSolved;
            this.solveTime = solveTime;
        }
    }

    List<IntVector2> solvedPath;
    List<IntVector2> triedPath;
    solverBack solveMaze(int X, int Z, List<IntVector2> SolvingPath, List<IntVector2> VisitedPath)
    {
        // List<direction> new_shit = VisitedPath.co


        //ref no difference

        //is VisitedPath a pointer? Would explain why all paths taken is added. 
        List<IntVector2> localSolvingPath = new List<IntVector2>(SolvingPath);

        localSolvingPath.Add(new IntVector2(X, Z));
        VisitedPath.Add(new IntVector2(X, Z));
        

        IntVector2 goalCoordinate = new IntVector2(gridX - 1, gridZ - 1);

        if (X == goalCoordinate.X && Z == goalCoordinate.Z)
        {
            
            solvedPath = localSolvingPath;
            triedPath = VisitedPath;

            //Solver finished
            return new solverBack(true, Time.realtimeSinceStartup - startSolveTime);
        }
        List<IntVector2> directionCheck = new List<IntVector2>();

        directionCheck.Add(NORTHDir);
        directionCheck.Add(SOUTHDir);
        directionCheck.Add(WESTDir);
        directionCheck.Add(EASTDir);

        //enumerator - is thread on computer. Can make enumerator delayed, so that we can see the pathfinder working, stepping through the paths. Looks cool on video.

        shuffleDirections(directionCheck);
        grid[X, Z].SolverVisited = true;
        foreach (IntVector2 dir in directionCheck)
        {
            if ((X + dir.X >= 0 && X + dir.X < gridX) && (Z + dir.Z >= 0 && Z + dir.Z < gridZ))
            {
                if (!grid[X + dir.X, Z + dir.Z].SolverVisited)
                {
                    if (dir == NORTHDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].SouthWall && !grid[X, Z].NorthWall)
                        {
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == SOUTHDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].NorthWall && !grid[X, Z].SouthWall)
                        {
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == WESTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].EastWall && !grid[X, Z].WestWall)
                        {
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == EASTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].WestWall && !grid[X, Z].EastWall)
                        {
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                }
            }
        }
        return new solverBack(false, 0);
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
        foreach (IntVector2 dir in solvedPath)
        {
            solverPos.x = posOffset * dir.X;
            solverPos.z = posOffset * dir.Z;
            visualizedSolverCubes.Add(Instantiate(greenCube, solverPos, Quaternion.identity));
        }
        foreach (IntVector2 dir in triedPath)
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

    void shuffleDirections(List<IntVector2> directionList)
    {
        //Shuffles directionList
        int n = directionList.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n);
            n--;
            IntVector2 value = directionList[k];
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

    void carveInDirection(IntVector2 dir, int gridXPos, int gridZPos)
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
        List<IntVector2> directionList = new List<IntVector2>();
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
            
            foreach (IntVector2 dir in directionList)
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
        List<IntVector2> directionList = new List<IntVector2>();
        //initializing list of directions 
        directionList.Add(NORTHDir);
        directionList.Add(SOUTHDir);
        directionList.Add(WESTDir);
        directionList.Add(EASTDir);

        shuffleDirections(directionList);

        //set current cell as visited
        grid[gridXPos, gridZPos].Visited = true;

        //Now loop through neighbours. If found neighbour, call self, with neighbours position
        foreach (IntVector2 dir in directionList)
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
