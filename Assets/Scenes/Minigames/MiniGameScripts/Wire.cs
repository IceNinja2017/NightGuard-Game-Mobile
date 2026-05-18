using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public SpriteRenderer wireEnd;
    Vector3 startPoint;
    private void Start()
    {
        startPoint = transform.parent.position;
    }
    private void OnMouseDrag()
    {
        Vector3 mousePos = Input.mousePosition;

        // Distance from camera to object
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePos);

        newPosition.z = 0f;

        transform.position = newPosition;

        Vector3 direction = transform.position - startPoint;
        transform.right = direction * transform.lossyScale.x;
        
        float dist = Vector2.Distance(startPoint, newPosition);
        wireEnd.size = new Vector2(dist, wireEnd.size.y);
    }
}

