namespace JWTAPI.Core.Repositories
{
    public interface IUnitOfWork
    {
         Task CompleteAsync();
    }
}