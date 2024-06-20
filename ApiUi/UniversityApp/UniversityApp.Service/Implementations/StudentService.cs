using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityApi.Data;
using UniversityApp.Core.Entites;
using UniversityApp.Data.Repositories;
using UniversityApp.Data.Repositories.Interfaces;
using UniversityApp.Service.Dtos;
using UniversityApp.Service.Dtos.StudentDtos;
using UniversityApp.Service.Exceptions;
using UniversityApp.Service.Extentions;
using UniversityApp.Service.Interfaces;

namespace UniversityApp.Service.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly UniversityDbContext _context;
        private readonly IGroupRepository _groupRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(UniversityDbContext context,IGroupRepository groupRepository, IStudentRepository studentRepository,IMapper mapper)
        {
            _context = context;
            _groupRepository = groupRepository;
            _studentRepository = studentRepository;
            _mapper = mapper;
        }
        public int Create(StudentCreateDto createDto)
        {
            Group group = _groupRepository.Get(x=>x.Id == createDto.GroupId && !x.IsDeleted,"Students");

            if (group == null)  
                throw new RestException(StatusCodes.Status404NotFound, "GroupId", "Group not found by given GroupId");

            if (group.Limit <= group.Students.Count)
                throw new RestException(StatusCodes.Status400BadRequest, "Group is full");

            if (_context.Students.Any(x => x.Email.ToUpper() == createDto.Email.ToUpper() && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Student already exists by given Email");


            Student entity = new Student
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                BirthDate = createDto.BirthDate,
                GroupId = createDto.GroupId,
                FileName = createDto.File.Save("uploads/students")
            };

            _context.Students.Add(entity);
            _context.SaveChanges();

            return entity.Id;
        }

        public StudentGetDto GetById(int id)
        {
            Student entity = _studentRepository.Get(x => x.Id == id && !x.IsDeleted);

            if (entity == null) throw new RestException(StatusCodes.Status404NotFound, "Student not found by given id");

            return _mapper.Map<StudentGetDto>(entity);

        }
        public void Update(int id, StudentUpdateDto updateDto)
        {
            Student entity = _studentRepository.Get(x => x.Id == id && !x.IsDeleted, "Group");

            if (entity == null)
                throw new RestException(StatusCodes.Status404NotFound, "Student not found");
            if (entity.Email != updateDto.Email && _studentRepository.Exists(x => x.Email == updateDto.Email && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Email already taken");
            if (!_groupRepository.Exists(x => x.Id == updateDto.GroupId && !x.IsDeleted))
                throw new RestException(StatusCodes.Status400BadRequest, "GroupId", "Invalid Group ID");

            entity.FullName = updateDto.FullName;
            entity.Email = updateDto.Email;
            entity.BirthDate = updateDto.BirthDate;
            entity.GroupId = updateDto.GroupId;
            entity.ModifiedAt = DateTime.Now;

            if (updateDto.File != null)
            {

                if (!string.IsNullOrEmpty(entity.FileName))
                {
                    var oldFilePath = Path.Combine("wwwroot", entity.FileName);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                entity.FileName = updateDto.File.Save("uploads/students");
            }

            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Student entity = _studentRepository.Get(x => x.Id == id && !x.IsDeleted);
            if (entity == null) 
                throw new RestException(StatusCodes.Status404NotFound, "Student not found !");
            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.Now;
            _studentRepository.Save();
        }


        public PaginatedList<StudentGetDto> GetAllPaginated(int page = 1, int size = 10)
        {
            var query = _studentRepository.GetAll(x => !x.IsDeleted);

            PaginatedList<Student> students = PaginatedList<Student>.Create(query, page, size);

            return new PaginatedList<StudentGetDto>(_mapper.Map<List<StudentGetDto>>(students.Items), students.TotalPages, students.PageIndex, students.PageSize);
        }

        public List<StudentGetDto> GetAll(string? search = null)
        {
            var students = _studentRepository.GetAll(x => search == null || x.FullName.Contains(search)).ToList();
            return _mapper.Map<List<StudentGetDto>>(students);
        }
    }
}
