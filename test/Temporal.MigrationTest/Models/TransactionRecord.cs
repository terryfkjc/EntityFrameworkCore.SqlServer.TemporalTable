using System;
using System.Collections.Generic;
using System.Text;

namespace Temporal.MigrationTest.Models
{
    class TransactionRecord
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        //public User Sender { get; set; }
    }
}
