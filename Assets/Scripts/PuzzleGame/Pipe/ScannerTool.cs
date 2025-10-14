using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScannerTool : MonoBehaviour
{
    public static ScannerTool Instance;

    [Header("ɨ�����")]
    public RectTransform scannerIcon;   // ɨ����ͼ��UI
    public Texture2D scannerCursor;     // ��ѡ�������ʽ
    public GameObject hintPrefab;       // ��ת�쳣��ʾ
    public GameObject[] scanPoints;     // �������UI Image��
    public int brokenPartIndex = 3;     // �쳣��������

    [Header("����")]
    public float detectionRadius = 40f; // �����뾶
    public bool isActive = false;

    private bool detected = false;      // �Ƿ��Ѿ���⵽�쳣

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateScanner()
    {
        if (isActive) return;
        isActive = true;
        detected = false;

        Debug.Log("ɨ�����Ѽ���");

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

        // ��ɨ����ͼ��������
        if (scannerIcon != null)
            scannerIcon.position = Input.mousePosition;

        // �������Ƿ񿿽��쳣����                                        
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
                            Debug.Log("��⵽�쳣��λ��");
                            detected = true;
                            StartCoroutine(ShowError(scanPoints[i]));
                            DeactivateScanner();
                            // ������˿��
                            if (ToolManager.Instance != null)
                                ToolManager.Instance.screwdriverActive = true;
                            break;
                        }

                    }
                    if (mode == ScanMode.Verification)
                    {
                        Debug.Log("��ת����������ɹ���");
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
        brokenPartIndex = -1;        // ����쳣��������ֹ����
        detected = false;            // ���ü��״̬
        ActivateScanner();
    }

    private IEnumerator ShowSuccess(GameObject point)
    {
        GameObject hint = Instantiate(hintPrefab, point.transform.parent);
        hint.GetComponentInChildren<TextMeshProUGUI>().text = "��ת���� ";
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
