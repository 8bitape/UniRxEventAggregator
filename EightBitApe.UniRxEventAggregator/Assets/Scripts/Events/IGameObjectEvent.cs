using UnityEngine;

namespace UniRxEventAggregator.Events
{
    public interface IGameObjectEvent
    {
        GameObject GameObject { get; }
    }
}
