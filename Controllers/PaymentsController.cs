using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentsController(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetAllPayments()
        {
            try
            {
                var payments = await _paymentRepository.GetAllPaymentsAsync();
                if (payments == null || !payments.Any())
                {
                    return NotFound("No payments found.");
                }

                var paymentDTOs = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
                return Ok(paymentDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving payments.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<ActionResult<PaymentDTO>> GetPaymentById(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                var paymentDTO = _mapper.Map<PaymentDTO>(payment);
                return Ok(paymentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving the payment.", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDTO paymentDTO)
        {
            if (paymentDTO == null)
            {
                return BadRequest("Payment data is required.");
            }

            try
            {
                var payment = _mapper.Map<Payment>(paymentDTO);
                await _paymentRepository.AddPaymentAsync(payment);
                return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, paymentDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while adding the payment.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] PaymentDTO paymentDTO)
        {
            if (paymentDTO == null)
            {
                return BadRequest("Payment data is required.");
            }

            if (id != paymentDTO.PaymentId)
            {
                return BadRequest("Payment ID mismatch.");
            }

            try
            {
                var existingPayment = await _paymentRepository.GetPaymentByIdAsync(id);
                if (existingPayment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                var payment = _mapper.Map<Payment>(paymentDTO);
                await _paymentRepository.UpdatePaymentAsync(payment);
                return Ok(new { message = $"ID {id} has been updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while updating the payment.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    return NotFound($"Payment with ID {id} not found.");
                }

                await _paymentRepository.DeletePaymentAsync(id);
                return Ok(new { message = $"ID {id} has been deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while deleting the payment.", details = ex.Message });
            }
        }
    }
}