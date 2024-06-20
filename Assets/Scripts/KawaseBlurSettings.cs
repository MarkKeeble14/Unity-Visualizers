using UnityEngine;

[System.Serializable]
public struct KawaseBlurSettings
{
    public bool Enabled;
    public bool CopyToFrameBuffer;
    [Range(2, 15)] public int BlurPasses;
    [Range(1, 4)] public int Downsample;
}

