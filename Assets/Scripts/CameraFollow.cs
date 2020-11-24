using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform leader;
    void FixedUpdate()
    {
        if (leader != null) {
            transform.position = new Vector3(leader.position.x, leader.position.y, transform.position.z);
        }
    }
    public void SetLeader(Transform leader) {this.leader = leader;}
}
