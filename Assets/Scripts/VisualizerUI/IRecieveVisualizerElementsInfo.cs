using System.Collections.Generic;

public interface IRecieveVisualizerElementsInfo
{
    public void RecieveVisualizerElementsInfo(Dictionary<VisualizerElementLabel, VisualizerElementsSettings> info);
}

public interface IRecieveVisualizerSpecificElementsInfo
{
    public void RecieveVisualizerSpecificElementsInfo(Dictionary<string, VisualizerElementsSettings> info);
}
