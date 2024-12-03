using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Customer,Agent")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ICarExtraRepository _carExtraRepository;
        private readonly ILogger<ReservationsController> _logger;
        private readonly IMapper _mapper;

        public ReservationsController(IReservationRepository reservationRepository, ICarExtraRepository carExtraRepository, ILogger<ReservationsController> logger, IMapper mapper)
        {
            _reservationRepository = reservationRepository;
            _carExtraRepository = carExtraRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // Get all reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationRepository.GetAllReservationsAsync();
                if (!reservations.Any())
                {
                    return NotFound(new { message = "No reservations found." });
                }

                var reservationDTOs = _mapper.Map<List<ReservationDTO>>(reservations);
                return Ok(reservationDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving reservations.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservationById(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound(new { message = $"Reservation with ID {id} not found." });
                }

                // Map to DTO with CarExtras
                var reservationDTO = _mapper.Map<ReservationDTO>(reservation);
                return Ok(reservationDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the reservation with ID {id}.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Customer,Agent")]
        public async Task<IActionResult> AddReservation([FromBody] CreateReservationDTO reservationDTO)
        {
            if (reservationDTO == null || reservationDTO.CarExtraIds == null)
            {
                return BadRequest(new { message = "Reservation data and CarExtraIds are required." });
            }

            try
            {
                // Fetch CarExtras based on the CarExtraIds provided
                var carExtras = new List<CarExtra>();
                foreach (var extraId in reservationDTO.CarExtraIds)
                {
                    var carExtra = _carExtraRepository.GetCarExtraById(extraId);
                    if (carExtra != null)
                    {
                        carExtras.Add(carExtra);
                    }
                    else
                    {
                        // If a CarExtra does not exist, return an error message
                        return BadRequest(new { message = $"CarExtra with ID {extraId} not found." });
                    }
                }

                // Map CreateReservationDTO to Reservation entity
                var reservation = _mapper.Map<Reservation>(reservationDTO);

                // Associate the CarExtras with the reservation
                reservation.Extras = carExtras;  // Add CarExtras from carExtraIds only

                // Save the reservation to the database
                await _reservationRepository.AddReservationAsync(reservation);

                // Map the saved reservation back to DTO
                var createdDTO = _mapper.Map<ReservationDTO>(reservation);

                return CreatedAtAction(nameof(GetReservationById), new { id = createdDTO.ReservationId }, createdDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a reservation.");
                return StatusCode(500, new { message = "An error occurred while saving the reservation." });
            }
        }

        // Update an existing reservation
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] ReservationDTO reservationDTO)
        {
            if (reservationDTO == null || id != reservationDTO.ReservationId)
            {
                return BadRequest(new { message = "Invalid reservation data or ID mismatch." });
            }

            try
            {
                var existingReservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (existingReservation == null)
                {
                    return NotFound(new { message = $"Reservation with ID {id} not found." });
                }

                var reservation = _mapper.Map(reservationDTO, existingReservation);
                await _reservationRepository.UpdateReservationAsync(reservation);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the reservation with ID {id}.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // Delete a reservation
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetReservationByIdAsync(id);
                if (reservation == null)
                {
                    return NotFound(new { message = $"Reservation with ID {id} not found." });
                }

                await _reservationRepository.DeleteReservationAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the reservation with ID {id}.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}