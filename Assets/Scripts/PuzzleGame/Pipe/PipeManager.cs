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
    public float cellSize = 1f;   // ���̸��Ӵ�С��Ĭ�� 1



    private bool hasGameFinished;
    private Pipe[,] pipes;
    private List<Pipe> startPipes;

    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        SpawnLevel();
    }

    //��һ����ά�������� hasPipe ����¼�ĸ��������й���
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
                hasPipe[i, j] = true;  // ����ʱռ��

                // ֻ�е�һ�е����� (row=0, col=3) ������ק
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

        // ǿ��ˢ��һ�����й��ӵĽӿ�״̬
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                if (tempPipe != null) tempPipe.RefreshInput();
            }
        }

        // Ȼ�����̼��һ�����
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

    public Pipe externalPipePrefab;  // �ⲿ��ѡ����Ԥ����
    public int externalFixedType = 4;
    private Pipe externalPipe;

    public void SpawnExternalPipe()
    {
        // ��������λ�ã������ұ߾���
        //float spawnX = _level.Column + 2f;      // ���̿���ұ���ƫ�� 2 ����λ
        //float spawnY = _level.Row * 0.5f;       // ���̸߶ȵ�һ�루���У�
        float spawnX = _level.Column * cellSize + 2f * cellSize;
        float spawnY = _level.Row * cellSize * 0.5f;


        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        // ʵ������ѡ����
        externalPipe = Instantiate(externalPipePrefab, spawnPos, Quaternion.identity);

        int pipeType = Mathf.Clamp(externalFixedType, 0, 6); // �̶���ʽ
        externalPipe.Init(pipeType);
        externalPipe.transform.localScale = Vector3.one * cellSize;

        externalPipe.IsDraggable = true;
    }

    // �жϸ����Ƿ���������
    public bool IsInsideBoard(int row, int col)
    {
        return row >= 0 && row < _level.Row && col >= 0 && col < _level.Column;
    }

    // �жϸ����Ƿ��
    public bool IsEmptyAt(int row, int col)
    {
        if (!IsInsideBoard(row, col))
            return false;
        return !hasPipe[row, col];
    }

    // �ѹ��ӷŽ�ĳ������
    //public void PlacePipeAt(Pipe dragged, int row, int col)
    //{
    //    // ��¼���϶�����Ĺ����볯��
    //    int type = dragged.PipeType;
    //    int rot = dragged.GetRotationIndex();

    //    // ��Ŀ�������һ������ʽ cell��
    //    Pipe newCell = Instantiate(_cellPrefab);
    //    //newCell.transform.position = new Vector3(col + 0.5f, row + 0.5f, 0);
    //    newCell.transform.position = GetCellCenter(row, col);

    //    newCell.Init(type);          // �����ͳ�ʼ��
    //    newCell.SetRotationIndex(rot); // ��ԭ��ת
    //    newCell.IsDraggable = false;   // �Ż����̺��ٿ���

    //    // д����������
    //    pipes[row, col] = newCell;
    //    hasPipe[row, col] = true;

    //    // ���١�ֻ�й��ӡ����ⲿ��
    //    Destroy(dragged.gameObject);

    //    if (PipeManager.Instance != null && PipeManager.Instance.isActiveAndEnabled)
    //        _ = StartCoroutine(ShowHint());
    //}
    public void PlacePipeAt(Pipe dragged, int row, int col)
    {
        // ��ȫ���
        if (!IsInsideBoard(row, col))
            return;

        // ���϶��� pipe �ƶ������̸�����
        dragged.transform.position = GetCellCenter(row, col);

        // ��������һ��
        dragged.transform.localScale = Vector3.one * cellSize;

        // ���ֿ���ק
        dragged.IsDraggable = true;

        // ������������
        pipes[row, col] = dragged;
        hasPipe[row, col] = true;

        // ���¼�����״̬
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
