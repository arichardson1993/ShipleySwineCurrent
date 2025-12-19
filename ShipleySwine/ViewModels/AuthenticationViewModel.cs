using ShipleySwine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShipleySwine.ViewModels
{
    public class AuthenticationViewModel
    {
        public string controller { get; set; }
        public string action { get; set; }
        public UserAuthentication user { get; set; }
    }
}