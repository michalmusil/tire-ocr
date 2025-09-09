namespace TireOcr.Shared.Persistence;

public interface IEntityRepository
{
    public Task<int> SaveChangesAsync();
}