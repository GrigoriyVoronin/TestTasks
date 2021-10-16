using System.Collections.Generic;
using System.Threading.Tasks;
using Artsofte.Models;
using Artsofte.Models.Requests;
using Artsofte.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Artsofte.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentsRepository _departmentsRepository;

        public DepartmentController(DepartmentsRepository departmentsRepository)
        {
            _departmentsRepository = departmentsRepository;
        }

        [HttpPost]
        public async Task<ActionResult<Department>> AddNewDepartment(UpdateDepartmentRequest updateDepartmentRequest)
        {
            try
            {
                var department = new Department();
                UpdateModelFromRequest(department, updateDepartmentRequest);
                await _departmentsRepository.AddAsync(department);
                return Ok(department);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Department>>> GetAllDepartments()
        {
            var departments = await _departmentsRepository.GetAllAsync();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _departmentsRepository.FindAsync(id);
            return department is null
                ? NotFound()
                : Ok(department);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> UpdateDepartment(int id,
            UpdateDepartmentRequest updateDepartmentRequest)
        {
            try
            {
                var department = await _departmentsRepository.FindAsync(id);
                if (department == null)
                    return NotFound();

                UpdateModelFromRequest(department, updateDepartmentRequest);
                await _departmentsRepository.UpdateAsync(department);
                return department;
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentsRepository.FindAsync(id);
            if (department == null)
                return NotFound();
            await _departmentsRepository.DeleteAsync(department);
            return Ok();
        }

        private static void UpdateModelFromRequest(Department department, UpdateDepartmentRequest request)
        {
            department.Name = request.Name;
            department.FloorNumber = request.FloorNumber;
        }
    }
}