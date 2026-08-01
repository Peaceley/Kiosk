using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Visits.Models.Requests
{
    public class VisitFullRequest
    {
       public int Id { get; set; } 
       public int PatientId { get; set; }
       public string? Status { get; set; }
       public DateTime CreatedAt {get; set;}

    }
}