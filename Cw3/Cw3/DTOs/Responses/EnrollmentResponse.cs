﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Responses
{
    public class EnrollmentResponse
    {
        public int IdEnrollment { get; set; }
        public int IdStudy { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
    }
}
