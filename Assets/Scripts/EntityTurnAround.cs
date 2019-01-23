using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTurnAround : MonoBehaviour
{
    [Header("Turn around")]

    [SerializeField] [Tooltip("Speed of rotation around the pillar")]
    float rotationSpeed = 2;
    [SerializeField] [Tooltip("Maximum speed of rotation")]
    float maxRotationSpeed = 2;

    protected bool canTurn = true;

    protected void TurnAround()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void AddRotationSpeed(float value)
    {
        rotationSpeed += value;
        rotationSpeed = Mathf.Clamp(rotationSpeed, -maxRotationSpeed, maxRotationSpeed);
    }

    public void SetRotationSpeed(float value)
    {
        rotationSpeed = value;
        rotationSpeed = Mathf.Clamp(rotationSpeed, -maxRotationSpeed, maxRotationSpeed);
    }

    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }

    public void SetCanTurn(bool canEntityTurn)
    {
        canTurn = canEntityTurn;
    }

    public bool GetCanTurn()
    {
        return canTurn;
    }

}
