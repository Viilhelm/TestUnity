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

        // �����һ�� pipe ����ק
        int randRow = Random.Range(0, _level.Row);
        int randCol = Random.Range(0, _level.Column);
        pipes[randRow, randCol].IsDraggable = true;

        SpawnExternalPipe();

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

    public Pipe externalPipePrefab;  // �ⲿ��ѡ����Ԥ����
    private Pipe externalPipe;

    public void SpawnExternalPipe()
    {
        // ��������λ�ã������ұ߾���
        float spawnX = _level.Column + 2f;      // ���̿���ұ���ƫ�� 2 ����λ
        float spawnY = _level.Row * 0.5f;       // ���̸߶ȵ�һ�루���У�

        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        // ʵ������ѡ����
        externalPipe = Instantiate(externalPipePrefab, spawnPos, Quaternion.identity);
        externalPipe.IsDraggable = true;  // �������϶�
    }


}
