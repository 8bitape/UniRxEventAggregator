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
                this.Subscribe<CurrentHealth>(this.CurrentHealth);
            }
        }

        private void CurrentHealth(CurrentHealth currentHealth)
        {
            this.Text.text = string.Format("Health: {0}", currentHealth.Health);
        }
    }
}
