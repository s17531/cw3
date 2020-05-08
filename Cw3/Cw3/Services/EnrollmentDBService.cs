using Cw3.DTOs;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class EnrollmentDBService : IEnrollmentDBService
    {
        private string SqlConn = "Data Source=db-mssql;Initial Catalog=s17531;Integrated Security=True";
        public EnrollmentStatus RegisterStudent(RegistrationStudentRequest registrationStudentRequest)
        {
            EnrollmentStatus registerStudentStatus = new EnrollmentStatus();
            using (var client = new SqlConnection(SqlConn))
            using (var command = new SqlCommand())
            {

                command.Connection = client;
                client.Open();

                var transaction = client.BeginTransaction();
                command.Transaction = transaction;
                try
                {


                    command.CommandText = $"SELECT IdStudy FROM STUDIES WHERE Name=@name";
                    command.Parameters.AddWithValue("name", registrationStudentRequest.Studies);

                    var dataReader = command.ExecuteReader();

                    if (!dataReader.Read())
                    {
                        dataReader.Close();
                        transaction.Rollback();
                        registerStudentStatus.enrollment = null;
                        registerStudentStatus.Status = 400;
                        registerStudentStatus.Message = "Nie ma takich studiów";
                        return registerStudentStatus;
                    }

                    string IdStudy = dataReader["IdStudy"].ToString();

                    dataReader.Close();
                    command.CommandText = $"SELECT IdEnrollment, StartDate FROM Enrollment WHERE Enrollment.Semester = 1  AND Enrollment.IdStudy = @IdStudy AND Enrollment.StartDate = (SELECT MAX(StartDate) FROM Enrollment WHERE Enrollment.Semester = 1  AND Enrollment.IdStudy = @IdStudy);";
                    command.Parameters.AddWithValue("IdStudy", IdStudy);

                    dataReader = command.ExecuteReader();
                    DateTime StartDate = DateTime.Now;
                    int IdEnrollemnt = -1;

                    if (!dataReader.Read())
                    {
                        command.CommandText = $"INSERT INTO Enrollment VALUES(SELECT MAX(IdEnrollment) + 1 FROM Enrollment WHERE Enrollment.IdStudy = @IdStudy) + 1, 1, @IdStudy, @StartDate)";
                        command.Parameters.AddWithValue("IdStudy", IdStudy);
                        command.Parameters.AddWithValue("StartDate", DateTime.Now);
                        command.ExecuteReader();

                        dataReader.Close();
                        command.CommandText = $"SELECT IdEnrollment, StartDate FROM Enrollment WHERE Enrollment.Semester = 1  AND Enrollment.IdStudy = @IdStudy AND Enrollment.StartDate = (SELECT MAX(StartDate) FROM Enrollment WHERE Enrollment.Semester = 1  AND Enrollment.IdStudy = @IdStudy);";
                        command.Parameters.AddWithValue("IdStudy", IdStudy);

                        dataReader = command.ExecuteReader();
                        dataReader.Read();
                    }
                    else
                    {
                        StartDate = DateTime.Parse(dataReader["StartDate"].ToString());
                    }

                    IdEnrollemnt = int.Parse(dataReader["IdEnrollment"].ToString());

                    dataReader.Close();
                    command.CommandText = $"SELECT IndexNumber FROM Student WHERE IndexNumber = @IndexNumber";
                    command.Parameters.AddWithValue("IndexNumber", registrationStudentRequest.IndexNumber);
                    dataReader = command.ExecuteReader();

                    if (dataReader.Read())
                    {
                        dataReader.Close();
                        transaction.Rollback();
                        registerStudentStatus.enrollment = null;
                        registerStudentStatus.Status = 400;
                        registerStudentStatus.Message = "Student o tym numerze indeksu już zapisany";
                        return registerStudentStatus;
                    }
                    dataReader.Close();


                    command.CommandText = $"INSERT INTO Student VALUES(@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("IndexNumber", registrationStudentRequest.IndexNumber);
                    command.Parameters.AddWithValue("FirstName", registrationStudentRequest.FirstName);
                    command.Parameters.AddWithValue("LastName", registrationStudentRequest.LastName);
                    command.Parameters.AddWithValue("BirthDate", registrationStudentRequest.BirthDate);
                    command.Parameters.AddWithValue("IdEnrollment", IdEnrollemnt);
                    command.ExecuteNonQuery();
                    transaction.Commit();

                    EnrollmentResponse enrollment = new EnrollmentResponse()
                    {
                        IdEnrollment = IdEnrollemnt,
                        Semester = 1,
                        IdStudy = int.Parse(IdStudy),
                        StartDate = StartDate,
                    };

                    registerStudentStatus.enrollment = enrollment;
                    registerStudentStatus.Status = 201;
                    registerStudentStatus.Message = "Student added";

                }
                catch (SqlException exc)
                {
                    transaction.Rollback();
                    registerStudentStatus.enrollment = null;
                    registerStudentStatus.Status = 400;
                    registerStudentStatus.Message = "Nastąpił wyjątek";

                }

            }
            return registerStudentStatus;
        }


        public EnrollmentStatus PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            EnrollmentStatus registerStudentStatus = new EnrollmentStatus();
            using (var client = new SqlConnection(SqlConn))
            using (var command = new SqlCommand())
            {

                command.Connection = client;
                client.Open();

                try
                {
                    command.CommandText = $"SELECT IdEnrollment FROM Enrollment WHERE Enrollment.Semester = @semester AND IdStudy = (SELECT IdStudy FROM Studies WHERE Studies.Name = @name);";
                    command.Parameters.AddWithValue("semester", promoteStudentsRequest.Semester);
                    command.Parameters.AddWithValue("name", promoteStudentsRequest.Studies);

                    var dataReader = command.ExecuteReader();

                    if (!dataReader.Read())
                    {
                        registerStudentStatus.enrollment = null;
                        registerStudentStatus.Status = 400;
                        registerStudentStatus.Message = "Nie ma takiego semestru dla tych studiów";
                        return registerStudentStatus;
                    }

                    dataReader.Close();

                    command.CommandText = "PromoteStudents";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Studies", promoteStudentsRequest.Studies);
                    command.Parameters.AddWithValue("@Semester", promoteStudentsRequest.Semester);

                    int rowAffected = command.ExecuteNonQuery();

                    command.CommandText = $"SELECT * FROM Enrollment WHERE Enrollment.Semester = @semester AND IdStudy = (SELECT IdStudy FROM Studies WHERE Studies.Name = @name);";
                    command.CommandType = CommandType.Text;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@name", promoteStudentsRequest.Studies);
                    command.Parameters.AddWithValue("@semester", promoteStudentsRequest.Semester + 1);

                    dataReader = command.ExecuteReader();

                    if (!dataReader.Read())
                    {
                        registerStudentStatus.enrollment = null;
                        registerStudentStatus.Status = 400;
                        registerStudentStatus.Message = "Błąd bazy!";
                        return registerStudentStatus;
                    }

                    EnrollmentResponse enrollment = new EnrollmentResponse()
                    {
                        IdEnrollment = int.Parse((dataReader["IdEnrollment"]).ToString()),
                        Semester = promoteStudentsRequest.Semester + 1,
                        IdStudy = int.Parse((dataReader["IdStudy"]).ToString()),
                        StartDate = DateTime.Parse(dataReader["StartDate"].ToString()),
                    };


                    registerStudentStatus.enrollment = enrollment;
                    registerStudentStatus.Status = 201;
                    registerStudentStatus.Message = "Promowany";

                }
                catch (SqlException exc)
                {
                    registerStudentStatus.enrollment = null;
                    registerStudentStatus.Status = 400;
                    registerStudentStatus.Message = "Wyjątek bazy danych";

                }

                return registerStudentStatus;
            }
        }


        EnrollmentStatus IEnrollmentDBService.RegisterStudent(RegistrationStudentRequest registrationStudentRequest)
        {
            throw new NotImplementedException();
        }

        EnrollmentStatus IEnrollmentDBService.PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            throw new NotImplementedException();
        }

    }
}
