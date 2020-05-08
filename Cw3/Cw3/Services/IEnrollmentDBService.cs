using Cw3.DTOs;
using Cw3.DTOs.Requests;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IEnrollmentDBService
    {
        public EnrollmentStatus RegisterStudent(RegistrationStudentRequest registrationStudentRequest);
        public EnrollmentStatus PromoteStudents(PromoteStudentsRequest promoteStudentsRequest);
    }
}
