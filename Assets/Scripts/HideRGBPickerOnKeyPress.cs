public class HideRGBPickerOnKeyPress : ActOnKeyPress
{
    protected override void Act()
    {
        RGBColorPicker._Instance.HideRGBPicker();
    }
}
