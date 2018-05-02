using System.Threading.Tasks;

namespace JWTAPI.Core.Repositories
{
    public interface IUnitOfWork
    {
         Task CompleteAsync();
    }
}