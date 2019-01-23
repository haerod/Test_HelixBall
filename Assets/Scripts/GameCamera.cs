using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : EntityTurnAround
{
    [SerializeField][Tooltip("Offset in Y between the camera and the current platform")]
    float offsetY;

    Transform currentLevelPlatform;
    Transform charaT;
    BouncyBall chara;
    Vector3 newPozY;

    public void SetChara(BouncyBall ball)
    {
        charaT = ball.transform;
        chara = ball;
    }

    void Update()
    {
        if (canTurn)
        {
            SetRotationSpeed(chara.GetRotationSpeed());
            TurnAround();
        }

        if (currentLevelPlatform)
            FollowCurrentPlatform();
        else
            FollowChara();
    }

    // Do a screenshake with "length" moves with an intensity of "intensty" (between 0 and 1)
    public void Screenshake(int length = 6, float intensity = .1f)
    {
        intensity = Mathf.Clamp(intensity, 0, 1);
        StartCoroutine(ScreenshakeCoroutine(length, intensity));
    }
    // Coroutine of the Screenshake method
    IEnumerator ScreenshakeCoroutine(int length, float intensity)
    {
        Vector3 basePoz = transform.position;
        for (int i = 0; i < length; i++)
        {
            transform.position = new Vector3(
                basePoz.x + UnityEngine.Random.Range(-intensity, intensity),
                basePoz.y + UnityEngine.Random.Range(-intensity, intensity),
                basePoz.z + UnityEngine.Random.Range(-intensity, intensity)
                );
            yield return new WaitForSeconds(.01f);
        }
        transform.position = basePoz;
    }

    // Set the variable "currentPlatform"
    public void SetCurrentLevelPlatform(Transform platformTrans)
    {
        currentLevelPlatform = platformTrans;
    }

    // Stay at the level of the current platform
    void FollowCurrentPlatform()
    {
        newPozY = new Vector3(transform.position.x, currentLevelPlatform.position.y + offsetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPozY, .5f);
    }

    void FollowChara()
    {
        newPozY = new Vector3(transform.position.x, charaT.position.y + offsetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPozY, .5f);
    }
}
