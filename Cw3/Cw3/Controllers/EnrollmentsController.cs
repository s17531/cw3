using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs;
using Cw3.DTOs.Requests;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {

        private readonly IEnrollmentDBService _enrollmentDBService;

        public EnrollmentsController(IEnrollmentDBService enrollmentDBService)
        {
            _enrollmentDBService = enrollmentDBService;
        }

        [HttpPost]
        public IActionResult RegisterNewStudent(RegistrationStudentRequest registrationStudent)
        {
            EnrollmentStatus enrollmentStatus = _enrollmentDBService.RegisterStudent(registrationStudent);

            if(enrollmentStatus.Status == 400)
            {
                return BadRequest(enrollmentStatus.Message);
            }

            return Ok(enrollmentStatus.enrollment);
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            EnrollmentStatus enrollmentStatus = _enrollmentDBService.PromoteStudents(promoteStudentsRequest);

            if (enrollmentStatus.Status == 400)
            {
                return BadRequest(enrollmentStatus.Message);
            }

            return Ok(enrollmentStatus.enrollment);
        }

    }
}