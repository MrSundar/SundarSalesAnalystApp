using SalesAnalystApp.Domain.Models;
using SalesAnalystApp.Data.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System;
using System.Globalization;

namespace SalesAnalystApp.Data.Implementations
{
    public class SalesRepository : ISalesRepository
    {
        private readonly string _dataSourcePath;
        private List<SalesRecord> _cache;

        public SalesRepository()
        {
            var basePath = AppContext.BaseDirectory;
            _dataSourcePath = Path.Combine(basePath, "DataSource", "Data.csv");
        }

        private string Clean(string input)
        {
            return new string(input.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
        }

        private void LoadData()
        {
            if (_cache != null) return;

            if (!File.Exists(_dataSourcePath))
            {
                throw new FileNotFoundException("Datasource artefact not found", _dataSourcePath);
            }

            _cache = File.ReadAllLines(_dataSourcePath)
                .Skip(1)
                .Select(line =>
                {
                    SalesRecord? result = null;

                    if (string.IsNullOrWhiteSpace(line)) return result;

                    var parts = line.Split(',');
                    if (parts.Length < 8) return result;

                    var segment = parts[0].Trim();
                    var country = parts[1].Trim();
                    var product = parts[2].Trim();
                    var discountBand = parts[3].Trim();

                    var unitsRaw = Clean(parts[4].Trim());
                    var manPriceRaw = Clean(parts[5].Trim().Replace("£", "").Replace("$", ""));
                    var salePriceRaw = Clean(parts[6].Trim().Replace("£", "").Replace("$", ""));
                    var dateRaw = parts[7].Trim();

                    result = new SalesRecord
                    {
                        Segment = segment,
                        Country = country,
                        Product = product,
                        DiscountBand = discountBand,
                        UnitsSold = (int)(decimal.TryParse(unitsRaw, out var units) ? units : 0),
                        ManufacturingPrice = decimal.TryParse(manPriceRaw, out var manPrice) ? manPrice : 0,
                        SalePrice = decimal.TryParse(salePriceRaw, out var salePrice) ? salePrice : 0,
                        Date = DateTime.TryParseExact(dateRaw, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : DateTime.MinValue
                    };

                    return result;
                })
                .Where(r => r is not null)
                .Select(r => r!)
                .ToList();
        }

        public Task<(IEnumerable<SalesRecord> Records, int TotalCount)> GetSalesAsync(int pageNumber, int pageSize)
        {
            LoadData();
            var total = _cache.Count;
            var page = _cache.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return Task.FromResult((page, total));
        }
    }
}
