using EventAggregator.Examples.Events;
using UniRx;
using UnityEngine.UI;

namespace EventAggregator.Examples.Components
{
    public class DescriptionText : PubSubMonoBehaviour
    {
        private Text Text { get; set; }

        private void Awake()
        {
            this.Text = this.GetComponent<Text>();

            if (this.Text != null)
            {
                PubSub.GetEvent<HealthChanged>().Where(e => e.Amount > 0).Subscribe(e => this.HealthIncreased());
                PubSub.GetEvent<HealthChanged>().Where(e => e.Amount < 0).Subscribe(e => this.HealthDecreased());
            }
        }

        private void HealthIncreased()
        {
            this.Text.text = "Health increased!";
        }

        private void HealthDecreased()
        {
            this.Text.text = "Health decreased!";
        }
    }
}
