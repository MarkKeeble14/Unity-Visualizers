using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "TimerReadFrom", menuName = "UI/TimerReadFrom", order = 1)]
public class TimerReadFrom : NumReadFrom
{
    [SerializeField] private float startingV;

    public float StartingValue
    {
        get { return startingV; }
    }

    public new float Value
    {
        get { return currentV; }
        set { 
            currentV = value;
            startingV = value;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }


    public void Subtract(float amnt)
    {
        currentV -= amnt;
    }

    public new void Reset()
    {
        currentV = startingV;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        currentV = baseV;
        startingV = baseV;
    }
}