using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShipleySwine.ViewModels;

namespace ShipleySwine.ViewModels
{
    public class BredGiltsIndexViewModel
    {
        public int berkCount { get; set; }
        public int durocCount { get; set; }
        public int exoticCount { get; set; }
        public int hampCount { get; set; }
        public int yorkCount { get; set; }
        public int otherCount { get; set; }

        public List<BredGuilt> berkshireGilts { get; set; }
        public List<BredGuilt> durocGilts { get; set; }
        public List<BredGuilt> exoticGilts { get; set; }
        public List<BredGuilt> hampshireGilts { get; set; }
        public List<BredGuilt> yorkshireGilts { get; set; }
        public List<BredGuilt> otherGilts { get; set; }

    }
}