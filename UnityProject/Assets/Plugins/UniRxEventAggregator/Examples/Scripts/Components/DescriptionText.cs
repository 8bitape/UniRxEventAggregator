using UniRxEventAggregator.Events;
using UniRxEventAggregator.Examples.Events;
using UniRx;
using UnityEngine.UI;

namespace UniRxEventAggregator.Examples.Components
{
    public class DescriptionText : PubSubMonoBehaviour
    {
        private Text Text { get; set; }

        private void Awake()
        {
            this.Text = this.GetComponent<Text>();

            if (this.Text != null)
            {
                // Subscribes to HealthChange events where the amount is greater than zero and calls SetText() in response.
                PubSub.GetEvent<HealthChange>().Where(e => e.Amount > 0).Subscribe(e => this.SetText("Health increased"));

                // Subscribes to HealthChange events where the amount is less than zero and calls SetText() in response.
                PubSub.GetEvent<HealthChange>().Where(e => e.Amount < 0).Subscribe(e => this.SetText("Health decreased"));
            }
        }

        private void SetText(string description)
        {
            this.Text.text = description;
        }
    }
}
