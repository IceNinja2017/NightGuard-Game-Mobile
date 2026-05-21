using System.Collections;
using UnityEngine;

public class HeadPop : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Sprite headSprite;
    [SerializeField] private Sprite headhit;
    [SerializeField] private BathroomGameManager gameManager;
    [SerializeField] private int headIndex;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private float showDuration = 0.3f;
    private float duration = 1f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Vector2 boxOffset;
    private Vector2 boxSize;
    private Vector2 boxOffsetHidden;
    private Vector2 boxSizeHidden;

    private bool hittable = true;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = transform.localPosition;
        endPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 1.7f);

        boxOffset = boxCollider.offset;
        boxSize = boxCollider.size;
        boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
        boxSizeHidden = new Vector2(boxSize.x, 0f);

        Hide();
    }

    public void Activate(int level)
    {
        SetLevel(level);
        CreateNext();
        StopAllCoroutines();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }

    public void Hide()
    {
        transform.localPosition = startPosition;
        boxCollider.offset = boxOffsetHidden;
        boxCollider.size = boxSizeHidden;
    }

    public void CreateNext()
    {
        spriteRenderer.sprite = headSprite;
        hittable = true;
    }

    private void SetLevel(int level)
    {
        float durationMin = Mathf.Clamp(1f - level * 0.1f, 0.1f, 1f);
        float durationMax = Mathf.Clamp(2f - level * 0.1f, 0.2f, 2f);
        duration = Random.Range(durationMin, durationMax);
    }

    private IEnumerator ShowHide(Vector2 start, Vector2 end)
    {
        // Move UP
        float elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
            boxCollider.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
            boxCollider.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = end;
        boxCollider.offset = boxOffset;
        boxCollider.size = boxSize;

        yield return new WaitForSeconds(duration);

        // Move DOWN
        elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
            boxCollider.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = start;
        boxCollider.offset = boxOffsetHidden;
        boxCollider.size = boxSizeHidden;

        gameManager.Missed(headIndex, true);
    }

    private IEnumerator HitSlideDown()
    {
        Vector2 currentPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(currentPos, startPosition, elapsed / showDuration);
            boxCollider.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Hide();
    }

    private void OnMouseDown()
    {
        if (hittable)
        {
            hittable = false;
            spriteRenderer.sprite = headhit;
            gameManager.AddScore(headIndex);

            StopAllCoroutines();
            StartCoroutine(HitSlideDown());
        }
    }
}