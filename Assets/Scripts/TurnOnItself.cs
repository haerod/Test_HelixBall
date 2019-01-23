using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnItself : MonoBehaviour
{
    void Update()
    {
        transform.RotateAround(transform.position, 1f * Time.deltaTime);
    }
}
