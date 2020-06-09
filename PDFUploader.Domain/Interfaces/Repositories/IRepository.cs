using PDFUploader.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace PDFUploader.Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}