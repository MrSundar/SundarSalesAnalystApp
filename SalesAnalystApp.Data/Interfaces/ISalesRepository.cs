using SalesAnalystApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesAnalystApp.Data.Interfaces
{
    public interface ISalesRepository
    {
        Task<(IEnumerable<SalesRecord> Records, int TotalCount)> GetSalesAsync(int pageNumber, int pageSize);
    }
}
