using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScannerTool : MonoBehaviour
{
    public static ScannerTool Instance;

    [Header("ɨ�����")]
    public RectTransform scannerIcon;   // ɨ����ͼ��UI
    public GameObject hintPrefab;       // ��ת�쳣��ʾ
    public GameObject[] scanPoints;     // �������UI Image��
    public int brokenPartIndex = 3;     // �쳣��������

    [Header("����")]
    public float detectionRadius = 40f; // �����뾶
    public KeyCode actionKey = KeyCode.F;   // �� F ��ִ�м��

    private bool detected = false;      // �Ƿ��Ѿ���⵽�쳣

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
                    Debug.Log("��⵽�쳣��λ��");
                    detected = true;
                    StartCoroutine(ShowError(scanPoints[i]));
                   
                    break;
                }

                if (mode == ScanMode.Verification)
                {
                    Debug.Log("��ת����������ɹ���");
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
        hint.GetComponentInChildren<TextMeshProUGUI>().text = "��ת����";
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
        Debug.Log("ɨ�����л�����֤ģʽ");
    }

    public void SetModeDiagnosis()
    {
        mode = ScanMode.Diagnosis;
        Debug.Log("ɨ�����л������ģʽ");
    }
    public void ResetScanner()
    {
        detected = false;
        scannerIcon.anchoredPosition = originalPos;
    }



}
