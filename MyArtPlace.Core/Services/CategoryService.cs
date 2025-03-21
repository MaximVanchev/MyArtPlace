﻿using MyArtPlace.Core.Models.Admin;
using Microsoft.EntityFrameworkCore;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyArtPlace.Core.Contracts;

namespace MyArtPlace.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApplicationDbRepository repo;

        public CategoryService(IApplicationDbRepository _repo)
        {
            repo = _repo;
        }

        public async Task<bool> AddCategory(AddCategoryViewModel model)
        {
            Category category = new Category()
            {
                Name = model.Name
            };

            try
            {
                await repo.AddAsync(category);
                await repo.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<CategoryListViewModel>> AllCategories()
        {
            return await repo.All<Category>()
            .Select(x => new CategoryListViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }

        public async Task<bool> DeleteCategoryById(Guid id)
        {
            try
            {
                await repo.DeleteAsync<Category>(id);
                await repo.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            return await repo.GetByIdAsync<Category>(id);
        }
    }
}
