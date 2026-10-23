using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipleySwine.ViewModels;

namespace ShipleySwine.ViewModels
{
    public class BredGiltsIndexViewModel
    {
        public List<BredGuilt> Gilts { get; set; }
        public Dictionary<string, decimal> BoarLinks { get; set; } = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        public decimal? FindBoarId(string name)
        {
            decimal id;
            return !string.IsNullOrWhiteSpace(name) && BoarLinks.TryGetValue(name, out id)
                ? (decimal?)id
                : null;
        }
    }
}
