using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScannerTool : MonoBehaviour
{
    public static ScannerTool Instance;

    [Header("扫描相关")]
    public RectTransform scannerIcon;   // 扫描仪图标UI
    public GameObject hintPrefab;       // 运转异常提示
    public GameObject[] scanPoints;     // 检测区域（UI Image）
    public int brokenPartIndex = 3;     // 异常区域索引

    [Header("参数")]
    public float detectionRadius = 40f; // 鼠标检测半径
    public KeyCode actionKey = KeyCode.F;   // 按 F 键执行检测

    private bool detected = false;      // 是否已经检测到异常

    private Vector2 originalPos;

    private void Awake()
    {
        Instance = this;
        originalPos = scannerIcon.anchoredPosition;
        if (GameManager.Instance != null &&
    GameManager.Instance.CurrentStage == GameManager.GameStage.Verification)
        {
            SetModeVerification();
        }

    }



    public void TryScanAtCursor()
    {
        if (detected) return;

        for (int i = 0; i < scanPoints.Length; i++)
        {
            RectTransform rt = scanPoints[i].GetComponent<RectTransform>();
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rt.position);
            float dist = Vector2.Distance(screenPos, Input.mousePosition);

            if (dist < detectionRadius)
            {
                if (mode == ScanMode.Diagnosis && i == brokenPartIndex)
                {
                    Debug.Log("检测到异常部位！");
                    detected = true;
                    StartCoroutine(ShowError(scanPoints[i]));
                   
                    break;
                }

                if (mode == ScanMode.Verification)
                {
                    Debug.Log("运转正常，修理成功！");
                    detected = true;
                    StartCoroutine(ShowSuccess(scanPoints[i]));
                    break;
                }
            }
        }
    }

    private IEnumerator ShowError(GameObject point)
    {
        GameObject hint = Instantiate(hintPrefab, point.transform.parent);
        hint.GetComponentInChildren<TextMeshProUGUI>().text = "Error";
        hint.transform.position = point.transform.position;

        Image img = point.GetComponent<Image>();
        if (img == null) yield break;

        Color baseColor = img.color;
        Color flashColor = Color.red;

        for (int i = 0; i < 6; i++)
        {
            img.color = (i % 2 == 0) ? flashColor : baseColor;
            yield return new WaitForSeconds(0.2f);
        }
        img.color = baseColor;
    }

    private IEnumerator ShowSuccess(GameObject point)
    {
        GameObject hint = Instantiate(hintPrefab, point.transform.parent);
        hint.GetComponentInChildren<TextMeshProUGUI>().text = "运转正常";
        hint.transform.position = point.transform.position;

        Image img = point.GetComponent<Image>();
        if (img == null) yield break;

        Color baseColor = img.color;
        Color flashColor = Color.green;

        for (int i = 0; i < 6; i++)
        {
            img.color = (i % 2 == 0) ? flashColor : baseColor;
            yield return new WaitForSeconds(0.2f);
        }
        img.color = baseColor;
    }

    public enum ScanMode { Diagnosis, Verification }
    public ScanMode mode = ScanMode.Diagnosis;

    public void SetModeVerification()
    {
        mode = ScanMode.Verification;
        Debug.Log("扫描仪切换到验证模式");
    }

    public void SetModeDiagnosis()
    {
        mode = ScanMode.Diagnosis;
        Debug.Log("扫描仪切换到诊断模式");
    }
    public void ResetScanner()
    {
        detected = false;
        scannerIcon.anchoredPosition = originalPos;
    }



}
