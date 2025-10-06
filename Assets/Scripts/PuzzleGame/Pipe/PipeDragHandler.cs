using UnityEngine;

public class PipeDragHandler : MonoBehaviour
{
    private Pipe pipe;

    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 startPosition;
    private bool startedFromBoard = false;
    private bool clearedFromBoard = false;

    private void Awake()
    {
        pipe = GetComponent<Pipe>();
    }

    private void Update()
    {
        if (!pipe.IsDraggable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    pipe.UpdateInput();
                    if (PipeManager.Instance != null && PipeManager.Instance.isActiveAndEnabled)
                        _ = PipeManager.Instance.ShowHintWrapper();
                }
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                pipe.UpdateInput();
                if (PipeManager.Instance != null && PipeManager.Instance.isActiveAndEnabled)
                    _ = PipeManager.Instance.ShowHintWrapper();
            }
        }

        // ---------------- 右键开始拖拽 ----------------
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mouseWorld.x, mouseWorld.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                startPosition = transform.position;
                isDragging = true;
                offset = transform.position - mouseWorld;

                PipeManager.Instance.WorldToCell(startPosition, out int row, out int col);
                startedFromBoard = PipeManager.Instance.IsInsideBoard(row, col);
                clearedFromBoard = false;

                // 如果从棋盘拖起 → 清空格子 + 隐藏背景
                if (startedFromBoard && !clearedFromBoard)
                {
                    PipeManager.Instance.ClearPipeAt(row, col);
                    clearedFromBoard = true;

                    // 找到背景子物体并隐藏
                    Transform background = transform;
                    SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = false;
                }
            }
        }


        if (isDragging && Input.GetMouseButton(1))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mouseWorld + offset;
        }


        if (isDragging && Input.GetMouseButtonUp(1))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PipeManager.Instance.WorldToCell(mouseWorld, out int row, out int col);

            if (PipeManager.Instance.IsInsideBoard(row, col) &&
                PipeManager.Instance.IsEmptyAt(row, col))
            {

                PipeManager.Instance.PlacePipeAt(pipe, row, col);
                if (PipeManager.Instance != null && PipeManager.Instance.isActiveAndEnabled)
                    _ = PipeManager.Instance.ShowHintWrapper();
            }
            else
            {

                if (startedFromBoard)
                {
                    // 棋盘里拖出来的 → 放到固定位置
                    transform.position = PipeManager.Instance.externalDropPos;
                }
                else
                {
                    // 外部候选 pipe → 回到自己的起始位置
                    transform.position = startPosition;
                }

            }

            isDragging = false;
        }
    }
}