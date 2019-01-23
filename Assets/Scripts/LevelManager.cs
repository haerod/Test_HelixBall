using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Metrics")]

    const float distanceBetweenLvlPlatforms = 5f;
    List<Transform> lvlPlatformsToDestroy = new List<Transform>();
    int currentLevel = 0; // Level of the last platform generated

    [Header("Objects prefabs")]
    public GameObject levelPlatform;
    public GameObject obstacle;
    public GameObject bigObstacle;
    public GameObject growingObstacle;
    public GameObject timedObstacle;
    public GameObject platform;
    public GameObject hole;
    public GameObject torch;
    public GameObject softGem;
    public GameObject hardGem;

    [Header("Scene references")]
    public Transform charaTrans;
    public BouncyBall chara;
    public GameObject panelDeath;
    public GameObject panelReward;
    public Text txtSoftGems;
    public Text txtHardGems;
    public Text txtLevel;

    [Header("FX")]
    public GameObject explosionFx;
    public GameObject rockFx;
    public GameObject bounceFx;
    public GameObject hitFx;
    public GameObject deadLight;
    public GameObject torchTakenFx;
    public GameObject softGemFx;
    public GameObject hardGemFx;
    //public GameObject hammerModeFx;

    [Header("SFX")]
    public GameObject bounceSfx;
    public GameObject breakHoleSfx;
    public GameObject hitSfx;
    public GameObject torchTakenSfx;
    public GameObject lightReductionSfx;
    public GameObject hammerModeSfx;

    [Header("Materials")]
    public Material platformMaterial;
    public Material obstacleMaterial;
    public Material holeMaterial;
    public Material playerMaterial;
    public Material playerFaceMaterial;
    public Material playerHammerFaceMaterial;
    public Material playerHammerBodyMaterial;

    void Awake()
    {
        // SINGLETON
        if (!instance)
            instance = this;
        else
            Destroy(this);

        chara = charaTrans.GetComponent<BouncyBall>();

        for (int i = 0; i < 3; i++)
        {
            GenerateLevelPlatform();
        }

        StartCoroutine(TimeTick());
    }

    // Instantiate a new platform
    public void GenerateLevelPlatform()
    {
        PlatformGenerator lvlPlatform;

            Vector3 poz = new Vector3(
                0,
                currentLevel * -distanceBetweenLvlPlatforms,
                0);
            
            lvlPlatform = Instantiate(levelPlatform, poz, Quaternion.identity, transform).GetComponent<PlatformGenerator>();

            lvlPlatform.SetDifficulty(currentLevel);
            lvlPlatform.BuildPlatforms();

        currentLevel++;
    }

    // Add a level platform in the list to destroy it and destroy old platforms
    public void AddLevelPlatformToDestroy(Transform lvlPlat)
    {
        if (lvlPlatformsToDestroy.Count > 1)
        {
            Destroy(lvlPlatformsToDestroy[0].gameObject);
            lvlPlatformsToDestroy.RemoveAt(0);
        }
        lvlPlatformsToDestroy.Add(lvlPlat);
    }

    // Displays the panel reward with the hard and soft gems current count
    public void DisplayPanelReward()
    {
        txtHardGems.text = PlayerGameData.instance.GetHardGemsCount().ToString();
        txtSoftGems.text = PlayerGameData.instance.GetSoftGemsCount().ToString();
        panelReward.SetActive(true);
    }

    // Click on the button Second descent
    public void ClickOnNewDescent()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    // Click on the button Back to house
    public void ClickOnGoBackToHouse()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    // Create a tick to the light reduction
    IEnumerator TimeTick()
    {
        yield return new WaitForSeconds(20f);
        chara.LightReduction();
        StartCoroutine(TimeTick());
        Instantiate(lightReductionSfx, Camera.main.transform.position, Quaternion.identity);
    }

    // Return the current CONSTUCTION level
    public void DisplayRecord()
    {
        txtLevel.text = "You was on the <b>" + (currentLevel - 2) + "</b> level !";
        if (currentLevel - 2 > PlayerGameData.instance.GetGreaterLevel())
            txtLevel.text += "\n<b>New record !</b>";
        else
            txtLevel.text += "\nBest level: " + (PlayerGameData.instance.GetGreaterLevel() +2);
    }
}
