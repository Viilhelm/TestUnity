using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    public static PipeManager Instance;

    [SerializeField] private NewScriptableObjectScript _level;
    [SerializeField] private GameObject _cellPrefab;
    public float cellSize = 1f;   // 棋盘格子大小，默认 1



    private bool hasGameFinished;
    private Pipe[,] pipes;
    private List<Pipe> startPipes;

    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        SpawnLevel();
    }

    //加一个二维布尔数组 hasPipe 来记录哪个格子里有管子
    private bool[,] hasPipe;

    private void SpawnLevel()
    {
        pipes = new Pipe[_level.Row, _level.Column];
        startPipes = new List<Pipe>();
        hasPipe = new bool[_level.Row, _level.Column];

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                //Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                Vector2 spawnPos = new Vector2(j * cellSize + cellSize / 2f,
                               i * cellSize + cellSize / 2f);

                //Pipe tempPipe = Instantiate(_cellPrefab);
                GameObject cellGO = Instantiate(_cellPrefab, spawnPos, Quaternion.identity);
                //Pipe tempPipe = cellGO.GetComponentInChildren<Pipe>();

                //tempPipe.transform.position = spawnPos;
                //tempPipe.transform.localScale = Vector3.one * cellSize;
                //tempPipe.Init(_level.Data[i * _level.Column + j]);
                cellGO.transform.localScale = Vector3.one * cellSize;

                Pipe tempPipe = cellGO.GetComponentInChildren<Pipe>();
                tempPipe.Init(_level.Data[i * _level.Column + j]);

                pipes[i, j] = tempPipe;
                hasPipe[i, j] = true;  // 生成时占满

                // 只有第一行第四列 (row=0, col=3) 可以拖拽
                if (i == _level.Row - 1 && j == 3)
                    tempPipe.IsDraggable = true;
                else
                    tempPipe.IsDraggable = false;

                if (tempPipe.PipeType == 1)
                {
                    startPipes.Add(tempPipe);
                }
            }
        }


        SpawnExternalPipe();

        //Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Column) + 2f;
        Vector3 cameraPos = Camera.main.transform.position;
        //cameraPos.x = _level.Column * 0.5f;
        //cameraPos.y = _level.Row * 0.5f;
        cameraPos.x = _level.Column * cellSize * 0.5f;
        cameraPos.y = _level.Row * cellSize * 0.5f;
        Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Column) * cellSize * 0.5f + 3f * cellSize;

        Camera.main.transform.position = cameraPos;


        StartCoroutine(ShowHint());

        // 强制刷新一次所有管子的接口状态
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                if (tempPipe != null) tempPipe.RefreshInput();
            }
        }

        // 然后立刻检查一次填充
        CheckFill();
        CheckWin();

    }


    private void Update()
    {
        if (hasGameFinished) return;

        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //int row = Mathf.FloorToInt(mousePos.y);
        //int col = Mathf.FloorToInt(mousePos.x);
        //if (row < 0 || col < 0) return;
        //if (row >= _level.Row) return;
        //if (col >= _level.Column) return;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    pipes[row, col].UpdateInput();
        //    StartCoroutine(ShowHint());
        //}
    }

    public IEnumerator ShowHintWrapper()
    {
        yield return StartCoroutine(ShowHint());
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
                if (tempPipe == null) continue;
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
            if (pipe == null || finished.Contains(pipe)) continue;
            finished.Add(pipe);

            List<Pipe> connected = pipe.ConnectedPipes();
            foreach (var connectedPipe in connected)
            {
                if (connectedPipe != null && !finished.Contains(connectedPipe))
                    check.Enqueue(connectedPipe);
            }
        }

        foreach (var filled in finished)
        {
            if (filled != null) filled.IsFilled = true;

        }

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                if (tempPipe != null) tempPipe.UpdateFilled();
            }
        }

    }

    private void CheckWin()
    {
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                if (tempPipe == null || !tempPipe.IsFilled)
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

    public Pipe externalPipePrefab;  // 外部候选管子预制体
    public int externalFixedType = 4;
    private Pipe externalPipe;

    public void SpawnExternalPipe()
    {
        // 计算生成位置：棋盘右边居中
        //float spawnX = _level.Column + 2f;      // 棋盘宽度右边再偏移 2 个单位
        //float spawnY = _level.Row * 0.5f;       // 棋盘高度的一半（居中）
        float spawnX = _level.Column * cellSize + 2f * cellSize;
        float spawnY = _level.Row * cellSize * 0.5f;


        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        // 实例化候选管子
        externalPipe = Instantiate(externalPipePrefab, spawnPos, Quaternion.identity);

        int pipeType = Mathf.Clamp(externalFixedType, 0, 6); // 固定样式
        externalPipe.Init(pipeType);
        externalPipe.transform.localScale = Vector3.one * cellSize;

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
        if (!IsInsideBoard(row, col))
            return false;
        return !hasPipe[row, col];
    }

    // 把管子放进某个格子
    //public void PlacePipeAt(Pipe dragged, int row, int col)
    //{
    //    // 记录被拖动物件的管型与朝向
    //    int type = dragged.PipeType;
    //    int rot = dragged.GetRotationIndex();

    //    // 在目标格生成一个“正式 cell”
    //    Pipe newCell = Instantiate(_cellPrefab);
    //    //newCell.transform.position = new Vector3(col + 0.5f, row + 0.5f, 0);
    //    newCell.transform.position = GetCellCenter(row, col);

    //    newCell.Init(type);          // 按管型初始化
    //    newCell.SetRotationIndex(rot); // 还原旋转
    //    newCell.IsDraggable = false;   // 放回棋盘后不再可拖

    //    // 写回棋盘数据
    //    pipes[row, col] = newCell;
    //    hasPipe[row, col] = true;

    //    // 销毁“只有管子”的外部件
    //    Destroy(dragged.gameObject);

    //    if (PipeManager.Instance != null && PipeManager.Instance.isActiveAndEnabled)
    //        _ = StartCoroutine(ShowHint());
    //}
    public void PlacePipeAt(Pipe dragged, int row, int col)
    {
        // 安全检查
        if (!IsInsideBoard(row, col))
            return;

        // 将拖动的 pipe 移动到棋盘格中心
        dragged.transform.position = GetCellCenter(row, col);

        // 保持缩放一致
        dragged.transform.localScale = Vector3.one * cellSize;

        // 保持可拖拽
        dragged.IsDraggable = true;

        // 更新棋盘数据
        pipes[row, col] = dragged;
        hasPipe[row, col] = true;

        // 重新检测填充状态
        if (isActiveAndEnabled)
            _ = StartCoroutine(ShowHintWrapper());
    }


    public void ClearPipeAt(int row, int col)
    {
        pipes[row, col] = null;
        hasPipe[row, col] = false;
    }

    public void WorldToCell(Vector3 worldPos, out int row, out int col)
    {
        //row = Mathf.RoundToInt(worldPos.y - 0.5f);
        //col = Mathf.RoundToInt(worldPos.x - 0.5f);
        row = Mathf.RoundToInt((worldPos.y - cellSize / 2f) / cellSize);
        col = Mathf.RoundToInt((worldPos.x - cellSize / 2f) / cellSize);

    }

    public Vector2 GetCellCenter(int row, int col)
    {
        //return new Vector2(col + 0.5f, row + 0.5f);
        return new Vector2(col * cellSize + cellSize / 2f,
                   row * cellSize + cellSize / 2f);

    }

    public Vector2 externalDropPos = new Vector2(12, -2);

    public Pipe GetPipe(int row, int col)
    {
        if (IsInsideBoard(row, col))
            return pipes[row, col];
        return null;
    }

    public NewScriptableObjectScript Level => _level;



}
