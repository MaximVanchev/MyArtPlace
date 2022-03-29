using MyArtPlace.Data;
using MyArtPlace.Infrastructure.Data.Common;

namespace MyArtPlace.Infrastructure.Data.Repositories
{
    public class ApplicationDbRepository : Repository, IApplicationDbRepository
    {
        public ApplicationDbRepository(MyArtPlaceContext context)
        {
            this.Context = context;
        }
    }
}