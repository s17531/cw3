﻿using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SqlServerDbDal : IStudentsDal
    {
        private string SqlConn = "Data Source=db-mssql;Initial Catalog=s17531;Integrated Security=True";

        public IEnumerable<Student> GetStudents()
        {
            var output = new List<Student>();
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT * FROM Student";

                    client.Open();
                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        output.Add(new Student
                        {
                            IndexNumber = dataReader["IndexNumber"].ToString(),
                            FirstName = dataReader["FirstName"].ToString(),
                            LastName = dataReader["LastName"].ToString(),
                        });
                    }
                }
            }

            return output;
        }


        public IEnumerable<Enrollment> GetStudentEnrollment(string studentIndexNumber)
        {
            var output = new List<Enrollment>();
            using (var client = new SqlConnection(SqlConn))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = $"SELECT IdEnrollment, Semester, Studies.Name, StartDate FROM Enrollment INNER JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy WHERE IdEnrollment = (SELECT IdEnrollment FROM Student WHERE Student.IndexNumber =@studentIndexNumber)";
                    command.Parameters.AddWithValue("studentIndexNumber", studentIndexNumber);

                    client.Open();
                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        output.Add(new Enrollment
                        {
                            IdEnrollment = int.Parse(dataReader["IdEnrollment"].ToString()),
                            Name = dataReader["Name"].ToString(),
                            Semester = int.Parse(dataReader["Semester"].ToString()),
                            StartDate = DateTime.Parse(dataReader["StartDate"].ToString())
                        });
                    }
                }
            }
            return output;
        }
    }
}
