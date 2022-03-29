using MyArtPlace.Core.Contracts;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApplicationDbRepository repo;

        public CategoryService(IApplicationDbRepository _repo)
        {
            repo = _repo;
        }

        public IQueryable<Category> AllCategories()
        {
            return repo.AllReadonly<Category>().AsQueryable();
        }
    }
}
