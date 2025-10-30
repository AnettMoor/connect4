namespace BLL;

public abstract class BaseEntity
{
    // creating code side id's
    public Guid Id { get; set; } = Guid.NewGuid();
}
