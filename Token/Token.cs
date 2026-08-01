using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Token
{
    public class Token
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public string TokenNumber { get; set; } = "";
        public string Status { get; set; } = "";
    }
}