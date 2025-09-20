using UnityEngine;

public class ChipPart : MonoBehaviour
{
    [Tooltip("Puzzle type, e.g., Flow / Pipe ")]
    public string puzzleType;

    public void OnClickPart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectedPuzzle = puzzleType;
            GameManager.Instance.GoToPuzzleScene();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null! Make sure there is a GameManager in the scene.");
        }
    }
}
