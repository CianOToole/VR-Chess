using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    public static int loadInt = 1;
    public void playGame()
    {
        SceneManager.LoadScene(loadInt);
    }

    public void setSceneOne()
    {
        loadInt = 1;
        SceneManager.LoadScene(0);
    }

    public void setSceneTwo()
    {
        loadInt = 2;
        SceneManager.LoadScene(5);
    }

    public void setSceneThree()
    {
        loadInt = 4;
        SceneManager.LoadScene(3);
    }

}
