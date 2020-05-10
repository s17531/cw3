using Cw3.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SqlServerStudentDbService : IStudentDbService
    {
        public Student GetStudent(string index)
        {
            if (index == "s17531")
                return new Student { FirstName = "Jan", LastName = "Kowal", IndexNumber = "s17531" };
        
            return null;
        }

        public IEnumerable<Student> GetStudents()
        {
            throw new NotImplementedException();
        }
    }

    }
