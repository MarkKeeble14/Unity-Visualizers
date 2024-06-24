using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
