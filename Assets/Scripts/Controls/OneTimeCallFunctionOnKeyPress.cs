public class OneTimeCallFunctionOnKeyPress : CallFunctionOnKeyPress
{
    private bool hasFired;
    protected override void Act()
    {
        if (hasFired) return;
        base.Act();
    }
}