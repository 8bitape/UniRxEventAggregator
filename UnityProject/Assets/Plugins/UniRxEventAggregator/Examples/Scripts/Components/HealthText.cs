using UniRxEventAggregator.Events;
using UniRxEventAggregator.Examples.BehaviourSubjects;
using UnityEngine.UI;

namespace UniRxEventAggregator.Examples.Components
{
    public class HealthText : PubSubMonoBehaviour
    {
        private Text Text { get; set; }

        private void Awake()
        {
            this.Text = this.GetComponent<Text>();

            if (this.Text != null)
            {
                // Subscribes to CurrentHealth BehaviourSubject and calls SetHealthText() in response to changes.
                this.Subscribe<CurrentHealth>(this.SetHealthText);
            }
        }

        private void SetHealthText(CurrentHealth currentHealth)
        {
            this.Text.text = string.Format("Health: {0}", currentHealth.Health);
        }
    }
}
