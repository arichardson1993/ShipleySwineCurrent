using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipleySwine.ViewModels
{
    public class HomePageViewModel
    {
        public int randomBoar;

        public HomePageViewModel(List<Boar> newBoars, int randomBoar)
        {
            NewBoars = newBoars;
            this.randomBoar = randomBoar;
        }

        public List<Boar> NewBoars { get; set; }
        public int randomNumber { get; set; } 
    }
}