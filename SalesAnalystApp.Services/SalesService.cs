using SalesAnalystApp.Data.Interfaces;
using SalesAnalystApp.Domain.Models;
using SalesAnalystApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesAnalystApp.Services
{
    public class SalesService(ISalesRepository repository) : ISalesService
    {
        private readonly ISalesRepository _repository = repository;

        public async Task<(IEnumerable<SalesRecord> Records, int TotalCount)> GetPagedSalesAsync(
            int pageNumber, int pageSize, string? country = null, string? segment = null, string? sortBy = null, bool descending = false)
        {
            var (records, total) = await _repository.GetSalesAsync(1, int.MaxValue); // get all and filter manually

            if (!string.IsNullOrWhiteSpace(country))
                records = records.Where(r => r.Country.Equals(country, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(segment))
                records = records.Where(r => r.Segment.Equals(segment, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                records = sortBy switch
                {
                    "Country" => descending ? records.OrderByDescending(r => r.Country) : records.OrderBy(r => r.Country),
                    "Segment" => descending ? records.OrderByDescending(r => r.Segment) : records.OrderBy(r => r.Segment),
                    "UnitsSold" => descending ? records.OrderByDescending(r => r.UnitsSold) : records.OrderBy(r => r.UnitsSold),
                    "SalePrice" => descending ? records.OrderByDescending(r => r.SalePrice) : records.OrderBy(r => r.SalePrice),
                    _ => records
                };
            }

            var totalFiltered = records.Count();
            var page = records.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return (page, totalFiltered);
        }
    }
}
