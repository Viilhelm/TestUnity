using System.Collections.Generic;
using UnityEngine;
using Common;

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



}
