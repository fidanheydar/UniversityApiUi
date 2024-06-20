using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Service.Dtos.StudentDtos
{
    public class StudenGetAllDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public int GroupId { get; set; }
        public DateTime BirthDate { get; set; }
        public string ImageUrl { get; set; }
    }
}
