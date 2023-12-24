using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Sprite[] digits;

    [SerializeField] private GameObject CompletePop;
    
    private const int Size = 9;
    private float _screenWidth;
    
    private Vector3 _tilePos;
    private Vector3 _buttonPos;
    
    private Board _sudoku;
    private Dictionary<Vector2, TileController> _tiles = new Dictionary<Vector2, TileController>();
    private Dictionary<int, ButtonController> _buttons = new Dictionary<int, ButtonController>();

    private Vector2 _selectedTile;
    
    private void Start()
    {
        // _sudoku = new Board(PlayerPrefs.GetInt("Blanks", 15));
        _sudoku = new Board(2);
        CalculateScreenWidth();
        
        DisplayBoard();
        DisplayButton();
        _selectedTile = Vector2.one * -1;
    }

    private void Update()
    {
        if (Board.IsSolved)
        {
            CompletePop.SetActive(true);
            return;
        }
        
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
                if (hit.collider is not null)
                {
                    var touchedObjectID = hit.collider.gameObject.GetInstanceID();
                    if (hit.collider.CompareTag("Tile")) {
                        foreach (var (key, controller) in _tiles)
                        {
                            if (controller.gameObject.GetInstanceID() == touchedObjectID)
                            {
                                controller.SetHighlight(true);
                                _selectedTile = key;
                            }
                            else
                            {
                                controller.SetHighlight(false);
                            }
                        }
                    }

                    if (hit.collider.CompareTag("Button"))
                    {
                        var value = hit.collider.gameObject.GetComponent<ButtonController>().Value;
                        if (_selectedTile != Vector2.one * - 1)
                        {
                            _sudoku.SetPositionValue((int)_selectedTile.x, (int)_selectedTile.y, value);
                            _tiles[_selectedTile].SetSprite(digits[value]);
                        }
                    }
                }
            }
        }
    }
    
    private void CalculateScreenWidth()
    {
        var topEdge = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        _screenWidth = topEdge.x * 2;

        var leftEdge = Camera.main.ViewportToWorldPoint(new Vector2(0, 0.75f));
        _tilePos = leftEdge;
        _tilePos.x = _tilePos.x + (_screenWidth / 18);
        _tilePos.z = 0;
        
        var bottomEdge = Camera.main.ViewportToWorldPoint(new Vector2(0, 0.15f));
        _buttonPos = bottomEdge;
        _buttonPos.x = _buttonPos.x + (_screenWidth / 18);
        _buttonPos.z = 0;
    }

    private void DisplayBoard()
    {
        for (var i = 0; i < Size; ++i)
        {
            for (var j = 0; j < Size; ++j)
            {
                var currTile = Instantiate(tilePrefab);
                currTile.name = $"Tile {i} {j}";
                currTile.tag = "Tile";
                
                currTile.transform.position = _tilePos;
                
                var tileController = currTile.GetComponent<TileController>();
                var tileValue = _sudoku.GetPositionValue(i, j);

                tileController.SetScale(Vector3.one * _screenWidth / 9);
                tileController.SetSprite(digits[tileValue]);
                tileController.InitColor(((i * Size) + j) % 2 == 0);
                
                if (tileValue == 0) 
                {
                    tileController.SetEmpty();
                    _tiles[new Vector2(i, j)] = tileController;
                }
                
                _tilePos.x = _tilePos.x + (_screenWidth / 9);
            }

            _tilePos.x = _tilePos.x - _screenWidth;
            _tilePos.y = _tilePos.y - (_screenWidth / 9);

        }
    }

    private void DisplayButton()
    {
        for (var i = 0; i < Size; ++i)
        {
            var currButton = Instantiate(buttonPrefab);
            currButton.name = $"Button {i + 1}";
            currButton.tag = "Button";

            currButton.transform.position = _buttonPos;

            var buttonController = currButton.GetComponent<ButtonController>();
            buttonController.Value = i + 1;
            
            buttonController.SetScale(Vector3.one * _screenWidth / 9);
            buttonController.SetSprite(digits[i + 1]);
            buttonController.Value = i + 1;

            _buttons[i + 1] = buttonController;
            
            _buttonPos.x = _buttonPos.x + (_screenWidth / 9);
        }
    }

    private void FixedUpdate()
    {
        if (!Board.IsSolved) _sudoku.UpdateResult();
    }
}

