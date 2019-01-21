namespace UniRxEventAggregator.Examples.Events
{
    public class HealthChange
    {
        public int Amount { get; private set; }

        public HealthChange(int amount)
        {
            this.Amount = amount;
        }
    }
}
