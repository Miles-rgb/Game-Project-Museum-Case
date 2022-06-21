using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MobileMenu : MonoBehaviour
{
    public void Continue()
    {
        
    }

    public void NewGame()
    {
        Loader.Load(Loader.Scene.Level_00);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
