using Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [HideInInspector] public bool hasGameFinished;

    //[SerializeField] private TMP_Text _titleText;
   // [SerializeField] private GameObject _winText;
    [SerializeField] private SpriteRenderer _clickHighlight;

    [SerializeField] private LevelList _allLevels;
    [SerializeField] private LevelData _defaultLevel;


    private void Awake()
    {
        Instance = this;

        hasGameFinished = false;
        //_winText.SetActive(false);
        //_titleText.gameObject.SetActive(true);
        //_titleText.text = GameManager.Instance.StageName +
        //" - " + GameManager.Instance.CurrentLevel.ToString();

        //CurrentLevelData = GameManager.Instance.GetLevel();

        //int levelIndex = GameManager.Instance.CurrentLevel - 1;
        //if (levelIndex < 0 || levelIndex >= _allLevels.Levels.Count)
        //{
        //    CurrentLevelData = _allLevels.Levels[0];
        //}
        //else
        //{
        //    CurrentLevelData = _allLevels.Levels[levelIndex];
        //}

        CurrentLevelData = _defaultLevel;

        SpawnBoard();

        SpawnNodes();

    }

    public bool IsLevelUnlocked(int level)
    {
        string levelName = "Level" + level.ToString();

        if (level == 1)
        {
            PlayerPrefs.SetInt(levelName, 1);
            return true;
        }

        if (PlayerPrefs.HasKey(levelName))
            return PlayerPrefs.GetInt(levelName) == 1;

        PlayerPrefs.SetInt(levelName, 0);
        return false;
    }




    [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPrefab;

    private void SpawnBoard()
    {
        //int currentLevelSize = GameManager.Instance.CurrentStage + 4;
        //int currentLevelSize = GameManager.Instance.CurrentLevel + 4;
        int currentLevelSize = CurrentLevelData.Size;

        var board = Instantiate(_boardPrefab,
            new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
            Quaternion.identity);

        board.size = new Vector2(currentLevelSize + 0.08f, currentLevelSize + 0.08f);

        for (int i = 0; i < currentLevelSize; i++)
        {
            for (int j = 0; j < currentLevelSize; j++)
            {
                Instantiate(_bgCellPrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity);
            }
        }

        Camera.main.orthographicSize = currentLevelSize + 2f;
        Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);

        _clickHighlight.size = new Vector2(currentLevelSize / 4f, currentLevelSize / 4f);
        _clickHighlight.transform.position = Vector3.zero;
        _clickHighlight.gameObject.SetActive(false);
    }

    private LevelData CurrentLevelData;
    [SerializeField] private Node _nodePrefab;
    private List<Node> _nodes;

    public Dictionary<Vector2Int, Node> _nodeGrid;

    private void SpawnNodes()
    {
        _nodes = new List<Node>();
        _nodeGrid = new Dictionary<Vector2Int, Node>();

        //int currentLevelSize = GameManager.Instance.CurrentStage + 4;
        //int currentLevelSize = GameManager.Instance.CurrentLevel + 4;
        int currentLevelSize = CurrentLevelData.Size;

        Node spawnedNode;
        Vector3 spawnPos;

        for (int i = 0; i < currentLevelSize; i++)
        {
            for (int j = 0; j < currentLevelSize; j++)
            {
                spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                spawnedNode.Init();

                int colorIdForSpawnedNode = GetColorId(i, j);

                if (colorIdForSpawnedNode != -1)
                {
                    spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                }

                _nodes.Add(spawnedNode);
                _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                spawnedNode.gameObject.name = i.ToString() + j.ToString();
                spawnedNode.Pos2D = new Vector2Int(i, j);

            }
        }

        List<Vector2Int> offsetPos = new List<Vector2Int>()
            {Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right };

        foreach (var item in _nodeGrid)
        {
            foreach (var offset in offsetPos)
            {
                var checkPos = item.Key + offset;
                if (_nodeGrid.ContainsKey(checkPos))
                {
                    item.Value.SetEdge(offset, _nodeGrid[checkPos]);
                }
            }
        }

    }

    public List<Color> NodeColors;

    public int GetColorId(int i, int j)
    {
        List<LevelData.Edge> edges = CurrentLevelData.Edges;
        Vector2Int point = new Vector2Int(i, j);

        for (int colorId = 0; colorId < edges.Count; colorId++)
        {
            if (edges[colorId].StartPoint == point ||
                edges[colorId].EndPoint == point)
            {
                return colorId;
            }
        }

        return -1;
    }

    public Color GetHighLightColor(int colorID)
    {
        Color result = NodeColors[colorID % NodeColors.Count];
        result.a = 0.4f;
        return result;
    }

    private Node startNode;

    private void Update()
    {
        if (hasGameFinished) return;

        if (Input.GetMouseButtonDown(0))
        {
            startNode = null;
            //return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit && hit.collider.gameObject.TryGetComponent(out Node tempNode))
            {
                if (startNode == null)
                {
                    
                    if (tempNode.IsClickable)
                    {
                        startNode = tempNode;
                        _clickHighlight.gameObject.SetActive(true);
                        _clickHighlight.color = GetHighLightColor(tempNode.colorId);
                    }
                }
                else if (startNode != tempNode)
                {
                    if (tempNode.IsEndNode && startNode.colorId != tempNode.colorId)
                    {
                        return;
                    }

                    startNode.UpdateInput(tempNode);
                    startNode = tempNode;   
                }
            }

            _clickHighlight.transform.position = mousePos2D;
        }

        if (Input.GetMouseButtonUp(0))
        {
            CheckWin();
            startNode = null;
            _clickHighlight.gameObject.SetActive(false);
        }

    }


    private void CheckWin()
    {
       
        foreach (var n in _nodes) n.SolveHighlight();

        
        foreach (var n in _nodes)
        {
            if (!n.IsWin) return;
        }

        
        GameManager.Instance.UnlockLevel();
        _clickHighlight.gameObject.SetActive(false);
        hasGameFinished = true;
        
        // _winText.SetActive(true);
    }


    //public void ClickedBack()
    //{
    //    GameManager.Instance.GoToMainScene();
    //}

    //public void ClickedRestart()
    //{
    //    GameManager.Instance.GoToPuzzleScene();
    //}
}
