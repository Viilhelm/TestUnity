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
                        _ = PipeManager.Instance.StartCoroutine(PipeManager.Instance.ShowHintWrapper());
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
                    _ = PipeManager.Instance.StartCoroutine(PipeManager.Instance.ShowHintWrapper());
            }
        }

        // ---------------- �Ҽ���ʼ��ק ----------------
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

                // ������������� �� ��ո��� + ���ر���
                if (startedFromBoard && !clearedFromBoard)
                {
                    PipeManager.Instance.ClearPipeAt(row, col);
                    clearedFromBoard = true;

                    // �ҵ����������岢����
                    Transform background = transform;
                    SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.enabled = false;

                    pipe.IsFilled = false;
                    pipe.UpdateFilled();
                }
            }
        }


        if (isDragging && Input.GetMouseButton(1))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mouseWorld + offset;

            // ֻ�����̷�Χ��ʱ����
            PipeManager.Instance.WorldToCell(mouseWorld, out int row, out int col);
            if (PipeManager.Instance.IsInsideBoard(row, col))
            {
                if (PipeManager.Instance != null && PipeManager.Instance.isActiveAndEnabled)
                    _ = PipeManager.Instance.StartCoroutine(PipeManager.Instance.ShowHintWrapper());
            }
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
                    _ = PipeManager.Instance.StartCoroutine(PipeManager.Instance.ShowHintWrapper());
            }
            else
            {

                if (startedFromBoard)
                {
                    // �������ϳ����� �� �ŵ��̶�λ��
                    //transform.position = PipeManager.Instance.externalDropPos;
                    // ���̿��
                    float boardWidth = PipeManager.Instance.Level.Column * PipeManager.Instance.cellSize;
                    float boardHeight = PipeManager.Instance.Level.Row * PipeManager.Instance.cellSize;

                    // ���������½ǣ�����ƫ 2 ������ƫ 1 ��
                    float dropX = boardWidth + 2f * PipeManager.Instance.cellSize;
                    float dropY = -1f * PipeManager.Instance.cellSize;

                    transform.position = new Vector2(dropX, dropY);

                    // ȷ������һ��
                    transform.localScale = Vector3.one * PipeManager.Instance.cellSize;
                }
                else
                {
                    // �ⲿ��ѡ pipe �� �ص��Լ�����ʼλ��
                    transform.position = startPosition;
                }

            }

            isDragging = false;
        }
    }
}