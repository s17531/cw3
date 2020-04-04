using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17531;Integrated Security=True";
        private readonly IStudentsDal studentDB;

        public StudentsController(IStudentsDal studentDB)
        {
            this.studentDB = studentDB;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(studentDB.GetStudents());
        }

        [HttpGet("{IndexNumber}/enrollment")]
        public IActionResult GetStudentsEnrollments(string IndexNumber)
        {
            return Ok(studentDB.GetStudentEnrollment(IndexNumber));
        }
        /*  httpGet - pobierz
        *  httpPost - wyślij
        *  httpPut - zaktualizuj
        *  HttpPatch - zaktualizuj częściowo (załataj)
        *  httpDelete - usuń
        */

/*
        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString)) 
            using (SqlCommand com = new SqlCommand()) 
            {
                com.Connection = con;
                com.CommandText = "select * from student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while(dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    list.Add(st);


                }
            }
            
            return Ok(list);

        }
*/
        [HttpGet ("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
            }

                return Ok();
        }


        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Nowak");
            }
            else
            {
                return NotFound("Nie znaleziono studenta");
            }
        }

        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 2000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent([FromBody] Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 2000)}";
            return Ok(student);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok($"student o {id} - Usuwanie ukończone");
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok($"student o {id} - zaktualizowany");
        }

    }
}