using UnityEngine;

public class PipeDragHandler : MonoBehaviour
{
    private Pipe pipe;

    private bool isDragging = false;
    private float pressTime = 0f;
    private Vector3 offset;
    private float dragThreshold = 0.3f; // ������ֵ��0.3��

    private void Awake()
    {
        pipe = GetComponent<Pipe>();
    }

    private void OnMouseDown()
    {
        pressTime = 0f;
        isDragging = false;
    }

    private void OnMouseDrag()
    {
        // ÿ֡�ۻ�����ʱ��
        pressTime += Time.deltaTime;

        // ������ pipe ����ק �� ����������ֵ��������קģʽ
        if (pipe.IsDraggable && pressTime > dragThreshold && !isDragging)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
            isDragging = true;
        }

        // ��ק�� �� �������
        if (isDragging)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z) + offset;
        }
    }

    private void OnMouseUp()
    {
        if (!isDragging)
        {
            // ���û�н�����קģʽ �� �̵����ִ����ת
            pipe.UpdateInput();
            PipeManager.Instance.StartCoroutine(PipeManager.Instance.ShowHintWrapper());
        }

        // �ɿ���꣬������ק
        isDragging = false;
        pressTime = 0f;
    }
}
