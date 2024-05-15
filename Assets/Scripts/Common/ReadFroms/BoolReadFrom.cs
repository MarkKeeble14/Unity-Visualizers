using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "BoolReadFrom", menuName = "UI/BoolReadFrom", order = 1)]
public class BoolReadFrom : ReadFrom
{
    [SerializeField] private bool baseB;
    [SerializeField] private bool currentB;
    public bool Bool
    {
        get { return currentB; }
        set { currentB = value; }
    }

    private void OnEnable()
    {
        Reset();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public override void Reset()
    {
        currentB = baseB;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Reset();
    }
}