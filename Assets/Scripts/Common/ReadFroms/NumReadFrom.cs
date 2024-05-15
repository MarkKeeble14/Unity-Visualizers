using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NumReadFrom", menuName = "UI/NumReadFrom", order = 1)]
public class NumReadFrom : ReadFrom
{
    [SerializeField] protected float baseV;

    [SerializeField] protected float currentV;
    public float Value
    {
        get { return currentV; }
        set { currentV = value; }
    }
    public float BaseValue
    {
        get { return baseV; }
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
