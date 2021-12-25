using UnityEngine;using Weapon;

public class NonDestroyableObstacle : MonoBehaviour, IHealth
{
    public float Health => 1;
    
    // this object has no health and cannot be destroyed
    public void AdjustHealth(float value) { }
    public void Die() { }
}