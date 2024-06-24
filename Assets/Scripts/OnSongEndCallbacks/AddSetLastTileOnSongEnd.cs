using UnityEngine;

public class AddSetLastTileOnSongEnd : AddToOnSongEnd
{
    [SerializeField] private ProceduralLevelTileSpawner spawner;
    [SerializeField] private GameObject levelTile;
    [SerializeField] private bool value;

    public override void CallOnSongEnd()
    {
        spawner.SetLastTile(levelTile);
    }
}
