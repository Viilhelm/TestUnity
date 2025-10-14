using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Init();
            DontDestroyOnLoad(gameObject);
            //return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void Init()
    //{
    //    //CurrentStage = 1;
    //    CurrentLevel = 1;

    //    Levels = new Dictionary<string, LevelData>();

    //    foreach (var item in _allLevels.Levels)
    //    {
    //        Levels[item.LevelName] = item;
    //    }
    //}

    //[HideInInspector]
    //public int CurrentStage;

    [HideInInspector]
    public int CurrentLevel = 1;

    //[HideInInspector]
    //public string StageName;

    [HideInInspector]
    public string SelectedPuzzle;

    //public bool IsLevelUnlocked(int level)
    //{
    //    //string levelName = "Level" + CurrentStage.ToString() + level.ToString();
    //    string levelName = "Level" + level.ToString();

    //    if (level == 1)
    //    {
    //        PlayerPrefs.SetInt(levelName, 1);
    //        return true;
    //    }

    //    if (PlayerPrefs.HasKey(levelName))
    //    {
    //        return PlayerPrefs.GetInt(levelName) == 1;
    //    }

    //    PlayerPrefs.SetInt(levelName, 0);
    //    return false;
    //}

    public void UnlockLevel()
    {
        CurrentLevel++;

        //if (CurrentLevel == 51)
        //{
        //    CurrentLevel = 1;
        //    CurrentStage++;

        //    if (CurrentStage == 8)
        //    {
        //        CurrentStage = 1;
        //        GoToMainScene();
        //    }
        //}

        //string levelName = "Level" + CurrentStage.ToString() + CurrentLevel.ToString();
        string levelName = "Level" + CurrentLevel.ToString();
        PlayerPrefs.SetInt(levelName, 1);
    }

    //[SerializeField]
    //private LevelData DefaultLevel;

    //[SerializeField]
    //private LevelList _allLevels;

    //private Dictionary<string, LevelData> Levels;

    //public LevelData GetLevel()
    //{
    //    //string levelName = "Level" + CurrentStage.ToString() + CurrentLevel.ToString();
    //    string levelName = "Level" + CurrentLevel.ToString();

    //    if (Levels.ContainsKey(levelName))
    //    {
    //        return Levels[levelName];
    //    }

    //    return DefaultLevel;
    //}

    private const string MainSceme = "MainScene";
    private const string PuzzleScene = "PuzzleScene";

    public void GoToPuzzleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(PuzzleScene);
    }

    public void GoToMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainSceme);
    }

    public bool PuzzleCompleted = false;
    public bool HasPlayedPuzzle = false;

    public int totalScrews = 4;
    private int removedCount = 0;

    public void OnScrewRemoved()
    {
        removedCount++;
        Debug.Log($"已拆下 {removedCount}/{totalScrews}");

        if (removedCount >= totalScrews)
        {
            Debug.Log("所有螺丝已拆下，进入内部结构...");
            SelectedPuzzle = "Pipe"; 
            Invoke(nameof(GoToPuzzleScene), 1f);

        }


    }

    public enum GameStage
    {
        Diagnosis,   // 扫描诊断阶段
        Repair,      // 拆螺丝、修理阶段
        Verification // 测试（扫描确认）阶段
    }

    public GameStage CurrentStage = GameStage.Diagnosis;

    // 由拼图场景回vectroyPanel后调用
    public void EnterVerification()
    {
        CurrentStage = GameStage.Verification;
        ScannerTool.Instance.SetModeVerification();        // 扫描仪切到验证模式
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PuzzleCompleted && scene.name == "MainScene")
        {
            Debug.Log("拼图修复完成，进入测试阶段");
            
            CurrentStage = GameStage.Verification;

           
            // 启用扫描仪验证模式
            //if (ScannerTool.Instance != null)
                //ScannerTool.Instance.SetModeVerification();
        }
    }

}
