using ECommerce.Domain.Entities;

public interface IHardDeleteTracker
{
    void Track(BaseEntity entity);
    bool IsTracked(BaseEntity entity);
    void Clear();
}