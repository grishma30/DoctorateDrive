using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using DoctorateDrive.Repositories;
using System;
using System.Collections.Generic;

namespace DoctorateDrive.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<StudentDetail> GetAllStudents() => _repository.GetAll();

        public StudentDetail? GetStudent(int id) => _repository.GetById(id);

        public void CreateStudent(StudentDetailDto dto, int userId)
        {
            var student = new StudentDetail
            {
                UserId = userId,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Dob = dto.Dob,
                Gender = dto.Gender,
                MobileNumber = dto.MobileNumber,
                CasteCategory = dto.CasteCategory,
                Nationality = dto.Nationality,
                Cgpagained = dto.Cgpagained,
                Cgpatotal = dto.Cgpatotal,
                Percentage = dto.Percentage,
                Gatequalified = dto.Gatequalified,
                FeesPaid = dto.FeesPaid,
                WhatsappNumber = dto.WhatsappNumber,
                EmailId = dto.EmailId,
                Country = dto.Country,
                State = dto.State,
                City = dto.City,
                Pin = dto.Pin,
                District = dto.District,
                GuardianName = dto.GuardianName,
                RelationWithGuardian = dto.RelationWithGuardian,
                GuardianEmail = dto.GuardianEmail,
                GuardianMobileNumber = dto.GuardianMobileNumber,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _repository.Add(student);
        }

        public void UpdateStudent(int id, StudentDetailDto dto)
        {
            var existing = _repository.GetById(id);
            if (existing != null)
            {
                existing.FirstName = dto.FirstName;
                existing.MiddleName = dto.MiddleName;
                existing.LastName = dto.LastName;
                existing.Dob = dto.Dob;
                existing.Gender = dto.Gender;
                existing.MobileNumber = dto.MobileNumber;
                existing.CasteCategory = dto.CasteCategory;
                existing.Nationality = dto.Nationality;
                existing.Cgpagained = dto.Cgpagained;
                existing.Cgpatotal = dto.Cgpatotal;
                existing.Percentage = dto.Percentage;
                existing.Gatequalified = dto.Gatequalified;
                existing.FeesPaid = dto.FeesPaid;
                existing.WhatsappNumber = dto.WhatsappNumber;
                existing.EmailId = dto.EmailId;
                existing.Country = dto.Country;
                existing.State = dto.State;
                existing.City = dto.City;
                existing.Pin = dto.Pin;
                existing.District = dto.District;
                existing.GuardianName = dto.GuardianName;
                existing.RelationWithGuardian = dto.RelationWithGuardian;
                existing.GuardianEmail = dto.GuardianEmail;
                existing.GuardianMobileNumber = dto.GuardianMobileNumber;
                existing.UpdatedAt = DateTime.Now;

                _repository.Update(existing);
            }
        }

        public void DeleteStudent(int id) => _repository.Delete(id);
    }
}
