using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bullet : MonoBehaviour
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
    public void Fired(string ownerName, float speed, Vector3 direction) {
        GetComponent<Rigidbody2D>().velocity = direction*speed;
        this.ownerName = ownerName;
    }
}
