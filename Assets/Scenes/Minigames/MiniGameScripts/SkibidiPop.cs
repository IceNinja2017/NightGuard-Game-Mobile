using UnityEngine;

public class SkibidiPop : MonoBehaviour
{
    public float popHeight = 1.5f;
    public float moveSpeed = 20f;

    private Vector3 downPos;
    private Vector3 upPos;
    private bool isUp = false;
    private float timer;

    void Start()
    {
        downPos = transform.localPosition;
        upPos = new Vector3(downPos.x, downPos.y + popHeight, downPos.z);
        SetRandomTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            isUp = !isUp;
            SetRandomTimer();
        }

        Vector3 target = isUp ? upPos : downPos;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed * Time.deltaTime);
    }

    void SetRandomTimer()
    {
        timer = isUp ? Random.Range(0.5f, 0.8f) : Random.Range(0.5f, 2.0f);
    }

    void OnMouseDown()
    {
        if (isUp)
        {
            Debug.Log("SKIBIDI WHACKED!");
            isUp = false;
            timer = Random.Range(1.0f, 2.0f); 
        }
    }
}