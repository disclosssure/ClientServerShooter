namespace Weapon
{
    public interface IHealth
    {
        float Health { get; }
        void AdjustHealth(float value);
        void Die();
    }
}