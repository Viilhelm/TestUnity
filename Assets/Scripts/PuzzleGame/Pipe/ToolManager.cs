using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;

    public RectTransform screwdriverIcon;
    public RectTransform scannerIcon;
    public KeyCode actionKey = KeyCode.F;
    public float pickRadius = 60f; // 鼠标靠近图标多少距离内算“拿起”

    private Vector2 screwOrigin;
    private Vector2 scanOrigin;
    private bool isDragging = false;

    public enum ToolType { None, Screwdriver, Scanner }
    public ToolType currentTool = ToolType.None;

    void Awake()
    {
        Instance = this;
        if (screwdriverIcon != null) screwOrigin = screwdriverIcon.anchoredPosition;
        if (scannerIcon != null) scanOrigin = scannerIcon.anchoredPosition;
    }

    void Update()
    {
        HandleToolSelection();
        HandleToolDrag();
    }

    private void HandleToolSelection()
    {
        // 右键按下时判断是否靠近某个工具图标
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Input.mousePosition;

            float distToScrew = Vector2.Distance(mousePos, screwdriverIcon.position);
            float distToScan = Vector2.Distance(mousePos, scannerIcon.position);

            if (distToScrew < pickRadius)
            {
                currentTool = ToolType.Screwdriver;
                isDragging = true;
                Debug.Log("自动选中螺丝刀");
            }
            else if (distToScan < pickRadius)
            {
                currentTool = ToolType.Scanner;
                isDragging = true;
                Debug.Log("自动选中扫描仪");
            }
        }
    }

    private void HandleToolDrag()
    {
        if (!isDragging) return;

        // 拖拽中
        if (Input.GetMouseButton(1))
        {
            switch (currentTool)
            {
                case ToolType.Screwdriver:
                    screwdriverIcon.position = Input.mousePosition;
                    if (Input.GetKeyDown(actionKey))
                        TryUnscrewAtCursor();
                    break;

                case ToolType.Scanner:
                    scannerIcon.position = Input.mousePosition;
                    if (Input.GetKeyDown(actionKey))
                        ScannerTool.Instance?.TryScanAtCursor();
                    break;
            }
        }
        else
        {
            // 松开右键时归位并清空
            ResetTools();
        }
    }

    private void ResetTools()
    {
        isDragging = false;
        currentTool = ToolType.None;

        screwdriverIcon.anchoredPosition = screwOrigin;
        scannerIcon.anchoredPosition = scanOrigin;
        Debug.Log("工具归位并取消选中");
    }

    private void TryUnscrewAtCursor()
    {
        GraphicRaycaster raycaster = FindFirstObjectByType<GraphicRaycaster>();
        EventSystem eventSystem = EventSystem.current;
        if (raycaster == null || eventSystem == null) return;

        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        foreach (var r in results)
        {
            if (r.gameObject.CompareTag("Screw"))
            {
                r.gameObject.GetComponent<Screw>()?.StartUnscrew();
                Debug.Log("F键成功触发拧螺丝");
                return;
            }
        }
    }
}
