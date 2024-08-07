using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SettingType
{
    CUSTOM,
    INTENSITY_DEFAULT_VALUE,
    INTENSITY_MULTIPLIER,
    RANGE_DEFAULT_VALUE,
    RANGE_MULTIPLIER,
    ATTACHED_TO_BAND,
    EMISSION_INTENSITY,
    SCALE,
    ENABLE_NORMALIZATION,
    ENABLE_SMOOTHING,
    SMOOTHING_SCALE,
    SAMPLING_LOUDNESS_MULTIPLIER,
    SMOOTHING_SHIFT,
    MIN_AUDIO_INPUT,
    MAX_AUDIO_INPUT,
    MIN_NORMALIZED_OUTPUT,
    MAX_NORMALIZED_OUTPUT,
    SIGNAL_MULTIPLIER,
    DEFAULT_VALUE,
    ADJUST_SPEED,
    SPACING,
    VISUALIZER_SEGMENT_TYPE,
    SEGMENT_WIDTH,
    RADIAL_DISTANCE,
    MINIMUM_EMISSION,
    MAXIMUM_EMISSION,
    MIN_NORMALIZED_AUDIO_SAMPLE_VALUE,
    MAX_NORMALIZED_AUDIO_SAMPLE_VALUE,
    CAMERA_MOVE_SPEED,
    CAMERA_ROTATE_SPEED
}


public class SetToolTipText : MonoBehaviour
{
    [SerializeField] private SpawnToolTip spawnToolTip;
    [SerializeField] private string defaultValue;
    [SerializeField] private SettingType toolTipContentType;

    // Start is called before the first frame update
    void Start()
    {
        SetToolTipToText(toolTipContentType);
    }

    public void SetToolTipToText(SettingType type)
    {
        toolTipContentType = type;
        spawnToolTip.SetToolTipText(GetToolTipContent(type));
    }

    private string GetToolTipContent(SettingType key)
    {
        switch (toolTipContentType)
        {
            case SettingType.CUSTOM:
                return defaultValue;
            case SettingType.ATTACHED_TO_BAND:
                return "The band the element responds to";
            case SettingType.INTENSITY_MULTIPLIER:
                return "Controls the brightness of the light when it recieves a strong signal";
            case SettingType.INTENSITY_DEFAULT_VALUE:
                return "Controls the minimum brightness of the light";
            case SettingType.EMISSION_INTENSITY:
                return "Controls the emission intensity of the element";
            case SettingType.RANGE_MULTIPLIER:
                return "Controls the reach of the light when it recieves a strong signal";
            case SettingType.RANGE_DEFAULT_VALUE:
                return "Controls the minimum reach of the light";
            case SettingType.SCALE:
                return "Scales the elements size";
            case SettingType.SMOOTHING_SHIFT:
                return "Shifts the smoothing equation";
            case SettingType.SAMPLING_LOUDNESS_MULTIPLIER:
                return "Multiplies the audio sample data";
            case SettingType.SMOOTHING_SCALE:
                return "Controls the scale of the smoothing equation";
            case SettingType.ENABLE_SMOOTHING:
                return "Enables the smoothing equation";
            case SettingType.ENABLE_NORMALIZATION:
                return "Enables normalization of audio sampling values";
            case SettingType.MIN_AUDIO_INPUT:
                return "The minimum expected value to be passed through the normalizer";
            case SettingType.MAX_AUDIO_INPUT:
                return "The maximum expected value to be passed through the normalizer";
            case SettingType.MIN_NORMALIZED_OUTPUT:
                return "The minimum value the normalizer can output";
            case SettingType.MAX_NORMALIZED_OUTPUT:
                return "The maximum value the normalizer can output";
            case SettingType.ADJUST_SPEED:
                return "The speed at which the element adjusts";
            case SettingType.DEFAULT_VALUE:
                return "The minimum value for the element";
            case SettingType.SIGNAL_MULTIPLIER:
                return "Multiplies the strength of the signal";
            case SettingType.SPACING:
                return "The space between each segment";
            case SettingType.SEGMENT_WIDTH:
                return "The width of each segment";
            case SettingType.RADIAL_DISTANCE:
                return "The diameter of the circle";
            case SettingType.VISUALIZER_SEGMENT_TYPE:
                return "The type of attachment for the Visualizer";
            case SettingType.MINIMUM_EMISSION:
                return "The minimum number of particles emitted";
            case SettingType.MAXIMUM_EMISSION:
                return "The maximum number of particles emitted";
            case SettingType.MIN_NORMALIZED_AUDIO_SAMPLE_VALUE:
                return "The minimum bound that audio data will be normalized to";
            case SettingType.MAX_NORMALIZED_AUDIO_SAMPLE_VALUE:
                return "The maximum bound that audio data will be normalized to";
            case SettingType.CAMERA_MOVE_SPEED:
                return "The speed the camera moves at";
            case SettingType.CAMERA_ROTATE_SPEED:
                return "The speed the camera rotates at";
            default:
                throw new UncaughtSwitchTypeException(typeof(SettingType), key.ToString());
        }
    }
}
