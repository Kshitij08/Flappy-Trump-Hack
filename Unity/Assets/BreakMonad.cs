using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BreakMonad : MonoBehaviour
{


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            OpenTwitterProfile();
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            SceneManager.LoadScene(1);
        }
    }

    public void OpenTwitterProfile()
    {
        //Debug.Log("Opened Twitter");
        string twitterUrl = $"https://x.com/KshitijGajapure";
        Application.OpenURL(twitterUrl);
    }

}
