using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PartButton : MonoBehaviour
{
    public string partName;          // 部件名称
    public bool isBroken = false;    // 是否损坏
    public GameObject hintPrefab;    // “运转异常”提示UI（Prefab）

    private GameObject hintInstance;

    public void ShowHint()
    {
        if (hintInstance == null && hintPrefab != null)
        {
            hintInstance = Instantiate(hintPrefab, transform.parent);
            hintInstance.GetComponentInChildren<TextMeshProUGUI>().text = $"{partName}：运转异常";
            hintInstance.transform.position = transform.position;
        }
    }

    public void OnClick()
    {
        if (isBroken)
        {
            ShowHint();
            // 延迟跳转管道修理场景
            Invoke(nameof(GoToRepairScene), 1.2f);
        }
    }

    private void GoToRepairScene()
    {
        SceneManager.LoadScene("PuzzleScene");  // 这里改成你的管道修理场景名
    }
}
