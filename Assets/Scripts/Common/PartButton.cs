using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PartButton : MonoBehaviour
{
    public string partName;          // ��������
    public bool isBroken = false;    // �Ƿ���
    public GameObject hintPrefab;    // ����ת�쳣����ʾUI��Prefab��

    private GameObject hintInstance;

    public void ShowHint()
    {
        if (hintInstance == null && hintPrefab != null)
        {
            hintInstance = Instantiate(hintPrefab, transform.parent);
            hintInstance.GetComponentInChildren<TextMeshProUGUI>().text = $"{partName}����ת�쳣";
            hintInstance.transform.position = transform.position;
        }
    }

    public void OnClick()
    {
        if (isBroken)
        {
            ShowHint();
            // �ӳ���ת�ܵ�������
            Invoke(nameof(GoToRepairScene), 1.2f);
        }
    }

    private void GoToRepairScene()
    {
        SceneManager.LoadScene("PuzzleScene");  // ����ĳ���Ĺܵ���������
    }
}
