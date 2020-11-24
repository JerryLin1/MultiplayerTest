using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Player : NetworkedBehaviour
{
    public string username = "joe";
    public GameObject bulletPrefab;
    public float movementSpeed = 5f;
    private Transform handPivot;
    private Transform hand;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector3 mousePosition;
    private float haxis;
    private float vaxis;
    private float bulletSpeed = 15f;
    private float gunCD = 0.5f;
    private float gunCDElapsed = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        handPivot = transform.Find("Hand Pivot");
        hand = handPivot.Find("Hand");

        if (IsLocalPlayer)
        {
            Camera.main.GetComponent<CameraFollow>().SetLeader(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
    }
    void FixedUpdate()
    {
        Move();
        PointTowardsCursor();
    }
    void MoveInput()
    {
        if (!IsLocalPlayer) return;

        haxis = Input.GetAxisRaw("Horizontal");
        vaxis = Input.GetAxisRaw("Vertical");

        // fire gun
        if (Input.GetMouseButtonDown(0) && gunCDElapsed <= 0f)
        {
            GameObject IbulletPrefab = Instantiate(bulletPrefab, hand.position, Quaternion.identity);
            IbulletPrefab.GetComponent<NetworkedObject>().Spawn();
            IbulletPrefab.GetComponent<Bullet>().Fired(username, bulletSpeed, direction.normalized);
            gunCDElapsed = gunCD;
        }
        gunCDElapsed -= Time.deltaTime;
    }
    [ServerRPC]
    void MyMethod(int myInt)
    {

    }

    void PointTowardsCursor()
    {
        if (!IsLocalPlayer) return;
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        handPivot.localScale = (mousePosition.x > transform.position.x) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        handPivot.up = direction;
    }
    void Move()
    {
        if (!IsLocalPlayer) return;
        float timeScaler = Time.deltaTime * 100;
        rb.velocity = new Vector2(haxis * movementSpeed * timeScaler, vaxis * movementSpeed * timeScaler);
    }
}
