using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkOptimizer : MonoBehaviour
{
    public float maxDist;
    float dist;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(player.position, transform.position);
        if(dist > maxDist)
        {

        }
    }
}
