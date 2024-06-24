using UnityEngine;

public class AddGameEventToOnSongEnd : AddToOnSongEnd
{
    [SerializeField] private GameEvent gameEvent;
    public override void CallOnSongEnd()
    {
        gameEvent.Activate();
    }
}