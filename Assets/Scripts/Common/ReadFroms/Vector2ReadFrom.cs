using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Vector2ReadFrom", menuName = "UI/Vector2ReadFrom", order = 1)]
public class Vector2ReadFrom : ReadFrom
{
    [SerializeField] private Vector2 baseV;

    [SerializeField] private Vector2 currentV;
    public Vector2 Value
    {
        get { return currentV; }
        set { currentV = value; }
    }

    private void OnEnable()
    {
        Reset();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public override void Reset()
    {
        currentV = baseV;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Reset();
    }
}
