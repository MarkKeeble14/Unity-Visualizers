using UnityEngine;

public class AutomateLookAround : AutomateInputContinuous
{
    [SerializeField] private Direction dir;
    [SerializeField] private KeyCode lookLeftKey;
    [SerializeField] private KeyCode lookRightKey;
    [SerializeField] private KeyCode lookUpKey;
    [SerializeField] private KeyCode lookDownKey;

    protected override void Automate()
    {
        switch (dir)
        {
            case Direction.DOWN:
                automating.RecieveInput(lookDownKey);
                break;
            case Direction.LEFT:
                automating.RecieveInput(lookLeftKey);
                break;
            case Direction.RIGHT:
                automating.RecieveInput(lookRightKey);
                break;
            case Direction.UP:
                automating.RecieveInput(lookUpKey);
                break;
        }
    }
}

