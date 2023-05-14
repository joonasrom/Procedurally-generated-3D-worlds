using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject pauseMenu;

void Update(){
        if (Input.GetKeyDown(KeyCode.Escape))
if (Paused){
    Resume();
} else {
    Pause();
}
    }

void Resume(){
pauseMenu.SetActive(false);
Time.timeScale = 1f;
Paused = false;
Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause(){
pauseMenu.SetActive(true);
Time.timeScale = 0f;
Paused = true;
Cursor.lockState = CursorLockMode.Confined;
Cursor.visible = true;
    }

    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void Quit(){
        Application.Quit();
    }
}
