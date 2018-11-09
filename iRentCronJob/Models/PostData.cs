using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentCronJob.Models
{
    public class PostData
    {
        public int PropertyID { get; set; }
        public List<int> Units { get; set; } = null;
    }
}