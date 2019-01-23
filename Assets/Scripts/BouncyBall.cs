using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BouncyBall : EntityTurnAround
{
    [Header("Values")]

    [Tooltip("Bounce power")] [SerializeField]
    float bounceForce = 1;

    [Tooltip("Maximum light intesity")][SerializeField]
    float maxLightIntensity;

    [Tooltip("Maximum light range")][SerializeField]
    float maxLightRange;

    [Tooltip("Minimum light intesity")]
    [SerializeField]
    float minLightIntensity;

    [Tooltip("Minimum light range")]
    [SerializeField]
    float minLightRange;

    float lightIntensityReduction;
    float lightRangeReduction;
    float currentLightIntensity;
    float currentLightRange;

    GameObject panelDeath;
    Rigidbody rb;
    Color ballColor;
    Animator anim;
    SphereCollider hitbox;
    GameCamera cam;
    Transform currentLvlPlatform = null;
    Light spot;
    MeshRenderer[] faceMeshRend;
    MeshRenderer mr;
    bool isChainingHoles = false;
    bool hammerFall = false;
 
    void Awake()
    {
        panelDeath = LevelManager.instance.panelDeath;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.SetBool("menu", false);
        ballColor = GetComponent<MeshRenderer>().material.color;
        hitbox = GetComponent<SphereCollider>();
        mr = GetComponent<MeshRenderer>();

        faceMeshRend = new MeshRenderer[3];
        for (int i = 0; i < 3; i++)
        {
            faceMeshRend[i] = transform.GetChild(i).GetComponent<MeshRenderer>();
        }

        cam = Camera.main.GetComponent<GameCamera>();
        cam.SetChara(this);
        spot = cam.GetComponentInChildren<Light>();

        currentLightIntensity = maxLightIntensity;
        currentLightRange = maxLightRange;
        spot.intensity = currentLightIntensity;
        spot.range = currentLightRange;
        lightIntensityReduction = 0.1f * maxLightIntensity;
        lightRangeReduction = 0.1f * maxLightRange;
    }

    void Update()
    {
        if(canTurn)
            TurnAround();

        if (rb) // Clamp bounce (if ball touch 2 platforms)
        {
            if (rb.velocity.y > bounceForce) 
                rb.velocity = new Vector3(0, bounceForce, 0);
        }

        if(spot.intensity >= minLightIntensity && spot.range >= minLightRange)
        {
            spot.intensity = Mathf.Lerp(spot.intensity, currentLightIntensity, .3f);
            spot.range = Mathf.Lerp(spot.range, currentLightRange, .3f);
        }   
    }

    #region COLLISIONS

    void OnCollisionEnter(Collision collision)
    {
        if (canTurn)
        {
            if (collision.transform.tag == "Platform")
                if (!hammerFall)
                    CollideSafePlatform(collision);
                else
                {
                    CollideHole(collision.transform);
                    EndHammerFall();
                }
            else if (collision.transform.tag == "Obstacle")
                if (!hammerFall)
                    CollideObstacle(collision.GetContact(0).point);
                else
                {
                    CollideHole(collision.transform);
                    EndHammerFall();
                }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Hole")
            CollideHole(other.transform);
        else if (other.transform.tag == "Torch")
            CollideTorch(other.transform);
        else if (other.transform.tag == "HardGem")
            CollideHardGem(other.transform);
        else if (other.transform.tag == "SoftGem")
            CollideSoftGem(other.transform);
    }

    void CollideSafePlatform(Collision col)
    {
        isChainingHoles = false;
        CheckLevelPlatform(col.transform);

        Vector3 bounce = new Vector3(0, bounceForce, 0); // Add force to bounce
        rb.AddForce(bounce, ForceMode.Impulse);

        Quaternion randRot = Quaternion.Euler(0, Random.Range(0, 360), 0); // Random rotation on Y for the FX

        Instantiate(LevelManager.instance.bounceFx, col.GetContact(0).point, randRot);
        Instantiate(LevelManager.instance.bounceSfx, col.GetContact(0).point, randRot);

        anim.SetTrigger("bounce");
    }

    void CollideObstacle(Vector3 colPoz)
    {
        isChainingHoles = false;
        Instantiate(LevelManager.instance.hitFx, colPoz, Quaternion.identity);
        Instantiate(LevelManager.instance.hitSfx, colPoz, Quaternion.identity);
        Death();
    }

    void CollideHole(Transform hole)
    {
        if (!isChainingHoles && !hammerFall) // Is the chara chaining holes without touch the floor ?
            isChainingHoles = true;
        else
            StartHammerFall(); // HAMMER FALL (destroy the next platform)

        if (currentLvlPlatform != hole.parent) // if it's a new Level platform (when the chara falls directly on the hole)
            currentLvlPlatform = hole.parent;

        Collider[] col = new Collider[0];
        for (int i = 0; i < currentLvlPlatform.childCount; i++) // Destroy all colliders of the level platform
        {
            col = currentLvlPlatform.GetChild(i).GetComponents<Collider>(); // Get all the colliders
            if (col.Length > 0)
            {
                for (int j = 0; j < col.Length; j++)
                {
                    Destroy(col[j]);
                }
            }
        }

        PlatformGenerator collidedPlatform = currentLvlPlatform.GetComponent<PlatformGenerator>();

        if (collidedPlatform.GetDifficulty() > PlayerGameData.instance.GetGreaterLevel()) // New record of deepness
            PlayerGameData.instance.SetGreaterLevel(collidedPlatform.GetDifficulty());

        if(!collidedPlatform.GetHasBeenDestroyed()) // If the platform has NOT already been destroyed
        {
            collidedPlatform.SetHasBeenDestroyedTrue();
            PassALevelPlatfrom();
            cam.SetCurrentLevelPlatform(null);
        }

        Instantiate(LevelManager.instance.explosionFx, hole.position, Quaternion.identity);
        Instantiate(LevelManager.instance.rockFx, hole.position, Quaternion.identity);
        Instantiate(LevelManager.instance.breakHoleSfx, hole.position, Quaternion.identity);
        Destroy(hole.gameObject);
    }

    void CollideTorch(Transform torch)
    {
        Instantiate(LevelManager.instance.torchTakenFx, torch.position, Quaternion.identity);
        Instantiate(LevelManager.instance.torchTakenSfx, torch.position, Quaternion.identity);
        currentLightIntensity = maxLightIntensity;
        currentLightRange = maxLightRange;
        Destroy(torch.gameObject, .1f);
    }

    void CollideSoftGem(Transform gem)
    {
        Instantiate(LevelManager.instance.torchTakenFx, gem.position, Quaternion.identity);
        Instantiate(LevelManager.instance.torchTakenSfx, gem.position, Quaternion.identity);
        LevelManager.instance.DisplayPanelReward(); 
        LevelManager.instance.txtSoftGems.GetComponent<CounterText>().SetValueToAdd(PlayerGameData.instance.GetSoftGemCollectedValue());
        Destroy(gem.gameObject, .1f);
    }

    void CollideHardGem(Transform gem)
    {
        Instantiate(LevelManager.instance.torchTakenFx, gem.position, Quaternion.identity);
        Instantiate(LevelManager.instance.torchTakenSfx, gem.position, Quaternion.identity);
        LevelManager.instance.DisplayPanelReward();
        LevelManager.instance.txtHardGems.GetComponent<CounterText>().SetValueToAdd(PlayerGameData.instance.GetHardGemCollectedValue());
        Destroy(gem.gameObject, .1f);
    }

    void StartHammerFall()
    {
        hammerFall = true;
        isChainingHoles = false;
        for (int i = 0; i < faceMeshRend.Length; i++)
        {
            faceMeshRend[i].material = LevelManager.instance.playerHammerFaceMaterial;
        }
        //mr.material = LevelManager.instance.playerHammerBodyMaterial;
        //Instantiate(LevelManager.instance.hammerModeFx, transform.position, Quaternion.identity);
        Instantiate(LevelManager.instance.hammerModeSfx, cam.transform.position, Quaternion.identity);
    }

    void EndHammerFall()
    {
        StartCoroutine(CoroutineEndHammerFall());
    }

    // Debug to dodge bad collision bugs
    IEnumerator CoroutineEndHammerFall()
    {
        yield return new WaitForSeconds(.5f);
        hammerFall = false;
        for (int i = 0; i < faceMeshRend.Length; i++)
        {
            faceMeshRend[i].material = LevelManager.instance.playerFaceMaterial;
        }
        //mr.material = LevelManager.instance.playerMaterial;
    }

    #endregion

    // Add the new level platfrom for the chara and the camera
    void CheckLevelPlatform(Transform collided)
    {
        Transform levelPlatformCollided = collided.parent;
        if (levelPlatformCollided != currentLvlPlatform)
        {
            currentLvlPlatform = levelPlatformCollided;
            cam.SetCurrentLevelPlatform(levelPlatformCollided);
        }
    }

    // When the ball pass across a level platform
    void PassALevelPlatfrom()
    {
        LevelManager.instance.AddLevelPlatformToDestroy(currentLvlPlatform);
        LevelManager.instance.GenerateLevelPlatform();
    }

    void Death()
    {
        Instantiate(LevelManager.instance.deadLight, transform.position, Quaternion.identity); // Display a light where the player is dead
        cam.Screenshake(5, 0.2f);
        canTurn = false;
        anim.SetBool("dead", true);
        cam.SetCanTurn(false);
        rb.velocity = Vector3.zero;
        Destroy(rb);
        StartCoroutine(DeathCoroutine());
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        LevelManager.instance.DisplayRecord();
        panelDeath.SetActive(true);
    }

    #region LIGHT
    
    // Reduce light
    public void LightReduction()
    {
        currentLightIntensity = Mathf.Clamp(currentLightIntensity - lightIntensityReduction, 0, maxLightIntensity);
        currentLightRange = Mathf.Clamp(currentLightRange - lightRangeReduction, 0, maxLightRange);
    }

    #endregion
}

