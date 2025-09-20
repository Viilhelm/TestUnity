using UnityEngine;

public class UIButtons : MonoBehaviour
{
    //[Header("Chip Panel")]
    //public GameObject chipPanel;
    //public GameObject victoryPanel;
    //public GameObject bg;



    //private void Start()
    //{
    //    if (GameManager.Instance.PuzzleCompleted)
    //    {
            
    //        bg.SetActive(false);
    //        chipPanel.SetActive(false);
    //        victoryPanel.SetActive(true);
    //    }
    //    else
    //    {
            
    //        if (GameManager.Instance.HasPlayedPuzzle)
    //        {
    //            bg.SetActive(false);
    //            chipPanel.SetActive(true);
    //            victoryPanel.SetActive(false);
    //        }
    //        else
    //        {
    //            bg.SetActive(true);
    //            chipPanel.SetActive(false);
    //            victoryPanel.SetActive(false);
    //        }
    //    }

    //    GameManager.Instance.HasPlayedPuzzle = true;
    //}

    public void GoToMain()
    {
        GameManager.Instance.PuzzleCompleted = false;
        GameManager.Instance.HasPlayedPuzzle = false;
        GameManager.Instance.GoToMainScene();
    }

    public void ReturnToPanel()
    {
        GameManager.Instance.GoToMainScene();
    }

    //public void OpenChipPanel()
    //{
    //    if (chipPanel != null)
    //        chipPanel.SetActive(true);
    //        bg.SetActive(false);
    //}

    //public void CloseChipPanel()
    //{
    //    if (chipPanel != null)
    //        chipPanel.SetActive(false);
    //        bg.SetActive(true);
    //}
}
