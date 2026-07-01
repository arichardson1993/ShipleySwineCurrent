using System;

namespace ShipleySwine.Models
{
    public class ContactBlockEntry
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Reason { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}
