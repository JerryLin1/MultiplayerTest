using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
public class Bullet : NetworkedBehaviour
{
    float lifeSpan = 2;
    public string ownerName;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeSpan-=Time.deltaTime;
        if (lifeSpan<=0) {
            Destroy(gameObject);
        }
    }
    [ServerRPC]
    public void Fired(string ownerName, float speed, Vector3 direction) {
        GetComponent<Rigidbody2D>().velocity = direction*speed;
        this.ownerName = ownerName;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
