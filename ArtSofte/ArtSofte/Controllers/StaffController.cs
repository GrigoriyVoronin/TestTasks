using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Artsofte.Models;
using Artsofte.Models.Requests;
using Artsofte.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Artsofte.Controllers
{
    [Route("api/staff")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly DepartmentsRepository _departmentsRepository;
        private readonly EmployeesRepository _employeesRepository;
        private readonly ProgrammingLanguagesRepository _languagesRepository;

        public StaffController(EmployeesRepository employeesRepository, DepartmentsRepository departmentsRepository,
            ProgrammingLanguagesRepository languagesRepository)
        {
            _employeesRepository = employeesRepository;
            _departmentsRepository = departmentsRepository;
            _languagesRepository = languagesRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> AddNewEmployee(UpdateEmployeeRequest updateEmployeeRequest)
        {
            try
            {
                var employee = new Employee();
                var department = await _departmentsRepository.FindAsync(updateEmployeeRequest.DepartmentId);
                var language = await _languagesRepository.FindAsync(updateEmployeeRequest.ProgrammingLanguageId);
                UpdateModelFromRequest(employee, updateEmployeeRequest, department, language);
                await _employeesRepository.AddAsync(employee);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            var employees = await _employeesRepository.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _employeesRepository.FindAsync(id);
            return employee is null
                ? NotFound()
                : Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, UpdateEmployeeRequest updateEmployeeRequest)
        {
            try
            {
                var employee = await _employeesRepository.FindAsync(id);
                if (employee == null)
                    return NotFound();

                var department = await _departmentsRepository.FindAsync(updateEmployeeRequest.DepartmentId);
                var language = await _languagesRepository.FindAsync(updateEmployeeRequest.ProgrammingLanguageId);
                UpdateModelFromRequest(employee, updateEmployeeRequest, department, language);
                await _employeesRepository.UpdateAsync(employee);
                return employee;
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeesRepository.FindAsync(id);
            if (employee == null)
                return NotFound();
            await _employeesRepository.DeleteAsync(employee);
            return Ok();
        }

        private static void UpdateModelFromRequest(Employee employee, UpdateEmployeeRequest request,
            Department department, ProgrammingLanguage programmingLanguage)
        {
            employee.Name = request.Name;
            employee.LastName = request.LastName;
            employee.Age = request.Age;
            employee.Department = department;
            employee.ProgrammingLanguage = programmingLanguage;
        }
    }
}