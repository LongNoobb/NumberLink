using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector] public int Number { get; set; }
    
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
    [SerializeField] private GameObject bottom1;
    [SerializeField] private GameObject bottom2;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color solvedColor;
    [SerializeField] private Color incorrectColor;

    private int number;
    private Dictionary<int, Dictionary<int , GameObject>> edge;
    private Dictionary<int, int> edgeCount;
    private Dictionary<int, Cell> connectedCell;

    private const int RIGHT = 0;
    private const int TOP = 1;
    private const int LEFT = 2;
    private const int DOWN = 3;



}
