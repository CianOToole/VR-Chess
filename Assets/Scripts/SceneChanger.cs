using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    
    public void playGame()
    {
        SceneManager.LoadScene(1);
    }

    public void playGameTwo()
    {
        SceneManager.LoadScene(2);
    }

    public void playGameThree()
    {
        SceneManager.LoadScene(4);
    }

    public void setSceneOne()
    {
        
        SceneManager.LoadScene(0);
    }

    public void setSceneTwo()
    {
        
        SceneManager.LoadScene(5);
    }

    public void setSceneThree()
    {
        
        SceneManager.LoadScene(3);
    }

    public void quit()
    {
        Application.Quit();
    }

}
