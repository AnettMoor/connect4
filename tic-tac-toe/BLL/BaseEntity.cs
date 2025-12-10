namespace BLL;

public abstract class BaseEntity
{
    // creating code side id's for repository
    public Guid Id { get; set; } = Guid.NewGuid();
}
