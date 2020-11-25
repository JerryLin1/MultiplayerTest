using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;

public class Player : NetworkedBehaviour
{
    public string username;
    public GameObject bulletPrefab;
    public float movementSpeed = 5f;
    private Transform handPivot;
    private Transform hand;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector3 playerMousePos;
    private float haxis;
    private float vaxis;
    private float bulletSpeed = 15f;
    private float gunCD = 0.5f;
    private float gunCDElapsed = 0f;
    private UIManager ui;
    [HideInInspector]
    public NetworkedVarInt playerScore = new NetworkedVarInt(new NetworkedVarSettings {WritePermission = NetworkedVarPermission.Everyone}, 0);
    private NetworkedVarFloat handRotation = new NetworkedVarFloat(new NetworkedVarSettings {WritePermission = NetworkedVarPermission.OwnerOnly}, 180);
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
        // if (rb.velocity.x > 0.1 && transform.rotation.z >= -20)
        // {
        //     float z = transform.rotation.z;
        //     transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, z - 5, transform.rotation.w);
        // }
        // else if (rb.velocity.x < -0.1 && transform.rotation.z <= 20)
        // {
        //     float z = transform.rotation.z;
        //     transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, z + 5, transform.rotation.w);
        // }
        // else if (transform.rotation)
        // {

        // }
        if (!IsLocalPlayer) return;

        haxis = Input.GetAxisRaw("Horizontal");
        vaxis = Input.GetAxisRaw("Vertical");

        // fire gun
        if (Input.GetMouseButtonDown(0) && gunCDElapsed <= 0f)
        {
            Vector3 d = direction;
            InvokeServerRpc(SpawnBullet, username, bulletSpeed, d.normalized);
            SpawnBulletClient(username, bulletSpeed, d.normalized);
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
    void SpawnBulletClient(string username, float bulletSpeed, Vector3 direction) {
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
    void Move()
    {
        if (!IsLocalPlayer) return;
        float timeScaler = Time.deltaTime * 100;
        rb.velocity = new Vector2(haxis * movementSpeed * timeScaler, vaxis * movementSpeed * timeScaler);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Bullet b = other.gameObject.GetComponent<Bullet>();
            // InvokeServerRpc(ui.AddPoint, b.ownerName);
            Destroy(other.gameObject);
        }
    }
}
