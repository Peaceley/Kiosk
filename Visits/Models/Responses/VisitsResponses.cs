using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kiosk.Visits.Models.Responses
{
    public class VisitsResponses
    {
       public int Id { get; set; } 
       public int PatientId { get; set; }
       public string? Status { get; set; }
       public string? Token{get; set;}
       public DateTime CreatedAt {get; set;}

    }
}