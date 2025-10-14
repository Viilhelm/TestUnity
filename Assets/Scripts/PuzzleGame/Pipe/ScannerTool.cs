using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScannerTool : MonoBehaviour
{
    public static ScannerTool Instance;

    [Header("扫描相关")]
    public RectTransform scannerIcon;   // 扫描仪图标UI
    public Texture2D scannerCursor;     // 可选：鼠标样式
    public GameObject hintPrefab;       // 运转异常提示
    public GameObject[] scanPoints;     // 检测区域（UI Image）
    public int brokenPartIndex = 3;     // 异常区域索引

    [Header("参数")]
    public float detectionRadius = 40f; // 鼠标检测半径
    public bool isActive = false;

    private bool detected = false;      // 是否已经检测到异常

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateScanner()
    {
        if (isActive) return;
        isActive = true;
        detected = false;

        Debug.Log("扫描仪已激活");

        if (scannerIcon != null)
            scannerIcon.gameObject.SetActive(true);

        //if (scannerCursor != null)
           // Cursor.SetCursor(scannerCursor, Vector2.zero, CursorMode.Auto);
    }

    public void DeactivateScanner()
    {
        isActive = false;
        if (scannerIcon != null)
            scannerIcon.gameObject.SetActive(false);
        //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (!isActive) return;

        // 让扫描仪图标跟随鼠标
        if (scannerIcon != null)
            scannerIcon.position = Input.mousePosition;

        // 检测鼠标是否靠近异常区域                                        
        if (!detected)
        {
            for (int i = 0; i < scanPoints.Length; i++)
            {
                RectTransform rt = scanPoints[i].GetComponent<RectTransform>();
                //Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rt.position);
                //float dist = Vector2.Distance(screenPos, Input.mousePosition);
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rt.position);
                float dist = Vector2.Distance(screenPos, Input.mousePosition);

                if (dist < detectionRadius)
                {

                    if (mode == ScanMode.Diagnosis && i == brokenPartIndex)
                    {
                        if (i == brokenPartIndex)
                        {
                            Debug.Log("检测到异常部位！");
                            detected = true;
                            StartCoroutine(ShowError(scanPoints[i]));
                            DeactivateScanner();
                            // 激活螺丝刀
                            if (ToolManager.Instance != null)
                                ToolManager.Instance.screwdriverActive = true;
                            break;
                        }

                    }
                    if (mode == ScanMode.Verification)
                    {
                        Debug.Log("运转正常，修理成功！");
                        detected = true;
                        StartCoroutine(ShowSuccess(scanPoints[i]));
                        DeactivateScanner();
                        break;
                    }
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

    public enum ScanMode { Diagnosis, Verification }

    public ScanMode mode = ScanMode.Diagnosis;


    public void SetModeVerification()
    {
        mode = ScanMode.Verification;
        brokenPartIndex = -1;        // 清空异常索引，防止残留
        detected = false;            // 重置检测状态
        ActivateScanner();
    }

    private IEnumerator ShowSuccess(GameObject point)
    {
        GameObject hint = Instantiate(hintPrefab, point.transform.parent);
        hint.GetComponentInChildren<TextMeshProUGUI>().text = "运转正常 ";
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


}
