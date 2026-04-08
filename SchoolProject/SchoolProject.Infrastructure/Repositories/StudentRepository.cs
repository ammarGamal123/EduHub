using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Infrastructure.InfrastructureBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Infrastructure.Repositories
{
    public class StudentRepository :
                 GenericRepositoryAsync<Student>, IStudentRepository
    {
        private readonly DbSet<Student> _students;

        public StudentRepository(ApplicationDbContext _context) : base(_context)
        {
            _students = _context.Set<Student>();
        }

        public async Task<List<Student>> GetStudentsAsync()
        {
            return await _students.Include(std => std.Department).ToListAsync();
        }
    }
}
