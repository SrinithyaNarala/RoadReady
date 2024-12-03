using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Models;
using RoadReady.Repositories;
using RoadReady.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using RoadReady.Models.DTO;

namespace RoadReady.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CarExtraController : ControllerBase
    {
        private readonly ICarExtraRepository _carExtraRepository;
        private readonly IMapper _mapper;

        public CarExtraController(ICarExtraRepository carExtraRepository, IMapper mapper)
        {
            _carExtraRepository = carExtraRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Agent")]
        public IActionResult GetAllCarExtras()
        {
            try
            {
                var carExtras = _carExtraRepository.GetAllCarExtras();
                var carExtraDtos = _mapper.Map<List<CarExtraDTO>>(carExtras);
                return Ok(carExtraDtos);
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
        public IActionResult GetCarExtraById(int id)
        {
            try
            {
                var carExtra = _carExtraRepository.GetCarExtraById(id);
                if (carExtra == null)
                    throw new NotFoundException("CarExtra not found.");

                var carExtraDto = _mapper.Map<CarExtraDTO>(carExtra);
                return Ok(carExtraDto);
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
        public IActionResult AddCarExtra([FromBody] CreateCarExtraDTO createCarExtraDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var carExtra = _mapper.Map<CarExtra>(createCarExtraDto);
                _carExtraRepository.AddCarExtra(carExtra);

                var carExtraDto = _mapper.Map<CarExtraDTO>(carExtra);
                return CreatedAtAction(nameof(GetCarExtraById), new { id = carExtra.ExtraId }, carExtraDto);
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
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult UpdateCarExtra(int id, [FromBody] UpdateCarExtraDTO updateCarExtraDto)
        {
            if (id != updateCarExtraDto.ExtraId)
                return BadRequest(new { message = "ID in URL does not match ID in body." });

            try
            {
                var carExtra = _mapper.Map<CarExtra>(updateCarExtraDto);
                _carExtraRepository.UpdateCarExtra(carExtra);

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public IActionResult DeleteCarExtra(int id)
        {
            try
            {
                _carExtraRepository.DeleteCarExtra(id);
                return Ok(new { message = $"ID {id} has been deleted." });
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