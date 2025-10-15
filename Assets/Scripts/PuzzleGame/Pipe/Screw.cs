using UnityEngine;
using UnityEngine.EventSystems;

public class Screw : MonoBehaviour
{
    private bool removed = false;
    private float rotationSpeed = 400f; // ��ת�ٶ�
    private float currentAngle = 0f;

    public void StartUnscrew()
    {
        if (removed) return;
        StartCoroutine(RemoveScrew());
    }


    private System.Collections.IEnumerator RemoveScrew()
    {
        removed = true;
        Debug.Log($"{name} ��ʼ���");

        // ģ����ת����
        float targetAngle = currentAngle + 360f;
        while (currentAngle < targetAngle)
        {
            currentAngle += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, -currentAngle);
            yield return null;
        }

        // ģ�⡰ж�¡�Ч������˿�³� + ��ʧ��
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

        // ֪ͨ��Ϸ������
        GameManager.Instance.OnScrewRemoved();
    }
}
