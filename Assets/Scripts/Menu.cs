using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    Text hardGemText;
    [SerializeField]
    Text softGemText;

    void Start()
    {
        UpdateGemsValue();
    }

    public void ClickOnJump()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void UpdateGemsValue()
    {
        hardGemText.text = PlayerGameData.instance.GetHardGemsCount().ToString();
        softGemText.text = PlayerGameData.instance.GetSoftGemsCount().ToString();
    }
}
