using UnityEngine;
using UnityEngine.EventSystems;

public class Screw : MonoBehaviour
{
    private bool removed = false;
    private float rotationSpeed = 400f; // 旋转速度
    private float currentAngle = 0f;

    public void StartUnscrew()
    {
        if (removed) return;
        StartCoroutine(RemoveScrew());
    }


    private System.Collections.IEnumerator RemoveScrew()
    {
        removed = true;
        Debug.Log($"{name} 开始拆除");

        // 模拟旋转动画
        float targetAngle = currentAngle + 360f;
        while (currentAngle < targetAngle)
        {
            currentAngle += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, -currentAngle);
            yield return null;
        }

        // 模拟“卸下”效果（螺丝下沉 + 消失）
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, -25, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        gameObject.SetActive(false);

        // 通知游戏管理器
        GameManager.Instance.OnScrewRemoved();
    }
}
