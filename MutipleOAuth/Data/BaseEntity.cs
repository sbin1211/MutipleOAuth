using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MutipleOAuth.Data
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}