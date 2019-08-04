using SpaceCommander;
using UnityEngine;

public class TestShield : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform entityTransform; // Hack til 2019.3
    private IPlayerEntity entity;
    public IPlayerEntity GetOwningEntity()
    {
        return entity;
    }

    public void SetOwningEntity(IPlayerEntity playerEntity)
    {
        entity = playerEntity;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
}
