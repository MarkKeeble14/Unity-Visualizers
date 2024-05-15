using UnityEngine;

public class BarReadFrom : ReadFrom 
{
    [SerializeField] protected string subject;
    public string Subject
    {
        get { return subject; }
    }

    public override void Reset()
    {
        
    }
}
