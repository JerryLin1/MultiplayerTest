using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    Camera mainCam;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetEnable(false);
    }
    void FixedUpdate()
    {
        Vector3 mouseposition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouseposition.x, mouseposition.y, transform.position.z);
    }
    public void SetEnable(bool isEnabled)
    {
        spriteRenderer.enabled = isEnabled;
    }
}
