using System.Collections.Generic;
using UnityEngine;

public class SignalDirector : MonoBehaviour
{
    [SerializeField]
    private List<SerializableKeyValuePair<SignalBroadcaster, List<AttachParameter>>> directions
        = new List<SerializableKeyValuePair<SignalBroadcaster, List<AttachParameter>>>();

    private void Update()
    {
        foreach (SerializableKeyValuePair<SignalBroadcaster, List<AttachParameter>> kvp in directions)
        {
            foreach (AttachParameter attachedParamter in kvp.Value)
            {
                attachedParamter.RecieveBroadcast(kvp.Key.GetBroadcastValue());
            }
        }
    }

    public void AddDirection(SignalBroadcaster broadcaster, List<AttachParameter> attachments)
    {
        directions.Add(new SerializableKeyValuePair<SignalBroadcaster, List<AttachParameter>>(broadcaster, attachments));
    }

    public void AddDirection(SignalBroadcaster broadcaster, AttachParameter attachment)
    {
        directions.Add(new SerializableKeyValuePair<SignalBroadcaster, List<AttachParameter>>(broadcaster, new List<AttachParameter>() { attachment }));
    }
}
