using UniRxEventAggregator.Events;
using UniRxEventAggregator.Examples.BehaviourSubjects;
using UniRxEventAggregator.Examples.Events;
using UniRx;

namespace UniRxEventAggregator.Examples.Components
{
    public class Health : PubSubMonoBehaviour
    {
        // When you subscribe to a BehaviourSubject the latest or default value is returned.
        public BehaviorSubject<CurrentHealth> CurrentHealth { get; private set; }

        private void Awake()
        {
            // Set the default value of the BehaviourSubject.
            this.CurrentHealth = new BehaviorSubject<CurrentHealth>(new CurrentHealth(100));

            // Register the BehaviourSubject so you can subscribe to it.
            this.Register(this.CurrentHealth);

            // Subscribes to HealthChange event and calls SetHealth() in response.
            this.Subscribe<HealthChange>(this.SetHealth);            
        }

        private void SetHealth(HealthChange healthChange)
        {
            // OnNext tells subscribers that the value has changed.
            this.CurrentHealth.OnNext(new CurrentHealth(this.CurrentHealth.Value.Health + healthChange.Amount));
        }
    }
}
