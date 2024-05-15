using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "BoolLockedNumReadFrom", menuName = "UI/BoolLockedNumReadFrom", order = 1)]
public class BoolLockedNumReadFrom : ReadFrom
{
    [SerializeField] private bool baseB;
    [SerializeField] private bool currentB;
    public bool Bool
    {
        get { return currentB; }
        set { currentB = value; }
    }
    [SerializeField] private float baseV;
    [SerializeField] private float currentV;
    public float Value
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
        currentB = baseB;
        currentV = baseV;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Reset();
    }
}
