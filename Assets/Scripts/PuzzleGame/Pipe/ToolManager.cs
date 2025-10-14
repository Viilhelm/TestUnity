using UnityEngine;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;
    public bool screwdriverActive = false;
    public Image screwdriverIcon; // �������е� ScrewdriverButton �� Image

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // ����˿����ѡ��ʱ���ð�ťͼƬ�������
        if (screwdriverActive && screwdriverIcon != null)
        {
            screwdriverIcon.raycastTarget = false; // ���⵲סUI����
            screwdriverIcon.transform.position = Input.mousePosition;
        }
    }

    public void SelectScrewdriver()
    {
        screwdriverActive = true;
        Debug.Log("��˿����ѡ��");
    }

    public void ResetTool()
    {
        screwdriverActive = false;
        if (screwdriverIcon != null)
            screwdriverIcon.raycastTarget = true; // �������ð�ť����
    }
}
