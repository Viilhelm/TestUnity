using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log("=== 点击检测 ===");
            if (results.Count == 0)
            {
                Debug.Log("没有命中任何UI对象！");
            }
            else
            {
                foreach (var result in results)
                {
                    Debug.Log($"命中：{result.gameObject.name}");
                }
            }
        }
    }
}
