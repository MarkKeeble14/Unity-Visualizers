[System.Serializable]
public class HasBoundary
{
    [UnityEngine.SerializeField] private bool has;
    [UnityEngine.SerializeField] private float bound;
    public float Bound
    {
        get { return bound; }
        set { bound = value; }
    }
    public bool Has
    {
        get { return has; }
        set { has = value; }
    }
}
