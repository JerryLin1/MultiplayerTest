using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
public class Bullet : NetworkedBehaviour
{
    float lifeSpan = 5;
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
    public void Fired(string ownerName, float speed, Vector3 direction) {
        GetComponent<Rigidbody2D>().velocity = direction*speed;
    }
    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            Destroy(gameObject);
        }
    }
}
