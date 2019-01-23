using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField]
    int difficulty; // Between 0 and 4

    [SerializeField]
    bool hasBeenDestroyed = false; // Between 0 and 4

    const float distFromCenter = 5;
    const int nbrOfElements = 15;

    Transform platform;
    List<string> gatchaRewardList = new List<string>();

    void Awake()
    {
    }

    public void SetDifficulty(int value)
    {
        difficulty = value;
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    public bool GetHasBeenDestroyed()
    {
        return hasBeenDestroyed;
    }

    public void SetHasBeenDestroyedTrue()
    {
        hasBeenDestroyed = true;
    }

    // Create the platfoms
    public void BuildPlatforms()
    {
        GameObject prefabToInsta = null;
        GameObject bonusPrefab = null;
        float angleIteration;
        float currentRotation;

        string[] tagsArray = GenerateTagsArray();
        int difficultyPoints = difficulty;

        if(Random.Range(0, 101) < difficulty*3) // "difficulty x 3" % of chance to have a bonus
        {
            bonusPrefab = BonusGatcha();
        }

        // Generate elements
        for (int i = 0; i < nbrOfElements; i++)
        {
            switch (tagsArray[i])
            {
                case "Hole":
                    prefabToInsta = LevelManager.instance.hole;
                    break;

                case "Platform":
                    prefabToInsta = LevelManager.instance.platform;
                    break;

                case "Obstacle":
                    if (difficultyPoints > 4)
                    {
                        int rand = Random.Range(0, 2);
                        switch(rand)
                        {
                            case 0:
                                prefabToInsta = LevelManager.instance.growingObstacle;
                                difficultyPoints -= 2;
                                break;
                            case 1:
                                prefabToInsta = LevelManager.instance.timedObstacle;
                                difficultyPoints -= 1;
                                break;
                        }
                    }
                    else
                        prefabToInsta = LevelManager.instance.obstacle;
                    break;
                default:
                    Debug.LogError("An unknown or null tag is called : " + tagsArray[i]);
                    break;
            }

            angleIteration = 360 / nbrOfElements;
            currentRotation = angleIteration * i;

            Transform element = Instantiate(prefabToInsta, transform.position, Quaternion.identity, transform).transform;
            element.Rotate(new Vector3(0, currentRotation, 0));
            element.Translate(new Vector3(distFromCenter, 0, 0));

            if(tagsArray[i] == "Platform" && bonusPrefab)
            {
                Instantiate(bonusPrefab, element.transform.position + Vector3.up, bonusPrefab.transform.rotation, transform);
                bonusPrefab = null;
            }
        }
    }

    // Returns an array of tags for the future new plaftorms
    string[] GenerateTagsArray()
    {
        List<int> platformNbrList = new List<int>();
        string[] platformTagsArray = new string[nbrOfElements];

        for (int i = 0; i < nbrOfElements; i++) // Full the platform list
        {
            platformNbrList.Add(i);
        }

        // Shuffle list
        for (int i = 0; i < platformNbrList.Count; i++)
        {
            int temp = platformNbrList[i];
            int randomIndex = Random.Range(i, platformNbrList.Count);
            platformNbrList[i] = platformNbrList[randomIndex];
            platformNbrList[randomIndex] = temp;
        }

        // Fist element of platformNbrList is the tagged whith HOLE
        platformTagsArray[platformNbrList[0]] = "Hole";

        // Next elements are OBSTACLES
        int obstacleCount = Mathf.Clamp(difficulty + 1, 0, 4);
        for (int i = 1; i < obstacleCount; i++)
        {
            platformTagsArray[platformNbrList[i]] = "Obstacle";
        }

        int standingPlatforms = Mathf.Clamp(difficulty + 1, 0, 4);

        // Last elements are SAFE PLATFORMS
        for (int i = standingPlatforms; i < nbrOfElements; i++)
        {
            platformTagsArray[platformNbrList[i]] = "Platform";
        }

        return platformTagsArray;
    }

    // Retruns randomly a bonus
    GameObject BonusGatcha()
    {
        gatchaRewardList.Clear();
        FullGatchaList("Torch", PlayerGameData.instance.GetGatchaTorchValue());
        FullGatchaList("SoftGem", PlayerGameData.instance.GetGatchaSoftGemValue());
        FullGatchaList("HardGem", PlayerGameData.instance.GetGatchaHardGemValue());

        string reward  = gatchaRewardList[Random.Range(0, gatchaRewardList.Count)];

        switch(reward)
        {
            case "Torch":
                return LevelManager.instance.torch;
            case "SoftGem":
                return LevelManager.instance.softGem;
            case "HardGem":
                return LevelManager.instance.hardGem;
            default:
                return null;
        }
    }

    // Linked with BonusGatcha()
    void FullGatchaList(string objectName, int gatchaObjectCount)
    {
        for (int i = 0; i < gatchaObjectCount; i++)
        {
            gatchaRewardList.Add(objectName);
        }
    }
}

