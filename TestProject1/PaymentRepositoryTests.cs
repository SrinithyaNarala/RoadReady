using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RoadReady.Authentication;
using RoadReady.Models;
using RoadReady.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class PaymentRepositoryTests
{
    private RoadReadyContext _context;
    private PaymentRepository _paymentRepository;

    [SetUp]
    public void Setup()
    {
        // Create unique in-memory DbContextOptions for test isolation
        var options = new DbContextOptionsBuilder<RoadReadyContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;

        // Initialize DbContext with in-memory options
        _context = new RoadReadyContext(options);

        // Seed data
        _context.Payments.AddRange(
            new Payment
            {
                PaymentId = 1,
                ReservationId = 101,
                PaymentMethod = "Credit Card",
                PaymentDate = DateTime.Now.AddDays(-1),
                Amount = 150.00M,
                PaymentStatus = "Completed"
            },
            new Payment
            {
                PaymentId = 2,
                ReservationId = 102,
                PaymentMethod = "PayPal",
                PaymentDate = DateTime.Now.AddDays(-2),
                Amount = 200.00M,
                PaymentStatus = "Pending"
            }
        );
        _context.SaveChanges();

        // Initialize repository
        _paymentRepository = new PaymentRepository(_context);
    }

    [TearDown]
    public void Cleanup()
    {
        // Dispose of the context after each test
        _context.Dispose();
    }

    [Test]
    public async Task GetPaymentByIdAsync_ShouldReturnPayment_WhenExists()
    {
        // Act
        var result = await _paymentRepository.GetPaymentByIdAsync(1);

        // Assert
        Assert.IsNotNull(result, "Expected a payment record but got null.");
        Assert.AreEqual(101, result.ReservationId, "ReservationId does not match.");
        Assert.AreEqual("Credit Card", result.PaymentMethod, "PaymentMethod does not match.");
    }

    [Test]
    public async Task GetPaymentByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _paymentRepository.GetPaymentByIdAsync(99);

        // Assert
        Assert.IsNull(result, "Expected no payment record but found one.");
    }

    [Test]
    public async Task GetAllPaymentsAsync_ShouldReturnAllPayments()
    {
        // Act
        var result = await _paymentRepository.GetAllPaymentsAsync();

        // Assert
        Assert.IsNotNull(result, "Expected a list of payments but got null.");
        Assert.AreEqual(2, result.Count(), "Number of payments returned does not match expected value.");
    }

    [Test]
    public async Task AddPaymentAsync_ShouldAddNewPayment()
    {
        // Arrange
        var newPayment = new Payment
        {
            PaymentId = 3,
            ReservationId = 103,
            PaymentMethod = "Bank Transfer",
            PaymentDate = DateTime.Now,
            Amount = 300.00M,
            PaymentStatus = "Processing"
        };

        // Act
        await _paymentRepository.AddPaymentAsync(newPayment);

        // Assert
        var result = await _paymentRepository.GetPaymentByIdAsync(3);
        Assert.IsNotNull(result, "Expected the new payment record but got null.");
        Assert.AreEqual(103, result.ReservationId, "ReservationId does not match.");
        Assert.AreEqual("Bank Transfer", result.PaymentMethod, "PaymentMethod does not match.");
    }

    [Test]
    public async Task UpdatePaymentAsync_ShouldUpdatePayment()
    {
        // Arrange
        var payment = await _paymentRepository.GetPaymentByIdAsync(1);
        payment.PaymentMethod = "Debit Card";
        payment.Amount = 160.00M;

        // Act
        await _paymentRepository.UpdatePaymentAsync(payment);

        // Assert
        var result = await _paymentRepository.GetPaymentByIdAsync(1);
        Assert.IsNotNull(result, "Expected the payment record but got null.");
        Assert.AreEqual("Debit Card", result.PaymentMethod, "PaymentMethod update failed.");
        Assert.AreEqual(160.00M, result.Amount, "Amount update failed.");
    }

    [Test]
    public async Task DeletePaymentAsync_ShouldRemovePayment_WhenExists()
    {
        // Act
        await _paymentRepository.DeletePaymentAsync(1);

        // Assert
        var result = await _paymentRepository.GetPaymentByIdAsync(1);
        Assert.IsNull(result, "Expected the payment record to be deleted but it still exists.");
    }

    [Test]
    public async Task DeletePaymentAsync_ShouldNotThrow_WhenRecordDoesNotExist()
    {
        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _paymentRepository.DeletePaymentAsync(99), "Expected no exception when deleting a non-existent record.");
    }
}