internal class Board
{
    private int[,] _board = new int[9, 9];
    private int[] _values = {1, 2, 3, 4, 5, 6, 7, 8, 9};
    private int _blanks;
    public static bool IsSolved = false;

    private bool[] rowChecks = { true, true, true, true, true, true, true, true, true };
    private bool[] colChecks = { true, true, true, true, true, true, true, true, true };
    private bool[] gridChecks = { true, true, true, true, true, true, true, true, true };
    
    private int[] xGrid = { 0, 0, 0, 3, 3, 3, 6, 6, 6 };
    private int[] yGrid = { 0, 3, 6, 0, 3, 6, 0, 3, 6 };

    public bool isChecked = false;
    
    public Board(int blanks)
    {
        _blanks = blanks;
        GenerateBoard(_board);
        GenerateBlanks();
    }

    public int GetPositionValue(int x, int y)
    {
        return _board[x, y];
    }

    public void SetPositionValue(int x, int y, int value)
    {
        _board[x, y] = value;
        rowChecks[x] = UpdateRowStatus(x);
        colChecks[y] = UpdateColStatus(y);
        
        int startRow = x - x % 3;
        int startCol = y - y % 3;

        for (var i = 0; i < 9; ++i)
        {
            if (xGrid[i] == startRow && yGrid[i] == startCol)
            {
                gridChecks[i] = UpdateGridStatus(startRow, startCol);
                break;
            }
        }

        isChecked = false;
    }

    private bool GenerateBoard(int[,] board, int row = 0, int col = 0)
    {
        if (row == 9) return true;

        var nextRow = (col == 8) ? row + 1 : row;
        var nextCol = (col + 1) % 9;

        ShuffleArray(_values);
        foreach (var num in _values)
        {
            if (IsValidMove(board, row, col, num))
            {
                board[row, col] = num;

                if (GenerateBoard(board, nextRow, nextCol)) return true;

                board[row, col] = 0;
            }
        }

        return false;
    }

    private void GenerateBlanks()
    {
        var blankIndex = new HashSet<int>();

        while (blankIndex.Count < _blanks)
        {
            blankIndex.Add(Random.Range(0, 81));
        }

        foreach (var idx in blankIndex)
        {
            var j = idx % 9;
            var i = (int) idx / 9;
            SetPositionValue(i, j, 0);
        }
    }

    private bool IsValidMove(int[,] board, int row, int col, int num)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == num || board[i, col] == num)
                return false;
        }

        int startRow = row - row % 3;
        int startCol = col - col % 3;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i + startRow, j + startCol] == num)
                    return false;
            }
        }

        return true;
    }
    
    private void ShuffleArray(IList<int> arr)
    {
        for (var i = 0; i < arr.Count; ++i)
        {
            var randIdx = Random.Range(i, arr.Count);
            (arr[i], arr[randIdx]) = (arr[randIdx], arr[i]);
        }
    }

    public void UpdateResult()
    {
        if (isChecked == false) IsSolved = CheckResult();
        isChecked = true;
    }

    private bool CheckResult()
    {
        for (int i = 0; i < 9; ++i)
        {
            if (colChecks[i] == false || rowChecks[i] == false || gridChecks[i] == false)
            {
                return false;
            }
        }

        return true;
    }

    private bool UpdateRowStatus(int row)
    {
        var values = new HashSet<int>();

        for (var i = 0; i < 9; ++i)
        {
            values.Add(_board[row, i]);
        }

        return values.Count == 9 && !values.Contains(0);
    }

    private bool UpdateColStatus(int col)
    {
        var values = new HashSet<int>();

        for (var i = 0; i < 9; ++i)
        {
            values.Add(_board[i, col]);
        }

        return values.Count == 9 && !values.Contains(0);
    }

    private bool UpdateGridStatus(int startX, int startY)
    {
        var values = new HashSet<int>();

        for (var i = startX; i < startX + 3; ++i)
        {
            for (var j = startY; j < startY + 3; ++j)
            {
                values.Add(_board[i, j]);
            }
        }

        return values.Count == 9 && !values.Contains(0);
    }
}