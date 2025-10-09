using UnityEngine;
using TMPro;
using System.Collections;


public class ScanButton : MonoBehaviour
{
    public GameObject[] parts;               // 所有可扫描的部件
    public GameObject hintPrefab;            // 提示框 Prefab
    public int brokenPartIndex = 3;          // 哪个是损坏的（0=Part_1，1=Part_2，2=Part_3...）

    private GameObject hintInstance;

    public void OnScanClick()
    {
        // 清除旧的提示
        if (hintInstance != null)
            Destroy(hintInstance);

        for (int i = 0; i < parts.Length; i++)
        {
            if (i == brokenPartIndex)
            {
                // 生成提示
                hintInstance = Instantiate(hintPrefab, parts[i].transform.parent);
                hintInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Error";
                hintInstance.transform.position = parts[i].transform.position;

                // 可选：让部件闪烁高亮
                StartCoroutine(FlashEffect(parts[i]));
            }
        }
    }

    private IEnumerator FlashEffect(GameObject target)
    {
        if (target == null) yield break; // 防止对象被销毁

        var img = target.GetComponent<UnityEngine.UI.Image>();
        if (img == null) yield break;

        Color baseColor = img.color;
        Color flashColor = Color.red;

        for (int i = 0; i < 6; i++)
        {
            if (img == null) yield break; // 循环中也检查
            img.color = (i % 2 == 0) ? flashColor : baseColor;
            yield return new WaitForSeconds(0.2f);
        }

        if (img != null)
            img.color = baseColor;

        AddOutline(target, Color.red);
    }

    private void AddOutline(GameObject target, Color color)
    {
        var outline = target.GetComponent<UnityEngine.UI.Outline>();
        if (outline == null)
            outline = target.AddComponent<UnityEngine.UI.Outline>();

        outline.effectColor = color;
        outline.effectDistance = new Vector2(5, 5);  // 边缘宽度可调
    }


}
