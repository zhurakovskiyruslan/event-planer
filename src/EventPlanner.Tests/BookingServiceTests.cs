using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Services;
using EventPlanner.Data.Entities;
using EventPlanner.Data.Enums;
using FluentValidation;
using Moq;

namespace EventPlanner.Tests;

public class BookingServiceTests
{
    private Mock<IBookingRepository> _bookingRepoMock;
    private Mock<IUserRepository> _userRepoMock;
    private Mock<ITicketRepository> _ticketRepoMock;
    private Mock<IEventRepository> _eventRepoMock;
    private Mock<IValidator<Booking>> _validatorMock;
    private BookingService _bookingService;
    
    
    [SetUp]
    public void Setup()
    {
        _bookingRepoMock = new Mock<IBookingRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _ticketRepoMock = new Mock<ITicketRepository>();
        _eventRepoMock = new Mock<IEventRepository>();
        _validatorMock = new Mock<IValidator<Booking>>();

        _bookingService = new BookingService(
            _bookingRepoMock.Object,
            _userRepoMock.Object,
            _ticketRepoMock.Object,
            _eventRepoMock.Object,
            _validatorMock.Object
        );
    }

    #region DeleteAsync
    
    [Test]
    public void DeleteAsync_IdNotFound_ThrowsNotFound()
    {
        //arrange
        _bookingRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(false);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.DeleteAsync(1));
        _bookingRepoMock.Verify(r => r.DeleteAsync(1), Times.Never);
    }
    
    [Test]
    public async Task DeleteAsync_IdExists_CallsDeleteOnce()
    {
        //arrange
        _bookingRepoMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        //act
        await _bookingService.DeleteAsync(1);
        //assert
        _bookingRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
    #endregion

    #region CreateAsync
    [Test]
    public async Task CreateAsync_NewBooking_CallsCreateOnce()
    {
        //arrange
        var booking = new Booking
        {
            UserId = 1,
            TicketId = 1,
            Status = BookingStatus.Active
        };
        _userRepoMock.Setup(r=>r.ExistsAsync(booking.UserId)).ReturnsAsync(true);
        _ticketRepoMock.Setup(r=>r.ExistsAsync(booking.TicketId)).ReturnsAsync(true);
        _bookingRepoMock.Setup(r => r.GetByUserAndTicketAsync(booking.UserId, booking.TicketId)).ReturnsAsync((Booking?)null);
        //act
        await _bookingService.CreateAsync(booking);
        //assert
        _bookingRepoMock.Verify(r=>r.AddAsync(booking), Times.Once);
    }

    [Test]
    public void CreateAsync_ExistingBooking_ThrowsConflict()
    {
        //arrange
        var booking = new Booking
    {
        UserId = 1,
        TicketId = 1,
        Status = BookingStatus.Active
    };
        _userRepoMock.Setup(r=>r.ExistsAsync(booking.UserId)).ReturnsAsync(true);
        _ticketRepoMock.Setup(r=>r.ExistsAsync(booking.TicketId)).ReturnsAsync(true);
        _bookingRepoMock.Setup(r => r.GetByUserAndTicketAsync(booking.UserId, booking.TicketId)).ReturnsAsync(booking);
        booking.Status = BookingStatus.Active;
        //act + assert
        Assert.ThrowsAsync<ConflictException>(() => 
            _bookingService.CreateAsync(booking));
        _bookingRepoMock.Verify(r=> r.GetByUserAndTicketAsync(booking.UserId, booking.TicketId), Times.Once);
        _bookingRepoMock.Verify(r => r.AddAsync(booking), Times.Never);
    }

    [Test]
    public void CreateAsync_NotExistingTicket_ThrowsNotFound()
    {
        //arrange
        var booking = new Booking
        {
            TicketId = 1
        };
        _ticketRepoMock.Setup(r => r.ExistsAsync(booking.TicketId)).ReturnsAsync(false);
        //act + Assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.CreateAsync(booking));
        _bookingRepoMock.Verify(r => r.AddAsync(booking), Times.Never);
    }

    [Test]
    public void CreateAsync_NotExistingUser_ThrowsNotFound()
    {
        //arrange
        var booking = new Booking
        {
            UserId = 1
        };
        _userRepoMock.Setup(r => r.ExistsAsync(booking.UserId)).ReturnsAsync(true);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.CreateAsync(booking));
        _bookingRepoMock.Verify(r => r.AddAsync(booking), Times.Never);
    }
    #endregion
    #region CancelAsync
    [Test]
    public void CancelAsync_IdNotFound_ThrowsNotFound()
    {
        //arrange
        const int id = 1;
        _bookingRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Booking?)null);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.CancelAsync(id));
        _bookingRepoMock.Verify(r => r.CancelAsync(id), Times.Never);
    }

    [Test]
    public void CancelAsync_AlreadyCanceled_ThrowsConflict()
    {
        //arrange
        const int id = 1;
        var booking = new Booking
        {
            Status = BookingStatus.Cancelled
        };
        _bookingRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(booking);
        //act + assert
        Assert.ThrowsAsync<ConflictException>(() => _bookingService.CancelAsync(id));
    }
    #endregion
    #region GetByIdAsync
    
    #endregion
}