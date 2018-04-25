﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class MazeGenerationScript : MonoBehaviour {

    float posOffset = 8.6f;
    public GameObject northWall;
    public GameObject southWall;
    public GameObject westWall;
    public GameObject eastWall;
    public GameObject redCube;
    public GameObject greenCube;
    public GameObject pinkCube;

    public int pGridX = 5;
    public int pGridZ = 5;
    int gridX = 5;
    int gridZ = 10;
    public static cell[,] grid;
    public int seedNumber = 0;
    public bool randomSeed = false;

    private static IntVector2 NORTHDir = new IntVector2(-1, 0);
    private static IntVector2 SOUTHDir = new IntVector2(1, 0);
    private static IntVector2 EASTDir = new IntVector2(0, 1);
    private static IntVector2 WESTDir = new IntVector2(0, -1);

    public class cell
    {
        public bool AddedToFrontier = false;
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

        directionCheck.Add(NORTHDir);
        directionCheck.Add(SOUTHDir);
        directionCheck.Add(WESTDir);
        directionCheck.Add(EASTDir);

        gridResolutions.Add(new IntVector2(32, 32));
        gridResolutions.Add(new IntVector2(64, 64));//
        gridResolutions.Add(new IntVector2(128, 128));
        //gridResolutions.Add(new IntVector2(256, 256));

        visualizedMaze = new GameObject[1];
        //recursiveBackTrack(0, 0);
        //visualizeMaze();
        //doTest();

    }

    public List<IntVector2> gridResolutions = new List<IntVector2>();
    public int numberOfTestsPerResolution = 30;

    void writeDataToFile(List<int> pathLengths, float solveTime, IntVector2 gridResolution, int mazeAlgorithm, int seed, string testPath, string statisticsPath)
    {
        pathLengths.Sort();

        //Begin testWriter
        //---------------------------------------------------------------

        StreamWriter testWriter = new StreamWriter(testPath, true);

        int shortPathThreshhold = (int)(gridResolution.X * 0.1f);
        int mediumPathThreshhold = (int)(gridResolution.X * 0.3f);

        int shortCorridors = 0;
        int mediumCorridors = 0;
        int longCorridors = 0;

        for (int i = 1; i < pathLengths.Count; i++)
        {
            if (pathLengths[i] < shortPathThreshhold)
                shortCorridors++;
            else if (pathLengths[i] < mediumPathThreshhold)
                mediumCorridors++;
            else
                longCorridors++;
        }

        switch (mazeAlgorithm)
        {
            case RECURSIVEDIVISION:
                testWriter.WriteLine("RecursiveDivision," + gridResolution.X + "x" + gridResolution.Z + "," + solveTime + "," + shortCorridors + "," + mediumCorridors + "," + longCorridors + "," + seed);
                break;
            case RECURSIVEBACKTRACKER:
                testWriter.WriteLine("RecursiveBacktracker," + gridResolution.X + "x" + gridResolution.Z + "," + solveTime + "," + shortCorridors + "," + mediumCorridors + "," + longCorridors + "," + seed);
                break;
            case PRIMS:
                testWriter.WriteLine("Prims," + gridResolution.X + "x" + gridResolution.Z + "," + solveTime + "," + shortCorridors + "," + mediumCorridors + "," + longCorridors + "," + seed);
                break;
        }

        testWriter.Close();
        //End testWriter
        //------------------------------------------------------------------

    }

    void writeStats(int mazeAlgorithm, IntVector2 gridResolution, string statisticsPath)
    {
        //Begin Statwriting
        string statString = "";
        if (mazeAlgorithm == RECURSIVEDIVISION)
            statString = "RecursiveDivision";
        else if (mazeAlgorithm == RECURSIVEBACKTRACKER)
            statString = "RecursiveBacktracker";
        else if (mazeAlgorithm == PRIMS)
            statString = "Prims";

        statString += "," + gridResolution.X + "x" + gridResolution.Z;

        foreach (int item in pathLengths)
        {
            corridorLengths[item - 1]++;
        }

        for (int i = 1; i < biggestResolution; i++)
        {
            statString += "," + ((corridorLengths[i] / (float)pathLengths.Count) * 100) + "%";
        }

        for (int i = 0; i < corridorLengths.Length; i++)
        {
            corridorLengths[i] = 0;
        }
        StreamWriter statWriter = new StreamWriter(statisticsPath, true);
        statWriter.WriteLine(statString);

        statWriter.Close();
    }



    string testFilePath = "Assets/Data/mazeTest.csv";
    string statisticsFilePath = "Assets/Data/mazeStats.csv";

    const int RECURSIVEDIVISION = 0;
    const int RECURSIVEBACKTRACKER = 1;
    const int PRIMS = 2;

    int[] corridorLengths;
    int biggestResolution = 0;

    void doTest()
    {
        File.WriteAllText(testFilePath, string.Empty);
        File.WriteAllText(statisticsFilePath, string.Empty);
        StreamWriter writer = new StreamWriter(testFilePath, true);
        StreamWriter statWriter = new StreamWriter(statisticsFilePath, true);
        writer.WriteLine("Maze Type, Resolution, Solving Time, Short Corridors, Medium Corridors, Long Corridors, Seed");
        string statString = "Maze Type, Resolution";

        foreach (IntVector2 res in gridResolutions)
        {
            if (biggestResolution < res.X)
                biggestResolution = res.X;
        }
        for (int i = 1; i < biggestResolution; i++)
        {
            statString += "," + (i + 1);
        }
        corridorLengths = new int[biggestResolution];
        statWriter.WriteLine(statString);
        writer.Close();
        statWriter.Close();
        // foreach (IntVector2 resolution in gridResolutions)
        // {
        //     string path = "Assets/Data/Test" + resolution.X + "x" + resolution.Z + ".csv";
        //     File.WriteAllText(path, string.Empty);
        //     StreamWriter writer = new StreamWriter(path, true);
        //     writer.WriteLine("sep=,");
        //     writer.Close();
        // }
        //Create 1 maze, extract data, create new maze, extract data, etc...
        for (int j = 0; j < 3; j++)
        {
            pathLengths.Clear();
            foreach (IntVector2 resolution in gridResolutions)
            {
                //0 == recursive Division
                //
                //
                gridX = resolution.X;
                gridZ = resolution.Z;

                for (int i = 0; i < numberOfTestsPerResolution; i++)
                {
                    if (j == 0)
                        InitializeRecursiveDivisionMazeCreationVariables();
                    else
                        InitializeMazeCreationVariables();

                    switch (j)
                    {
                        case RECURSIVEDIVISION:
                            recursiveDivision(0, 0, gridX, gridZ, chooseOrientation(gridX, gridZ));
                            break;
                        case RECURSIVEBACKTRACKER:
                            recursiveBackTrack(0, 0);
                            break;
                        case PRIMS:
                            primsAlgorithm(0, 0);
                            break;
                    }

                    startSolveTime = Time.realtimeSinceStartup;
                    solverBack solveTime = solveMaze(0, 0);
                    //solvingTime.Add(solveTime.solveTime);
                    //seedNumbers.Add(seedNumber);
                    AddPathLengths();
                    writeDataToFile(pathLengths, solveTime.solveTime, resolution, j, seedNumber, testFilePath, statisticsFilePath);
                    writeStats(j, resolution, statisticsFilePath);
                    pathLengths.Clear();
                    pathLengths.TrimExcess();
                }

            }
        }
    }

    float startSolveTime;

    void Update()
    {
        float temp = Time.realtimeSinceStartup;

        if (Input.GetKeyDown("t"))
        {
            doTest();
            
        }
        //recursive backtracker
        if (Input.GetKeyDown("q"))
        {
            foreach (GameObject item in visualizedMaze)
            {
                Destroy(item);
            }
            visualizedMaze = new GameObject[4 * pGridX * pGridZ];
            gridX = pGridX;
            gridZ = pGridZ;
            InitializeMazeCreationVariables();
            recursiveBackTrack(0, 0);
            visualizeMaze();
            startSolveTime = Time.realtimeSinceStartup;
            solveMazeReadyForVisualization(0, 0, new List<IntVector2>(), new List<IntVector2>());
            visualizeMazeSolver();
            AddPathLengths();
        }
        //Prim's algorithm
        if (Input.GetKeyDown("e"))
        {
            foreach (GameObject item in visualizedMaze)
            {
                Destroy(item);
            }

            visualizedMaze = new GameObject[4 * pGridX * pGridZ];
            gridX = pGridX;
            gridZ = pGridZ;
            InitializeMazeCreationVariables();
            primsAlgorithm(0, 0);
            visualizeMaze();
            startSolveTime = Time.realtimeSinceStartup;
            solveMazeReadyForVisualization(0, 0, new List<IntVector2>(), new List<IntVector2>());

            visualizeMazeSolver();
        }
        //Recursive Division
        if (Input.GetKeyDown("r"))
        {
            foreach (GameObject item in visualizedMaze)
            {
                Destroy(item);
            }
            visualizedMaze = new GameObject[4 * pGridX * pGridZ];
            gridX = pGridX;
            gridZ = pGridZ;
            InitializeRecursiveDivisionMazeCreationVariables();
            recursiveDivision(0, 0, gridX, gridZ, chooseOrientation(gridX, gridZ));
            visualizeMaze();
            startSolveTime = Time.realtimeSinceStartup;
            solveMazeReadyForVisualization(0, 0, new List<IntVector2>(), new List<IntVector2>());
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
    List<IntVector2> directionCheck = new List<IntVector2>();

    solverBack solveMazeReadyForVisualization(int X, int Z, List<IntVector2> SolvingPath, List<IntVector2> VisitedPath)
    {
        List<IntVector2> lDirectionCheck = new List<IntVector2>();
        lDirectionCheck.Add(NORTHDir);
        lDirectionCheck.Add(SOUTHDir);
        lDirectionCheck.Add(WESTDir);
        lDirectionCheck.Add(EASTDir);
        // List<direction> new_shit = VisitedPath.co


        //ref no difference

        //is VisitedPath a pointer? Would explain why all paths taken is added. 
        List<IntVector2> localSolvingPath = new List<IntVector2>(SolvingPath);

        localSolvingPath.Add(new IntVector2(X, Z));
        VisitedPath.Add(new IntVector2(X, Z));

        {
            IntVector2 goalCoordinate = new IntVector2(gridX - 1, gridZ - 1);

            if (X == goalCoordinate.X && Z == goalCoordinate.Z)
            {

                solvedPath = localSolvingPath;
                triedPath = VisitedPath;

                //Solver finished. Convert from seconds to milliseconds
                return new solverBack(true, (Time.realtimeSinceStartup - startSolveTime) * 1000f);
            }
        }

        //enumerator - is thread on computer. Can make enumerator delayed, so that we can see the pathfinder working, stepping through the paths. Looks cool on video.

        shuffleDirections(lDirectionCheck);
        grid[X, Z].SolverVisited = true;
        foreach (IntVector2 dir in lDirectionCheck)
        {
            if ((X + dir.X >= 0 && X + dir.X < gridX) && (Z + dir.Z >= 0 && Z + dir.Z < gridZ))
            {
                if (!grid[X + dir.X, Z + dir.Z].SolverVisited)
                {
                    if (dir == NORTHDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].SouthWall && !grid[X, Z].NorthWall)
                        {
                            solverBack mongo = solveMazeReadyForVisualization(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == SOUTHDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].NorthWall && !grid[X, Z].SouthWall)
                        {
                            solverBack mongo = solveMazeReadyForVisualization(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == WESTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].EastWall && !grid[X, Z].WestWall)
                        {
                            solverBack mongo = solveMazeReadyForVisualization(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == EASTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].WestWall && !grid[X, Z].EastWall)
                        {
                            solverBack mongo = solveMazeReadyForVisualization(X + dir.X, Z + dir.Z, localSolvingPath, VisitedPath);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                }
            }
        }
        return new solverBack(false, 0);
    }

    solverBack solveMaze(int X, int Z)
    {
        //If at goal
        if (X == gridX - 1 && Z == gridZ - 1)
        {
            //Solver finished. Convert from seconds to milliseconds
            return new solverBack(true, (Time.realtimeSinceStartup - startSolveTime) * 1000f);
        }

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
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == SOUTHDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].NorthWall && !grid[X, Z].SouthWall)
                        {
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == WESTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].EastWall && !grid[X, Z].WestWall)
                        {
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z);
                            if (mongo.isSolved)
                                return mongo;
                        }
                    }
                    if (dir == EASTDir)
                    {
                        if (!grid[X + dir.X, Z + dir.Z].WestWall && !grid[X, Z].EastWall)
                        {
                            solverBack mongo = solveMaze(X + dir.X, Z + dir.Z);
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

    void visualizePrimSolver(List<IntVector2> frontier, IntVector2 currFrontier)
    {
        foreach (GameObject obj in visualizedSolverCubes)
            DestroyImmediate(obj);
        foreach (GameObject obj in visualizedTriedCubes)
            DestroyImmediate(obj);

        Vector3 aids = new Vector3();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                aids.x = i * posOffset;
                aids.z = j * posOffset;

                if (grid[i, j].Visited)
                    visualizedSolverCubes.Add(Instantiate(redCube, aids, Quaternion.identity));
            }
        }

        foreach (IntVector2 item in frontier)
        {
            aids.x = posOffset * item.X;
            aids.z = posOffset * item.Z;
            visualizedSolverCubes.Add(Instantiate(greenCube, aids, Quaternion.identity));
        }

        aids.x = posOffset * currFrontier.X;
        aids.z = posOffset * currFrontier.Z;
        visualizedSolverCubes.Add(Instantiate(pinkCube, aids, Quaternion.identity));
    }

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

    public class mazeLoc
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
            //grid[gridXPos + dir.X, gridZPos + dir.Z].Visited = true;

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
            //recursiveBackTrack(gridXPos + dir.X, gridZPos + dir.Z);
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

        List<IntVector2> frontier = new List<IntVector2>();
        List<mazeLoc> inCells = new List<mazeLoc>();

        //Add directions
        List<IntVector2> directionList = new List<IntVector2>();
        //initializing list of directions 
        directionList.Add(NORTHDir);
        directionList.Add(SOUTHDir);
        directionList.Add(WESTDir);
        directionList.Add(EASTDir);
        
        //frontier.Add(new mazeLoc(startX, startZ));
        inCells.Add(new mazeLoc(startX, startZ));
        grid[startX, startZ].Visited = true;

        foreach (IntVector2 dir in directionList)
        {
            //Add valid adjacent cells to frontier. 
            if ((startX + dir.X >= 0 && (startX + dir.X < gridX)) && (startZ + dir.Z >= 0 && startZ + dir.Z < gridZ))
            {
                if (grid[startX + dir.X, startZ + dir.Z].Visited == false && grid[startX + dir.X, startZ + dir.Z].AddedToFrontier == false)
                {
                    grid[startX + dir.X, startZ + dir.Z].AddedToFrontier = true;
                    frontier.Add(new IntVector2(startX + dir.X, startZ + dir.Z));
                }
            }
        }

        visualizePrimSolver(frontier, new IntVector2(0,0));

        while (frontier.Count > 0)
        {
            //Get current frontier cell coordinate
            shuffleDirections(directionList);
            int CFI = Random.Range(0, frontier.Count);
            //A) Choose random frontier cell from frontier-list
            IntVector2 CFC = new IntVector2(frontier[CFI].X, frontier[CFI].Z);
            grid[CFC.X, CFC.Z].Visited = true;
            //frontier.Remove(frontier[CFI]);

            foreach (IntVector2 dir in directionList)
            { 
                //Add valid adjacent cells to frontier. 
                if (IsDirCoordValid(CFC, dir))
                {
                    if (grid[CFC.X + dir.X, CFC.Z + dir.Z].Visited == false && grid[CFC.X + dir.X, CFC.Z + dir.Z].AddedToFrontier == false)
                    {
                        grid[CFC.X + dir.X, CFC.Z + dir.Z].AddedToFrontier = true;
                        frontier.Add(new IntVector2(CFC.X + dir.X, CFC.Z + dir.Z));
                    }
                }
            }

            foreach (IntVector2 dir in directionList)
            {
                //If cell adjacent to this frontier has already been visited, carve path to it. (only once per frontier-loop)
                if (IsDirCoordValid(CFC, dir))
                {
                    if (grid[CFC.X + dir.X, CFC.Z + dir.Z].Visited == true)
                    {
                        carveInDirection(dir, CFC.X, CFC.Z);
                        break;
                    }
                }
            }

            frontier.RemoveAt(CFI);
            //if (frontier.Remove(CFC))
            //    print(CFC.X + " " + CFC.Z);
            //visualizeMaze();
            //visualizePrimSolver(frontier, CFC);
        }
    }

    bool IsDirCoordValid(IntVector2 coordinate, IntVector2 dir)
    {
        if ((coordinate.X + dir.X >= 0 && (coordinate.X + dir.X < gridX)) && (coordinate.Z + dir.Z >= 0 && coordinate.Z + dir.Z < gridZ))
        {
            return true;
        }
        else
            return false;
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
