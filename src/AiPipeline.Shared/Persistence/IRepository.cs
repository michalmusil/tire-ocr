namespace TireOcr.Shared.Persistence;

public interface IRepository
{
    public Task<int> SaveChangesAsync();
}