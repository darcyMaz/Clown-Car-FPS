using UnityEngine;
using System.Collections;
using System;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    public event Action OnSceneChange;
    private Queue SceneBuffer;
    private string PreviousSceneName = "Start Menu";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        OnSceneChange += ChangeScene;
        SceneBuffer = new Queue();

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // If at least one scene has been queued into the buffer, invoke a scene change.
        if (SceneBuffer.Count != 0) OnSceneChange.Invoke();
    }

    public void BufferSceneChange(string scene)
    {
        SceneBuffer.Enqueue(scene);
    }

    private void ChangeScene()
    {
        // Change the scene from here.
        // Choose the first position. In the future, may have more precise functionality.
        try 
        {
            string nextScene = (string)SceneBuffer.Dequeue();
            PreviousSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }
        catch (InvalidCastException ive)
        {
            Debug.Log("A scene change was attempted but the Queue holding scene strings was accidently sent something that was not a string.");
            Debug.Log(ive.Message);
        }
        catch (Exception e)
        {
            Debug.Log("There was an attempt to change scene but there was an error in the SceneManager.");
            Debug.Log(e.Message);
        }

        SceneBuffer.Clear();
    }

    public string GetPreviousSceneName()
    {
        return PreviousSceneName;
    }
}
