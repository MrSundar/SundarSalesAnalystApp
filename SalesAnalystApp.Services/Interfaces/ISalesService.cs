using SalesAnalystApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesAnalystApp.Services.Interfaces
{
    public interface ISalesService
    {
        Task<(IEnumerable<SalesRecord> Records, int TotalCount)> GetPagedSalesAsync(
            int pageNumber, int pageSize, string? country = null, string? segment = null, string? sortBy = null, bool descending = false);
    }
}
