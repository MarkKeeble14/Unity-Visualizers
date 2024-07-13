using System.Collections.Generic;

public interface IRecieveVisualizerElementsInfo
{
    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info);
}

public interface IRecieveVisualizerSpecificElementsInfo
{
    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info);
}

public interface IRecieveVisualizerFloatValues
{
    public void RecieveVisualizerFloatValues(Dictionary<string, float> values);
}

public interface IRecieveVisualizerIntValues
{
    public void RecieveVisualizerIntValues(Dictionary<string, int> values);
}

public interface IRecieveVisualizerBoolValues
{
    public void RecieveVisualizerBoolValues(Dictionary<string, bool> values);
}