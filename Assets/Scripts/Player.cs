using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.Spawning;
using DG.Tweening;

public class Player : NetworkedBehaviour
{
    public NetworkedVarString username = new NetworkedVarString(nvsOwner);
    public GameObject bulletPrefab;
    public float movementSpeed = 5f;
    private Transform handPivot;
    private Transform hand;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector3 playerMousePos;
    private static NetworkedVarSettings nvsOwner = new NetworkedVarSettings { WritePermission = NetworkedVarPermission.OwnerOnly};
    private static NetworkedVarSettings nvsEveryone = new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone};
    private NetworkedVarFloat haxis = new NetworkedVarFloat(nvsOwner);
    private float vaxis;
    private float bulletSpeed = 30f;
    private float gunCD = 0.5f;
    private float gunCDElapsed = 0f;
    private UIManager ui;
    [HideInInspector]
    public NetworkedVarInt playerScore = new NetworkedVarInt(nvsEveryone, 0);
    private NetworkedVarFloat handRotation = new NetworkedVarFloat(nvsOwner, 180);
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        handPivot = transform.Find("Hand Pivot");
        hand = handPivot.Find("Hand");
        ui = GameObject.Find("Canvas").GetComponent<UIManager>();

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

        haxis.Value = Input.GetAxisRaw("Horizontal");
        vaxis = Input.GetAxisRaw("Vertical");

        // fire gun
        if (Input.GetMouseButtonDown(0) && gunCDElapsed <= 0f)
        {
            Vector3 d = direction;
            InvokeServerRpc(SpawnBullet, username.Value, bulletSpeed, d.normalized);
            SpawnBulletClient(username.Value, bulletSpeed, d.normalized);
            gunCDElapsed = gunCD;
        }
        gunCDElapsed -= Time.deltaTime;
    }

    [ServerRPC]
    void SpawnBullet(string username, float bulletSpeed, Vector3 direction)
    {
        GameObject IbulletPrefab = Instantiate(bulletPrefab, hand.position, Quaternion.identity);
        IbulletPrefab.GetComponent<Bullet>().Fired(username, bulletSpeed, direction.normalized);
        InvokeClientRpcOnEveryoneExcept(SpawnBulletClient, OwnerClientId, username, bulletSpeed, direction.normalized);
    }
    // wtf is this this is not right bro
    [ClientRPC]
    void SpawnBulletClient(string username, float bulletSpeed, Vector3 direction)
    {
        GameObject IbulletPrefab = Instantiate(bulletPrefab, hand.position, Quaternion.identity);
        IbulletPrefab.GetComponent<Bullet>().Fired(username, bulletSpeed, direction.normalized);
    }
    void PointTowardsCursor()
    {
        hand.localRotation = Quaternion.Euler(0, handRotation.Value, 90);
        if (!IsLocalPlayer) return;
        playerMousePos = Input.mousePosition;
        playerMousePos = Camera.main.ScreenToWorldPoint(playerMousePos);
        direction = new Vector2(playerMousePos.x - transform.position.x, playerMousePos.y - transform.position.y);
        handPivot.up = direction;
        if (transform.position.x > playerMousePos.x)
        {
            handRotation.Value = 180;
        }
        else
        {
            handRotation.Value = 0;
        }
    }
    void RotateWhenMoving()
    {
        if (haxis.Value > 0.1 && transform.rotation.z >= -0.2)
        {
            transform.DORotate(new Vector3(0, 0, -30), 1);
        }
        else if (haxis.Value < -0.1 && transform.rotation.z <= 0.2)
        {
            transform.DORotate(new Vector3(0, 0, 30), 1);
        }
        else
        {
            transform.DORotate(new Vector3(0, 0, 0), 1);
        }
    }
    void Move()
    {
        RotateWhenMoving();
        if (!IsLocalPlayer) return;
        float timeScaler = Time.deltaTime * 100;
        rb.velocity = new Vector2(haxis.Value * movementSpeed * timeScaler, vaxis * movementSpeed * timeScaler);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);
            if (!IsHost) return;
            Bullet b = other.gameObject.GetComponent<Bullet>();
            ui.InvokeServerRpc(ui.AddPoint, b.ownerName);
        }
    }
}
