using DG.Tweening.Core.Easing;
using System.Collections;
using UnityEngine;

public class HeadPop : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Sprite headSprite;
    [SerializeField] private Sprite headhit;

    private Vector2 startPosition; //Kung naa Sa Ubus
    private Vector2 endPosition; //Kung naa Sa Taas
    private float showDuration = 0.5f;
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
         
    }

    private void Start()
    {
        Activate(1);
    }

    public void Activate(int level)
    {
        SetLevel(level);
        CreateNext();
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
        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);

        duration = Random.Range(durationMin, durationMax);
    }

    private IEnumerator ShowHide(Vector2 start, Vector2 end)
    {
        transform.localPosition = start;
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
    }

    private IEnumerator QuickHide()
    {
        yield return new WaitForSeconds(0.25f);
        Hide();
    }

    private void OnMouseDown()
    {
        if(hittable)
        {
            spriteRenderer.sprite = headhit;

            StopAllCoroutines();
            StartCoroutine(QuickHide());
            hittable = false;
        }
    }

}
