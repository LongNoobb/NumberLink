using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector] public bool hasGameFinished;

    public float EdgeSize => cellGap + cellSize;

    [SerializeField] private Cell cellPrefab;
    [SerializeField] private SpriteRenderer backGroundSprite;
    [SerializeField] private SpriteRenderer highlightSprite;
    [SerializeField] private Vector2 highlightSize;
    [SerializeField] private LevelData levelData;
    [SerializeField] private float cellGap;
    [SerializeField] private float cellSize;
    [SerializeField] private float levelGap;

    private int[,] levelGrid;
    private Cell[,] cellGrid;
    private Cell startCell;
    private Vector2 startPos;

    private List<Vector2Int> Directions = new List<Vector2Int>()
    {
        Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    };

    private void Awake()
    {
        //instance = this;
        //hasGameFinished = false;
        //highlightSprite.gameObject.SetActive(false);
        //levelGrid = new int[levelData.row, levelData.col];
        //cellGrid = new Cell[levelData.row, levelData.col];
        //for (int i = 0; i < levelData.row; i++)
        //{
        //    for (int j = 0; j < levelData.col; j++)
        //    {
        //        levelGrid[i, j] = levelData.data[i * levelData.col + j];
        //    }
        //}

        //SpawnLevel();
        instance = this;
        hasGameFinished = false;
        highlightSprite.gameObject.SetActive(false);

        GenerateLevel(3, 3, extraEdges: 2);

        levelGrid = new int[levelData.row, levelData.col];
        cellGrid = new Cell[levelData.row, levelData.col];
        for (int i = 0; i < levelData.row; i++)
            for (int j = 0; j < levelData.col; j++)
                levelGrid[i, j] = levelData.data[i * levelData.col + j];

        SpawnLevel();
    }



    //private void Awake()
    //{
    //    instance = this;
    //    hasGameFinished = false;
    //    highlightSprite.gameObject.SetActive(false);


    //    levelData = GenerateRandomLevel(4, 4, 3); 

    //    levelGrid = new int[levelData.row, levelData.col];
    //    cellGrid = new Cell[levelData.row, levelData.col];

    //    for (int i = 0; i < levelData.row; i++)
    //    {
    //        for (int j = 0; j < levelData.col; j++)
    //        {
    //            levelGrid[i, j] = levelData.data[i * levelData.col + j];
    //        }
    //    }

    //    SpawnLevel();
    //}

    private void SpawnLevel()
    {
        float width = (cellSize + cellGap) * levelData.col - cellGap + levelGap;
        float height = (cellSize + cellGap) * levelData.row - cellGap + levelGap;
        backGroundSprite.size = new Vector2(width, height);
        Vector3 backGroundPos = new Vector3(
            width / 2f - cellSize / 2f - levelGap / 2f,
            height / 2f - cellSize / 2f - levelGap / 2f,
            0
            );
        backGroundSprite.transform.position = backGroundPos;

        Camera.main.orthographicSize = width * 1.2f;
        Camera.main.transform.position = new Vector3(backGroundPos.x, backGroundPos.y, -10f);

        Vector3 startPos = Vector3.zero;
        Vector3 rightOffset = Vector3.right * (cellSize + cellGap);
        Vector3 topOffset = Vector3.up * (cellSize + cellGap);

        for (int i = 0; i < levelData.row; i++)
        {
            for (int j = 0; j < levelData.col; j++)
            {
                Vector3 spawnPos = startPos + j * rightOffset + i * topOffset;
                Cell tempCell = Instantiate(cellPrefab, spawnPos, Quaternion.identity);
                tempCell.Init(i, j, levelGrid[i, j]);
                cellGrid[i, j] = tempCell;
                if (tempCell.Number == 0)
                {
                    Destroy(tempCell.gameObject);
                    cellGrid[i, j] = null;
                }
            }
        }

        for (int i = 0; i < levelData.row; i++)
        {
            for (int j = 0; j < levelData.col; j++)
            {
                if (cellGrid[i, j] != null)
                {
                    cellGrid[i, j].Init();
                }
            }
        }

    }

    private void Update()
    {
        if (hasGameFinished) return;
        if (Input.GetMouseButtonDown(0))
        {
            startCell = null;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            startPos = mousePos2D;
            if (hit && hit.collider.TryGetComponent(out startCell))
            {
                highlightSprite.gameObject.SetActive(true);
                highlightSprite.size = highlightSize;
                highlightSprite.transform.position = startCell.transform.position;
            }
            else
            {
                hit = Physics2D.Raycast(mousePos, Vector2.left);
                if (hit && hit.collider.TryGetComponent(out startCell))
                {
                    startCell.RemoveEdge(0);
                }
                hit = Physics2D.Raycast(mousePos, Vector2.down);
                if (hit && hit.collider.TryGetComponent(out startCell))
                {
                    startCell.RemoveEdge(1);
                }
                hit = Physics2D.Raycast(mousePos, Vector2.right);
                if (hit && hit.collider.TryGetComponent(out startCell))
                {
                    startCell.RemoveEdge(2);
                }
                hit = Physics2D.Raycast(mousePos, Vector2.up);
                if (hit && hit.collider.TryGetComponent(out startCell))
                {
                    startCell.RemoveEdge(3);
                }
                startCell = null;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (startCell == null) return;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            Vector2 offset = mousePos2D - startPos;
            Vector2Int offsetDirection = GetDirection(offset);
            float offsetValue = GetOffSet(offset, offsetDirection);
            int directionIndex = GetDirectionIndex(offsetDirection);
            Vector3 angle = new Vector3(0, 0, 90f * (directionIndex - 1));
            highlightSprite.size = new Vector2(highlightSize.x, offsetValue);
            highlightSprite.transform.eulerAngles = angle;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (startCell == null) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit && hit.collider.TryGetComponent(out Cell endCell))
            {
                if (endCell == startCell)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var adjacentCell = GetAdjacentCell(startCell.Row, startCell.Column, i);
                        if (adjacentCell != null)
                        {
                            int adjacentDirection = (i + 2) % 4;
                            startCell.RemoveEdge(i);
                            adjacentCell.RemoveEdge(adjacentDirection);
                            //adjacentCell.RemoveEdge(adjacentDirection);

                        }
                    }
                }
                else
                {
                    Vector2 offset = mousePos2D - startPos;
                    Vector2Int offsetDirection = GetDirection(offset);
                    int directionIndex = GetDirectionIndex(offsetDirection);
                    if (startCell.isValidCell(endCell, directionIndex))
                    {
                        startCell.AddEdge(directionIndex);
                        endCell.AddEdge((directionIndex + 2) % 4);
                    }
                }
            }
            startCell = null;
            highlightSprite.gameObject.SetActive(false);
            checkWin();
        }
    }

    private void checkWin()
    {
        for (int i = 0; i < levelData.row; i++)
        {
            for (int j = 0; j < levelData.col; j++)
            {
                if (cellGrid[i, j] != null && cellGrid[i, j].Number != 0) return;
            }
        }
        hasGameFinished = true;
        if (hasGameFinished)
        {
            
            GameObject[] game = GameObject.FindGameObjectsWithTag("Cell");
            int count = game.Count();
            if (count!=0)
            {
                foreach(GameObject g in game)
                {
                    Destroy(g);
                }
            }
            Awake();
        }
    }

    private int GetDirectionIndex(Vector2Int offsetDirection)
    {
        int result = 0;
        if (offsetDirection == Vector2Int.right)
        {
            result = 0;
        }
        if (offsetDirection == Vector2Int.left)
        {
            result = 2;
        }
        if (offsetDirection == Vector2Int.up)
        {
            result = 1;
        }
        if (offsetDirection == Vector2Int.down)
        {
            result = 3;
        }
        return result;
    }

    private float GetOffSet(Vector2 offset, Vector2Int offsetDirection)
    {
        float result = 0;
        if (offsetDirection == Vector2Int.left || offsetDirection == Vector2Int.right)
        {
            result = Mathf.Abs(offset.x);
        }
        if (offsetDirection == Vector2Int.up || offsetDirection == Vector2Int.down)
        {
            result = Mathf.Abs(offset.y);
        }
        return result;
    }

    private Vector2Int GetDirection(Vector2 offset)
    {
        Vector2Int result = Vector2Int.zero;
        if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x) && offset.y > 0)
        {
            result = Vector2Int.up;
        }
        if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x) && offset.y < 0)
        {
            result = Vector2Int.down;
        }
        if (Mathf.Abs(offset.y) < Mathf.Abs(offset.x) && offset.x > 0)
        {
            result = Vector2Int.right;
        }
        if (Mathf.Abs(offset.y) < Mathf.Abs(offset.x) && offset.x < 0)
        {
            result = Vector2Int.left;
        }
        return result;
    }

    public Cell GetAdjacentCell(int row, int col, int direction)
    {
        Vector2Int currentDirection = Directions[direction];
        Vector2Int startPos = new Vector2Int(row, col);
        Vector2Int checkPos = startPos + currentDirection;
        while (isValid(checkPos) && cellGrid[checkPos.x, checkPos.y] == null)
        {
            checkPos += currentDirection;
        }
        return isValid(checkPos) ? cellGrid[checkPos.x, checkPos.y] : null;
    }

    public bool isValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < levelData.row && pos.y < levelData.col;
    }

    //public LevelData GenerateRandomLevel(int row, int col, int maxLinksPerCell)
    //{
    //    LevelData level = new LevelData();
    //    level.row = row;
    //    level.col = col;
    //    level.data = new List<int>();

    //    int[,] linkCounts = new int[row, col];
    //    bool[,] visited = new bool[row, col];

    //    // Possible directions
    //    List<Vector2Int> directions = new List<Vector2Int>() {
    //    Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
    //};

    //    System.Random rand = new System.Random();

    //    // Randomly connect neighboring cells
    //    for (int i = 0; i < row; i++)
    //    {
    //        for (int j = 0; j < col; j++)
    //        {
    //            foreach (var dir in directions)
    //            {
    //                int ni = i + dir.x;
    //                int nj = j + dir.y;

    //                if (ni >= 0 && nj >= 0 && ni < row && nj < col)
    //                {
    //                    if (rand.NextDouble() < 0.4) // 40% chance to link
    //                    {
    //                        if (linkCounts[i, j] < maxLinksPerCell && linkCounts[ni, nj] < maxLinksPerCell)
    //                        {
    //                            linkCounts[i, j]++;
    //                            linkCounts[ni, nj]++;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    // Flatten to list
    //    for (int i = 0; i < row; i++)
    //    {
    //        for (int j = 0; j < col; j++)
    //        {
    //            level.data.Add(linkCounts[i, j]);
    //        }
    //    }

    //    return level;
    //}



    private List<(Vector2Int, Vector2Int)> GenerateSpanningTree(int row, int col, System.Random rand)
    {
        bool[,] visited = new bool[row, col];
        var directions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        var edges = new List<(Vector2Int, Vector2Int)>();
        var stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(rand.Next(row), rand.Next(col));
        visited[start.x, start.y] = true;
        stack.Push(start);

        while (stack.Count > 0)
        {
            var cur = stack.Pop();
           
            var dirs = directions.OrderBy(_ => rand.Next()).ToArray();
            foreach (var d in dirs)
            {
                var nxt = cur + d;
                if (nxt.x >= 0 && nxt.y >= 0 && nxt.x < row && nxt.y < col && !visited[nxt.x, nxt.y])
                {
                    visited[nxt.x, nxt.y] = true;
                    edges.Add((cur, nxt));
                    stack.Push(nxt);
                }
            }
        }

        return edges;
    }

    private List<(Vector2Int, Vector2Int)> AddExtraEdges(
    List<(Vector2Int, Vector2Int)> treeEdges, int row, int col, int extraCount, System.Random rand)
    {
        var allPossible = new HashSet<string>();
        foreach (var e in treeEdges)
        {
            string key = $"{e.Item1.x},{e.Item1.y}-{e.Item2.x},{e.Item2.y}";
            allPossible.Add(key);
            allPossible.Add($"{e.Item2.x},{e.Item2.y}-{e.Item1.x},{e.Item1.y}");
        }

        var directions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        int attempts = 0;
        while (extraCount > 0 && attempts < row * col * 10)
        {
            attempts++;
            int x = rand.Next(row), y = rand.Next(col);
            var dir = directions[rand.Next(4)];
            var nx = x + dir.x;
            var ny = y + dir.y;
            if (nx < 0 || ny < 0 || nx >= row || ny >= col) continue;
            string key = $"{x},{y}-{nx},{ny}";
            if (allPossible.Contains(key)) continue;
            allPossible.Add(key);
            allPossible.Add($"{nx},{ny}-{x},{y}");
            treeEdges.Add((new Vector2Int(x, y), new Vector2Int(nx, ny)));
            extraCount--;
        }

        return treeEdges;
    }

    private void GenerateLevel(int row, int col, int extraEdges)
    {
        var rand = new System.Random();

        var edges = GenerateSpanningTree(row, col, rand);

        edges = AddExtraEdges(edges, row, col, extraEdges, rand);


        int[,] connectionGrid = new int[row, col];
        foreach (var (a, b) in edges)
        {
            connectionGrid[a.x, a.y]++;
            connectionGrid[b.x, b.y]++;
        }


        levelData.row = row;
        levelData.col = col;
        levelData.data = new List<int>(row * col);
        for (int i = 0; i < row; i++)
            for (int j = 0; j < col; j++)
                levelData.data.Add(connectionGrid[i, j]);
    }


}

[System.Serializable]
public struct LevelData
{
    public int row, col;
    public List<int> data;
}
