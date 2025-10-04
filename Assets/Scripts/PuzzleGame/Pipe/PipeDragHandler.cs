using UnityEngine;

public class PipeDragHandler : MonoBehaviour
{
    private Pipe pipe;

    private bool isDragging = false;
    private float pressTime = 0f;
    private Vector3 offset;
    private float dragThreshold = 0.3f; // 长按阈值，0.3秒

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
        // 每帧累积按下时间
        pressTime += Time.deltaTime;

        // 如果这个 pipe 可拖拽 且 长按超过阈值，进入拖拽模式
        if (pipe.IsDraggable && pressTime > dragThreshold && !isDragging)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
            isDragging = true;
        }

        // 拖拽中 → 跟随鼠标
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
            // 如果没有进入拖拽模式 → 短点击，执行旋转
            pipe.UpdateInput();
            PipeManager.Instance.StartCoroutine(PipeManager.Instance.ShowHintWrapper());
        }

        // 松开鼠标，结束拖拽
        isDragging = false;
        pressTime = 0f;
    }
}
