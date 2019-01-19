using UniRxEventAggregator.Events;
using UniRxEventAggregator.Examples.BehaviourSubjects;
using UniRxEventAggregator.Examples.Events;
using UniRx;

namespace UniRxEventAggregator.Examples.Components
{
    public class Health : PubSubMonoBehaviour
    {
        public BehaviorSubject<CurrentHealth> CurrentHealth { get; private set; }

        private void Awake()
        {
            this.CurrentHealth = new BehaviorSubject<CurrentHealth>(new CurrentHealth(100));

            this.Register(this.CurrentHealth);

            this.Subscribe<HealthChanged>(this.HealthChanged);
        }

        private void HealthChanged(HealthChanged healthChanged)
        {
            this.CurrentHealth.OnNext(new CurrentHealth(this.CurrentHealth.Value.Health + healthChanged.Amount));
        }
    }
}
