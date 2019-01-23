using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterText : MonoBehaviour
{
    [SerializeField]
    bool isHardGemText;

    Text txt;
    int valueToAdd = 0;

    private void Awake()
    {
        txt = GetComponent<Text>();
    }

    public void SetValueToAdd(int value)
    {
        valueToAdd = value;
        if (isHardGemText) // Add the value in the PlayerGameData
            PlayerGameData.instance.AddHardGems(valueToAdd);
        else
            PlayerGameData.instance.AddSoftGems(valueToAdd);
        StartCount();
    }

    // Add the valueToAdd in the text, number by number
    void StartCount()
    {
        StartCoroutine(CoroutineCount());
    }

    IEnumerator CoroutineCount()
    {
        int currentValue = int.Parse(txt.text); // get the current value in the text

        txt.color = Color.white;
        for (int i = 0; i < valueToAdd; i++) // add numbers one by one
        {
            currentValue++;
            txt.text = currentValue.ToString();
            yield return new WaitForSeconds(.1f);
        }

        txt.color = Color.black;
        valueToAdd = 0;

        yield return new WaitForSeconds(1f);

        LevelManager.instance.panelReward.SetActive(false);
    }
}
