using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wife : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeMation();
    }

    private void ChangeMation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Space");
        }
    }
}
