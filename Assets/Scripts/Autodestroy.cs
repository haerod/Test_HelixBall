using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    [SerializeField][Tooltip("The gameobject destroy itself after this time")]
    float destroyTime;

    void Start()
    {
        Destroy(gameObject, destroyTime);   
    }
}
