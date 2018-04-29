using System.Threading.Tasks;

namespace JWTAPI.Models.Repositories
{
    public interface IUnitOfWork
    {
         Task CompleteAsync();
    }
}