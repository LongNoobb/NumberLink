using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector]public bool hasGameFinished;

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
        Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left
    };

    private void Awake()
    {
        instance = this;
        hasGameFinished = false;
        highlightSprite.gameObject.SetActive(false);
        levelGrid = new int[levelData.row, levelData.col];
        cellGrid = new Cell[levelData.row, levelData.col];
        for (int i = 0; i < levelData.row; i++)
        {
            for (int j = 0; j < levelData.col; j++)
            {
                levelGrid[i, j] = levelData.data[i * levelData.row + j];
            }
        }

        SpawnLevel();
    }

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
                tempCell.Init(i,j, levelGrid[i, j]);
                cellGrid[i, j] = tempCell;
                if (tempCell.Number == 0)
                {
                    Destroy(tempCell.gameObject);
                    cellGrid[i, j] = null;
                }
            }
        }

        for(int i = 0; i < levelData.row; i++)
        {
            for(int j = 0;j < levelData.col; j++)
            {
                if (cellGrid[i, j] != null)
                {
                    cellGrid[i, j].Init();
                }
            }
        }

    }

    public Cell GetAdjacentCell(int row, int col, int direction)
    {
        Vector2Int currentDirection = Directions[direction];
        Vector2Int startPos=new Vector2Int(row, col);
        Vector2Int checkPos = startPos + currentDirection;
        while(isValid(checkPos) && cellGrid[checkPos.x, checkPos.y] == null)
        {
            checkPos += currentDirection;
        }
        return isValid(checkPos) ? cellGrid[checkPos.x, checkPos.y] : null;
    }

    public bool isValid(Vector2Int pos)
    {
        return pos.x > 0 && pos.y >= 0 && pos.x < levelData.row && pos.y < levelData.col;
    }
}

[System.Serializable]
public struct LevelData
{
    public int row, col;
    public List<int> data;
}
