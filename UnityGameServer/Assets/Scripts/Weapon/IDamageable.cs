namespace Weapon
{
    public interface IDamaging
    {
        float Damage { get; }
        void DealDamage(IHealth health, float damage);
    }
}
