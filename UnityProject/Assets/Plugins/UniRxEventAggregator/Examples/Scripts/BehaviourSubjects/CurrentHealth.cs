namespace EventAggregator.Examples.BehaviourSubjects
{
    public class CurrentHealth
    {
        public int Health { get; private set; }

        public CurrentHealth(int health)
        {
            this.Health = health;
        }
    }
}
