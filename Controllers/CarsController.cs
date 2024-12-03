using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;
using RoadReady.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using RoadReady.Exceptions;
using AutoMapper;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;

        public CarsController(ICarRepository carRepository, IMapper mapper)
        {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<IEnumerable<CarDTO>>> GetAllCars()
        {
            try
            {
                var cars = await _carRepository.GetAllCarsAsync();
                var carDTOs = _mapper.Map<IEnumerable<CarDTO>>(cars);
                return Ok(carDTOs);
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

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public async Task<ActionResult<CarDTO>> GetCarById(int id)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(id);
                if (car == null)
                    throw new NotFoundException("Car not found.");

                var carDTO = _mapper.Map<CarDTO>(car);
                return Ok(carDTO);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> AddCar([FromBody] CarDTO carDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var car = _mapper.Map<Car>(carDTO);
                await _carRepository.AddCarAsync(car);
                var createdCarDTO = _mapper.Map<CarDTO>(car);

                return CreatedAtAction(nameof(GetCarById), new { id = car.CarId }, createdCarDTO);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] CarDTO carDTO)
        {
            if (id != carDTO.CarId)
                return BadRequest(new { message = "ID in URL does not match ID in body." });

            try
            {
                var car = _mapper.Map<Car>(carDTO);
                await _carRepository.UpdateCarAsync(car);
                return Ok(new { message = $"Car with ID {id} has been updated." });
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                await _carRepository.DeleteCarAsync(id);
                return Ok(new { message = $"Car with ID {id} has been deleted." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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