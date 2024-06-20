using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApp.Core.Entites;
using UniversityApp.Service.Dtos;
using UniversityApp.Service.Dtos.StudentDtos;
using UniversityApp.Service.Interfaces;

namespace UniversityApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly UniversityDbContext _context;
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }


        [HttpGet("")]
        public ActionResult<PaginatedList<StudentGetDto>> GetAll(int page = 1, int size = 10)
        {
            return Ok(_studentService.GetAllPaginated(page, size));
        }

        [HttpPost("")]
        public ActionResult Create([FromForm] StudentCreateDto createDto)
        {
            return StatusCode(201, new { Id = _studentService.Create(createDto) });
        }

        [HttpGet("{id}")]
        public ActionResult GetById(int id)
        {
            return Ok(_studentService.GetById(id));
        }

        [HttpPut("{id}")]
        public ActionResult Update( int id, [FromForm] StudentUpdateDto updateDto)
        {
            _studentService.Update(id, updateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _studentService.Delete(id);
            return NoContent();
        }
    }
}
