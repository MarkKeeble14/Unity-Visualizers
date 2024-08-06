public class ShowRGBPickerOnKeyRelease : ActOnKeyRelease
{
    protected override void Act()
    {
        RGBColorPicker._Instance.ShowRGBPicker();
    }
}
