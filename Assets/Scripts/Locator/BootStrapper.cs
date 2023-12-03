using ServiceLocator;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initiailze()
    {
        Locator.Initialize();
        Application.targetFrameRate = 60;
        Locator.Instance.Register(new PoolService());
        SceneManager.LoadSceneAsync(1);
    }
}