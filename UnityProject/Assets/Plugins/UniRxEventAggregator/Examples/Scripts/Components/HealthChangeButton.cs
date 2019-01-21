using UniRxEventAggregator.Events;
using UniRxEventAggregator.Examples.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UniRxEventAggregator.Examples.Components
{
    public class HealthChangeButton : PubSubMonoBehaviour
    {
        [SerializeField]
        private int amount = 1;

        private Button Button { get; set; }

        private void Awake()
        {
            this.Button = this.GetComponent<Button>();

            if (this.Button != null)
            {
                this.Button.onClick.AddListener(this.HealthChange);
            }
        }

        private void HealthChange()
        {
            // Publishes the HealthChange event which notifies subscribers.
            PubSub.Publish(new HealthChange(amount));
        }
    }
}
