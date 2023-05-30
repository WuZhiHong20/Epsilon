using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Transform transform;
    private Vector3 move;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        move = Vector3.zero;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        move.x = Mathf.Sin(time);
        transform.position = move;
    }
}
