using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cell : MonoBehaviour
{
    [HideInInspector]
    public int Number
    {
        get { return number; }
        set
        {
            number = value;
            numberText.text = number.ToString();
            if (number == 0)
            {
                cellSprite.color = solvedColor;
                numberText.gameObject.SetActive(false);
            }
            else if (number < 0)
            {
                cellSprite.color = incorrectColor;
                numberText.gameObject.SetActive(false);
            }
            else
            {
                cellSprite.color = defaultColor;
                numberText.gameObject.SetActive(true);
            }
        }
    }

    [HideInInspector] public int Row;
    [HideInInspector] public int Column;

    [SerializeField] private TMP_Text numberText;
    [SerializeField] private SpriteRenderer cellSprite;

    [SerializeField] private GameObject right1;
    [SerializeField] private GameObject right2;
    [SerializeField] private GameObject top1;
    [SerializeField] private GameObject top2;
    [SerializeField] private GameObject left1;
    [SerializeField] private GameObject left2;
    [SerializeField] private GameObject down1;
    [SerializeField] private GameObject down2;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color solvedColor;
    [SerializeField] private Color incorrectColor;

    private int number;
    private Dictionary<int, Dictionary<int, GameObject>> edge;
    private Dictionary<int, int> edgeCount;
    private Dictionary<int, Cell> connectedCell;

    private const int RIGHT = 0;
    private const int TOP = 1;
    private const int LEFT = 2;
    private const int DOWN = 3;

    public void Init(int row, int col, int num)
    {
        Number = num;
        Row = row;
        Column = col;

        edgeCount = new Dictionary<int, int>();
        edgeCount[RIGHT] = 0;
        edgeCount[LEFT] = 0;
        edgeCount[TOP] = 0;
        edgeCount[DOWN] = 0;

        connectedCell = new Dictionary<int, Cell>();
        connectedCell[RIGHT] = null;
        connectedCell[LEFT] = null;
        connectedCell[TOP] = null;
        connectedCell[DOWN] = null;

        edge = new Dictionary<int, Dictionary<int, GameObject>>();
        edge[RIGHT] = new Dictionary<int, GameObject>();
        edge[RIGHT][1] = right1;
        edge[RIGHT][2] = right2;

        edge[TOP] = new Dictionary<int, GameObject>();
        edge[TOP][1] = top1;
        edge[TOP][2] = top2;

        edge[LEFT] = new Dictionary<int, GameObject>();
        edge[LEFT][1] = left1;
        edge[LEFT][2] = left2;

        edge[DOWN] = new Dictionary<int, GameObject>();
        edge[DOWN][1] = down1;
        edge[DOWN][2] = down2;


    }

    public void Init()
    {
        for(int i = 0; i < 4; i++)
        {
            if(SceneManager.GetActiveScene().buildIndex==1)
            {
                connectedCell[i] = GameManagerLevel1.instance.GetAdjacentCell(Row, Column, i);
            }
            else
            {

            }
            if (connectedCell[i]==null) continue;

            var singleEdge = edge[i][1].GetComponentInChildren<SpriteRenderer>();
            var doubleEdge = edge[i][2].GetComponentsInChildren<SpriteRenderer>();

            Vector2Int edgeOffset = new Vector2Int(
                connectedCell[i].Row - Row, connectedCell[i].Column - Column
                );

            float edgeSize = Mathf.Abs(edgeOffset.x) > Mathf.Abs(edgeOffset.y) ? Mathf.Abs(edgeOffset.x) : Mathf.Abs(edgeOffset.y);
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                edgeSize *= GameManagerLevel1.instance.EdgeSize;
            }
            else
            {
                edgeSize *= GameManager.instance.EdgeSize;
            }
               
            ChangeSpriteSize(singleEdge, edgeSize);
            foreach(var item in doubleEdge)
            {
                ChangeSpriteSize(item, edgeSize);
            }

        }

        right1.SetActive(false);
        right2.SetActive(false);
        down1.SetActive(false);
        down2.SetActive(false);
        left1.SetActive(false); 
        left2.SetActive(false);
        top1.SetActive(false);
        top2.SetActive(false);
    }

    public void AddEdge(int direction)
    {
        if (connectedCell[direction] == null) return;

        if (edgeCount[direction] == 2)
        {
            RemoveEdge(direction);
            RemoveEdge(direction);
            return;
        }

        edgeCount[direction]++;
        Number--;
        edge[direction][1].SetActive(false);
        edge[direction][2].SetActive(false);
        edge[direction][edgeCount[direction]].SetActive(true);
    }

    public void RemoveEdge(int direction)
    {
        if (connectedCell[direction] == null || edgeCount[direction] == 0) return;
        edgeCount[direction]--;
        Number++;
        edge[direction][1].SetActive(false);
        edge[direction][2].SetActive(false);
        if (edgeCount[direction] != 0)
        {
            edge[direction][edgeCount[direction]].SetActive(true);
        }
        
    }

    public void RemoveAllEdge()
    {
        for (int i=0; i < 4; i++)
        {
            RemoveEdge(i);
            RemoveEdge(i);
        }
    }

    private void ChangeSpriteSize(SpriteRenderer sprite, float size)
    {
        sprite.size = new Vector2(sprite.size.x, size);
    }

    public bool isValidCell(Cell cell, int direction)
    {
        return connectedCell[direction] == cell;
    }
}
