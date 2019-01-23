using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
    [SerializeField]
    bool keyboadDebug;

    [Space]

    [SerializeField]
    BouncyBall ball;
    GameCamera cam;

    Vector2 startPosition;
    Vector2 movedPosition;
    Touch touch; // Stock the first touch
    float distance;

    [SerializeField]
    Text debugText;

    void Awake()
    {
        cam = Camera.main.GetComponent<GameCamera>();    
    }

    void Update()
    {
        // On touch, get distance to transform in force
        if (Input.touchCount > 0)
            distance = GetGestureDistanceX(startPosition, movedPosition);
        else
        {
            distance = 0;
        }
        // Keyboard debug
        if (keyboadDebug)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                distance = 50;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                distance = -50;
            }
            else
                distance = 0;
        }

        ball.SetRotationSpeed(distance * 0.7f);
    }

    // Returns the X axis distance between the start point and the current point
    float GetGestureDistanceX(Vector2 start, Vector2 current)
    {
        touch = Input.touches[0];
        switch (touch.phase)
        {
            case TouchPhase.Began:
                startPosition = touch.position;
                movedPosition = startPosition;
                break;
            case TouchPhase.Moved:
                movedPosition = touch.position;
                break;
            case TouchPhase.Ended:
                distance = 0;
                break;
        }

        //startPosition = Vector3.Lerp(startPosition, movedPosition, .2f);

        float dist = start.x - current.x;
        //debugText.text = dist.ToString();

        return dist;
    }

}
