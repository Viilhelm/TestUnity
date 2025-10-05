using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    public static PipeManager Instance;

    [SerializeField] private NewScriptableObjectScript _level;
    [SerializeField] private Pipe _cellPrefab;


    private bool hasGameFinished;
    private Pipe[,] pipes;
    private List<Pipe> startPipes;

    public Vector2 externalDropPos = new Vector2(12, -2); // 棋盘外固定位置


    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        pipes = new Pipe[_level.Row, _level.Column];
        startPipes = new List<Pipe>();

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                Pipe tempPipe = Instantiate(_cellPrefab);
                tempPipe.transform.position = spawnPos;
                tempPipe.Init(_level.Data[i * _level.Column + j]);
                pipes[i, j] = tempPipe;
                if (tempPipe.PipeType == 1)
                {
                    startPipes.Add(tempPipe);
                }
            }
        }

        Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Column) + 2f;
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _level.Column * 0.5f;
        cameraPos.y = _level.Row * 0.5f;
        Camera.main.transform.position = cameraPos;

        StartCoroutine(ShowHint());
    }

    private void Update()
    {
        if (hasGameFinished) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);
        if (row < 0 || col < 0) return;
        if (row >= _level.Row) return;
        if (col >= _level.Column) return;

        if (Input.GetMouseButtonDown(0))
        {
            pipes[row, col].UpdateInput();
            StartCoroutine(ShowHint());
        }
    }

    private IEnumerator ShowHint()
    {
        yield return new WaitForSeconds(0.1f);
        CheckFill();
        CheckWin();
    }

    private void CheckFill()
    {
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                if (tempPipe.PipeType != 0)
                {
                    tempPipe.IsFilled = false;
                }
            }
        }

        Queue<Pipe> check = new Queue<Pipe>();
        HashSet<Pipe> finished = new HashSet<Pipe>();
        foreach (var pipe in startPipes)
        {
            check.Enqueue(pipe);
        }

        while (check.Count > 0)
        {
            Pipe pipe = check.Dequeue();
            finished.Add(pipe);
            List<Pipe> connected = pipe.ConnectedPipes();
            foreach (var connectedPipe in connected)
            {
                if (!finished.Contains(connectedPipe))
                {
                    check.Enqueue(connectedPipe);
                }
            }
        }

        foreach (var filled in finished)
        {
            filled.IsFilled = true;
        }

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }

    }

    private void CheckWin()
    {
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                if (!pipes[i, j].IsFilled)
                    return;
            }
        }

        hasGameFinished = true;
        StartCoroutine(GameFinished());
    }

    private IEnumerator GameFinished()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.PuzzleCompleted = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

<<<<<<< Updated upstream
    
=======
    public Pipe externalPipePrefab;  // 外部候选管子预制体
    public int externalFixedType = 3;
    private Pipe externalPipe;

    public void SpawnExternalPipe()
    {
        // 计算生成位置：棋盘右边居中
        float spawnX = _level.Column + 2f;      // 棋盘宽度右边再偏移 2 个单位
        float spawnY = _level.Row * 0.5f;       // 棋盘高度的一半（居中）

        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        // 实例化候选管子
        externalPipe = Instantiate(externalPipePrefab, spawnPos, Quaternion.identity);

        int pipeType = Mathf.Clamp(externalFixedType, 0, 6); // 固定样式
        externalPipe.Init(pipeType);
        externalPipe.IsDraggable = true;
    }

    // 判断格子是否在棋盘内
    public bool IsInsideBoard(int row, int col)
    {
        return row >= 0 && row < _level.Row && col >= 0 && col < _level.Column;
    }

    // 判断格子是否空
    public bool IsEmptyAt(int row, int col)
    {
        return !hasPipe[row, col];
    }

    // 把管子放进某个格子
    public void PlacePipeAt(Pipe dragged, int row, int col)
    {
        // 记录被拖动物件的管型与朝向
        int type = dragged.PipeType;
        int rot = dragged.GetRotationIndex();

        // 在目标格生成一个“正式 cell”
        Pipe newCell = Instantiate(_cellPrefab);
        newCell.transform.position = new Vector3(col + 0.5f, row + 0.5f, 0);
        newCell.Init(type);          // 按管型初始化
        newCell.SetRotationIndex(rot); // 还原旋转
        newCell.IsDraggable = false;   // 放回棋盘后不再可拖

        // 写回棋盘数据
        pipes[row, col] = newCell;
        hasPipe[row, col] = true;

        // 销毁“只有管子”的外部件
        Destroy(dragged.gameObject);
    }


    public void ClearPipeAt(int row, int col)
    {
        pipes[row, col] = null;
        hasPipe[row, col] = false;
    }

    public void WorldToCell(Vector3 worldPos, out int row, out int col)
    {
        row = Mathf.RoundToInt(worldPos.y - 0.5f);
        col = Mathf.RoundToInt(worldPos.x - 0.5f);
    }

    public Vector2 GetCellCenter(int row, int col)
    {
        return new Vector2(col + 0.5f, row + 0.5f);
    }

>>>>>>> Stashed changes

}
