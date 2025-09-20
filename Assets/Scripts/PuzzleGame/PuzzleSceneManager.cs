using UnityEngine;

public class PuzzleSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject FlowPuzzleRoot;
    [SerializeField] private GameObject PipePuzzleRoot;

    void Start()
    {

        FlowPuzzleRoot.SetActive(false);
        PipePuzzleRoot.SetActive(false);

        if (GameManager.Instance.SelectedPuzzle == "Flow")
        {
            FlowPuzzleRoot.SetActive(true);
        }
        else if (GameManager.Instance.SelectedPuzzle == "Pipe")
        {
            PipePuzzleRoot.SetActive(true);
        }
    }
}
