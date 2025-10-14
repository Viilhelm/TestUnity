using UnityEngine;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;
    public bool screwdriverActive = false;
    public Image screwdriverIcon; // 复用已有的 ScrewdriverButton 的 Image

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // 当螺丝刀被选中时，让按钮图片跟随鼠标
        if (screwdriverActive && screwdriverIcon != null)
        {
            screwdriverIcon.raycastTarget = false; // 避免挡住UI射线
            screwdriverIcon.transform.position = Input.mousePosition;
        }
    }

    public void SelectScrewdriver()
    {
        screwdriverActive = true;
        Debug.Log("螺丝刀已选中");
    }

    public void ResetTool()
    {
        screwdriverActive = false;
        if (screwdriverIcon != null)
            screwdriverIcon.raycastTarget = true; // 重新启用按钮交互
    }
}
