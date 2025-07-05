using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameMode { Level, Custom }
    public GameMode currentMode;

    public TMP_Dropdown modeDropdown;
    public GameObject startCustomButton;

    [SerializeField] private Vector3 _startPos;
    [SerializeField] private float _offsetX, _offsetY;
    [SerializeField] private SubGrid _subGridPrefab;
    [SerializeField] private TMP_Text _levelText;

    private bool hasGameFinished;
    private Cell[,] cells;
    private Cell selectedCell;

    private const int GRID_SIZE = 9;
    private const int SUBGRID_SIZE = 3;

    private void Start()
    {
        hasGameFinished = false;

        // Read mode from PlayerPrefs
        currentMode = (GameMode)PlayerPrefs.GetInt("GameMode", 0);

        // Set dropdown and button visibility accordingly
        if (modeDropdown != null)
            modeDropdown.value = (int)currentMode;

        if (startCustomButton != null)
            startCustomButton.SetActive(currentMode == GameMode.Custom);

        // Initialize board
        cells = new Cell[GRID_SIZE, GRID_SIZE];
        selectedCell = null;

        if (currentMode == GameMode.Custom)
        {
            SpawnEmptyBoard();
        }
        else
        {
            SpawnCells();
        }
    }

    public void OnModeChanged()
    {
        int modeValue = modeDropdown.value;
        PlayerPrefs.SetInt("GameMode", modeValue);
        PlayerPrefs.Save();

        // Reload scene once, after saving
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SpawnEmptyBoard()
    {
        _levelText.text = "CUSTOM MODE";

        for (int i = 0; i < GRID_SIZE; i++)
        {
            Vector3 spawnPos = _startPos + i % 3 * _offsetX * Vector3.right + i / 3 * _offsetY * Vector3.up;
            SubGrid subGrid = Instantiate(_subGridPrefab, spawnPos, Quaternion.identity);
            List<Cell> subgridCells = subGrid.cells;

            int startRow = (i / 3) * 3;
            int startCol = (i % 3) * 3;

            for (int j = 0; j < GRID_SIZE; j++)
            {
                int row = startRow + j / 3;
                int col = startCol + j % 3;

                subgridCells[j].Row = row;
                subgridCells[j].Col = col;
                subgridCells[j].Init(0); // Empty
                cells[row, col] = subgridCells[j];
            }
        }
    }

    private void SpawnCells()
    {
        int[,] puzzleGrid = new int[GRID_SIZE, GRID_SIZE];
        int level = PlayerPrefs.GetInt("Level", 0);

        if (level == 0)
        {
            CreateAndStoreLevel(puzzleGrid, 1);
            level = 1;
        }
        else
        {
            GetCurrentLevel(puzzleGrid);
        }

        _levelText.text = "LEVEL " + level;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            Vector3 spawnPos = _startPos + i % 3 * _offsetX * Vector3.right + i / 3 * _offsetY * Vector3.up;
            SubGrid subGrid = Instantiate(_subGridPrefab, spawnPos, Quaternion.identity);
            List<Cell> subgridCells = subGrid.cells;

            int startRow = (i / 3) * 3;
            int startCol = (i % 3) * 3;

            for (int j = 0; j < GRID_SIZE; j++)
            {
                int row = startRow + j / 3;
                int col = startCol + j % 3;

                subgridCells[j].Row = row;
                subgridCells[j].Col = col;

                int cellValue = puzzleGrid[row, col];
                subgridCells[j].Init(cellValue);
                cells[row, col] = subgridCells[j];
            }
        }
    }

    public void StartCustomPuzzle()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                if (cells[i, j].Value != 0)
                {
                    cells[i, j].IsLocked = true;
                }
            }
        }

        CheckWin(); // Optional, or handle other logic
    }

    private void CreateAndStoreLevel(int[,] grid, int level)
    {
        int[,] tempGrid = Generator.GeneratePuzzle((Generator.DifficultyLevel)(level / 100));
        string arrayString = "";

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                arrayString += tempGrid[i, j] + ",";
                grid[i, j] = tempGrid[i, j];
            }
        }

        arrayString = arrayString.TrimEnd(',');
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetString("Grid", arrayString);
    }

    private void GetCurrentLevel(int[,] grid)
    {
        string[] array = PlayerPrefs.GetString("Grid").Split(',');
        int index = 0;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                grid[i, j] = int.Parse(array[index++]);
            }
        }
    }

    private void GoToNextLevel()
    {
        int level = PlayerPrefs.GetInt("Level", 0);
        CreateAndStoreLevel(new int[GRID_SIZE, GRID_SIZE], level + 1);
        RestartGame();
    }

    private void Update()
    {
        if (hasGameFinished || !Input.GetMouseButton(0)) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit && hit.collider.TryGetComponent(out Cell tempCell))
        {
            if (tempCell != selectedCell && !tempCell.IsLocked)
            {
                ResetGrid();
                selectedCell = tempCell;
                HighLight();
            }
        }
    }

    public void UpdateCellValue(int value)
    {
        if (hasGameFinished || selectedCell == null) return;

        selectedCell.UpdateValue(value);
        HighLight();
        CheckWin();
    }

    private void CheckWin()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                if (cells[i, j].IsIncorrect || cells[i, j].Value == 0) return;
            }
        }

        hasGameFinished = true;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                cells[i, j].UpdateWin();
            }
        }

        if (currentMode == GameMode.Level)
            Invoke(nameof(GoToNextLevel), 2f);
    }

    private void HighLight()
    {
        if (selectedCell == null) return;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                cells[i, j].IsIncorrect = !IsValid(cells[i, j]);
            }
        }

        int row = selectedCell.Row;
        int col = selectedCell.Col;
        int subRow = row - row % SUBGRID_SIZE;
        int subCol = col - col % SUBGRID_SIZE;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            cells[i, col].HighLight();
            cells[row, i].HighLight();
            cells[subRow + i % 3, subCol + i / 3].HighLight();
        }

        selectedCell.Select();
    }

    private bool IsValid(Cell cell)
    {
        int row = cell.Row;
        int col = cell.Col;
        int value = cell.Value;

        if (value == 0) return true;

        cell.Value = 0;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            if (cells[row, i].Value == value || cells[i, col].Value == value)
            {
                cell.Value = value;
                return false;
            }
        }

        int subRow = row - row % SUBGRID_SIZE;
        int subCol = col - col % SUBGRID_SIZE;

        for (int r = subRow; r < subRow + SUBGRID_SIZE; r++)
        {
            for (int c = subCol; c < subCol + SUBGRID_SIZE; c++)
            {
                if (cells[r, c].Value == value)
                {
                    cell.Value = value;
                    return false;
                }
            }
        }

        cell.Value = value;
        return true;
    }

    private void ResetGrid()
    {
        for (int i = 0; i < GRID_SIZE; i++)
            for (int j = 0; j < GRID_SIZE; j++)
                cells[i, j].Reset();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
