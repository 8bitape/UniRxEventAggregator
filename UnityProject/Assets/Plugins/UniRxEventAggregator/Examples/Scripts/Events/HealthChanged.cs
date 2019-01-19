namespace EventAggregator.Examples.Events
{
    public class HealthChanged
    {
        public int Amount { get; private set; }

        public HealthChanged(int amount)
        {
            this.Amount = amount;
        }
    }
}
