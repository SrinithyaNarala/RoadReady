using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;
using Microsoft.AspNetCore.Authorization;
using RoadReady.Exceptions;

namespace RoadReady.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardDataController : ControllerBase
    {
        private readonly IAdminDashboardDataRepository _adminDashboardDataRepository;

        public AdminDashboardDataController(IAdminDashboardDataRepository adminDashboardDataRepository)
        {
            _adminDashboardDataRepository = adminDashboardDataRepository;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllAdminDashboardData()
        {
            try
            {
                var data = _adminDashboardDataRepository.GetAllAdminDashboardData();
                return Ok(data);
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpPost]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> AddDashboardData([FromBody] AdminDashboardData dashboardData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _adminDashboardDataRepository.AddDashboardDataAsync(dashboardData);
                return CreatedAtAction(nameof(GetAllAdminDashboardData), new { id = dashboardData.DashboardId }, dashboardData);
            }
            catch (DuplicateResourceException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpPut("{id}")]
        //[Authorize(Roles = "admin,agent")]
        public async Task<IActionResult> UpdateDashboardData(int id, [FromBody] AdminDashboardData dashboardData)
        {
            if (id != dashboardData.DashboardId)
                return BadRequest(new { message = "ID in URL does not match ID in body." });

            try
            {
                await _adminDashboardDataRepository.UpdateDashboardDataAsync(dashboardData);
                return Ok(new { message = $"ID {id} has been updated." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InternalServerException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }



}