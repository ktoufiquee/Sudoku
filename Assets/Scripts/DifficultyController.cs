using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyController : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame(int blanks)
    {
        PlayerPrefs.SetInt("Blanks", blanks);
        SceneManager.LoadScene("Game");
    }
}
