using Cw3.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs
{
    public class EnrollmentStatus
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public EnrollmentResponse enrollment { get; set; }
    }
}
