using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public SpriteRenderer wireEnd;
    public GameObject lightOn;
    public Camera WireCam;
    Vector3 startPoint;
    Vector3 startPosition;


    private void Start()
    {
        startPoint = transform.parent.position;
        startPosition = transform.position;

    }
    private void OnMouseDrag()
    {
        Vector3 mousePos = Input.mousePosition;

        float distance =
            Mathf.Abs(WireCam.transform.position.z - startPosition.z);

        mousePos.z = distance;

        Vector3 newPosition =
            WireCam.ScreenToWorldPoint(mousePos);

        newPosition.z = startPosition.z;

        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(newPosition, .5f);


        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            UpdateWire(collider.transform.position);

            if (transform.parent.name ==
               collider.transform.parent.name)
            {
                MaIn.Instance.SwitchChange(1);

                collider.GetComponent<Wire>()?.Done();
                Done();
            }

            return;
        }

        UpdateWire(newPosition);
    }
    void Done()
    {
        lightOn.SetActive(true);

        Destroy(this);
    }
    private void OnMouseUp()
    {
        //reset wire position 
        UpdateWire(startPosition);  
    }
    void UpdateWire(Vector3 newPosition)
    {
        transform.position = newPosition;

        Vector3 direction = transform.position - startPoint;
        transform.right = direction * transform.lossyScale.x;

        float dist = Vector2.Distance(startPoint, newPosition);
        wireEnd.size = new Vector2(dist, wireEnd.size.y);
    }
}

