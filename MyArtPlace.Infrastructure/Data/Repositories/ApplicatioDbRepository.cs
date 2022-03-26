using MyArtPlace.Data;
using MyArtPlace.Infrastructure.Data.Common;

namespace MyArtPlace.Infrastructure.Data.Repositories
{
    public class ApplicatioDbRepository : Repository, IApplicatioDbRepository
    {
        public ApplicatioDbRepository(MyArtPlaceContext context)
        {
            this.Context = context;
        }
    }
}