using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnStart : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene(0);
    }
}
