using ECommerce.Domain.Entities;

public sealed class HardDeleteTracker : IHardDeleteTracker
{
    private readonly HashSet<BaseEntity> _entities = [];

    public void Track(BaseEntity entity)=>  _entities.Add(entity);

    public bool IsTracked(BaseEntity entity)=> _entities.Contains(entity);

    public void Clear()=> _entities.Clear();
}