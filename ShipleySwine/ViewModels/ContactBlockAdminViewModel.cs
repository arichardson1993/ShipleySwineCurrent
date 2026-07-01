using ShipleySwine.Models;
using System.Collections.Generic;

namespace ShipleySwine.ViewModels
{
    public class ContactBlockAdminViewModel
    {
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Reason { get; set; }

        public IReadOnlyList<ContactBlockEntry> Blocks { get; set; }

        public string StatusMessage { get; set; }

        public string ErrorMessage { get; set; }
    }
}
