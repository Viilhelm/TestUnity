using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [HideInInspector] public bool IsDraggable = false;

    [HideInInspector] public bool IsFilled;
    [HideInInspector] public int PipeType;

    [SerializeField] private Transform[] _pipePrefabs;

    private Transform currentPipe;
    private int rotation;

    private SpriteRenderer emptySprite;
    private SpriteRenderer filledSprite;
    private List<Transform> connectBoxes;

    private const int minRotation = 0;
    private const int maxRotation = 3;
    private const int rotationMultiplier = 90;

    public void Init(int pipe)
    {
        PipeType = pipe % 10;
        transform.localScale = Vector3.one * PipeManager.Instance.cellSize;
        currentPipe = Instantiate(_pipePrefabs[PipeType], transform);
        currentPipe.transform.localPosition = Vector3.zero;
        if (PipeType == 1 || PipeType == 2)
        {
            rotation = pipe / 10;
        }
        else
        {
            rotation = Random.Range(minRotation, maxRotation + 1);
        }
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

        if (PipeType == 0 || PipeType == 1)
        {
            IsFilled = true;
        }

        if (PipeType == 0)
        {
            return;
        }

        emptySprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
        emptySprite.gameObject.SetActive(!IsFilled);
        filledSprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
        filledSprite.gameObject.SetActive(IsFilled);

        connectBoxes = new List<Transform>();
        for (int i = 2; i < currentPipe.childCount; i++)
        {
            connectBoxes.Add(currentPipe.GetChild(i));
        }

        UpdateFilled();
    }

    public void UpdateInput()
    {
        if (PipeType == 0 || PipeType == 1 || PipeType == 2)
            return;

        rotation = (rotation + 1) % (maxRotation + 1);
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

        // 旋转后立即刷新连通
        if (PipeManager.Instance != null)
            PipeManager.Instance.StartCoroutine(PipeManager.Instance.ShowHintWrapper());
    }

    public void UpdateFilled()
    {
        if (PipeType == 0) return;
        emptySprite.gameObject.SetActive(!IsFilled);
        filledSprite.gameObject.SetActive(IsFilled);
    }

    //public List<Pipe> ConnectedPipes()
    //{
    //    List<Pipe> result = new List<Pipe>();

    //    foreach (var box in connectBoxes)
    //    {
    //        RaycastHit2D[] hit = Physics2D.RaycastAll(box.transform.position, Vector2.zero, 0.1f);
    //        Debug.Log($"{name} connectBox at {box.position}");
    //        //for (int i = 0; i < hit.Length; i++)
    //        //{
    //        //    result.Add(hit[i].collider.transform.parent.parent.GetComponent<Pipe>());
    //        //}
    //        for (int i = 0; i < hit.Length; i++)
    //        {
    //            if (hit[i].collider == null) continue;

    //            Transform t = hit[i].collider.transform;

    //            Transform target = t.parent != null ? t.parent.parent : null;
    //            if (target == null) continue;

    //            Pipe pipe = target.GetComponent<Pipe>();
    //            if (pipe != null && pipe != this)
    //            {
    //                Debug.Log($"{name} at {transform.position} connected to {pipe.name} at {pipe.transform.position}");
    //                result.Add(pipe);
    //            }
    //        }
    //    }

    //    return result;
    //}

    public bool[] GetOpenings()
    {
        bool[] openings = new bool[4];
        if (connectBoxes == null) return openings;

        foreach (var box in connectBoxes)
        {
            Vector2 d = (Vector2)(box.position - transform.position); // 世界坐标差
            // 归一化到主方向，容差避免微小偏移
            if (Mathf.Abs(d.x) > Mathf.Abs(d.y))
            {
                if (d.x > 0.05f) openings[1] = true;   // 右
                else if (d.x < -0.05f) openings[3] = true; // 左
            }
            else
            {
                if (d.y > 0.05f) openings[0] = true;   // 上
                else if (d.y < -0.05f) openings[2] = true; // 下
            }
        }
        return openings;
    }

    public List<Pipe> ConnectedPipes()
    {
        List<Pipe> result = new List<Pipe>();
        PipeManager mgr = PipeManager.Instance;

        // 找到自己在棋盘里的坐标
        int row, col;
        mgr.WorldToCell(transform.position, out row, out col);

        bool[] myOpen = GetOpenings();

        // 上
        if (myOpen[0])
        {
            Pipe n = mgr.GetPipe(row + 1, col);
            if (n != null && n.GetOpenings()[2]) result.Add(n);
        }
        // 右
        if (myOpen[1])
        {
            Pipe n = mgr.GetPipe(row, col + 1);
            if (n != null && n.GetOpenings()[3]) result.Add(n);
        }
        // 下
        if (myOpen[2])
        {
            Pipe n = mgr.GetPipe(row - 1, col);
            if (n != null && n.GetOpenings()[0]) result.Add(n);
        }
        // 左
        if (myOpen[3])
        {
            Pipe n = mgr.GetPipe(row, col - 1);
            if (n != null && n.GetOpenings()[1]) result.Add(n);
        }

        return result;
    }


    public int GetRotationIndex() { return rotation; }

    public void SetRotationIndex(int rot)
    {
        rotation = ((rot % (maxRotation + 1)) + (maxRotation + 1)) % (maxRotation + 1);
        if (currentPipe != null)
            currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);
    }

    public void RefreshInput()
    {
        if (currentPipe != null)
            currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * 90);
    }


}
