using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private string StartMenuName;

    public void StartGame()
    {
        SceneManager.Instance.BufferSceneChange(StartMenuName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
