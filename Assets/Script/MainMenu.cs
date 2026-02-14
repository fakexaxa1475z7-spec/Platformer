using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Name")]
    public string firstLevelName = "Level1";

    void ClearCheckpointData()
    {
        PlayerPrefs.DeleteKey("SavedLevel");
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointZ");
    }

    // ▶ ปุ่ม Start
    public void StartGame()
    {
        ClearCheckpointData();

        SceneManager.LoadScene(firstLevelName);
    }

    // 🔁 ปุ่ม Continue (ตัวอย่างแบบง่าย)
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            string level = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(level);
        }
    }

    // ❌ ปุ่ม Exit
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}
