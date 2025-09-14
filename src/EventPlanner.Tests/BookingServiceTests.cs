using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.ReadModels;
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
    public async Task CancelAsync_CallsDeleteOnce()
    {
        //arrange
        const int id = 1;
        var booking = new Booking()
        { 
            Status = BookingStatus.Active
        };
        _bookingRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(booking);
        //act
        await _bookingService.CancelAsync(id);
        //assert
        _bookingRepoMock.Verify(r => r.CancelAsync(id), Times.Once);
        
    }
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

    [Test]
    public async Task GetById_ExistingBooking_CallsGetOnce()
    {
        //arrange
        const int id = 1;
        _bookingRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new Booking());
        //act
        await _bookingService.GetById(id);
        //assert
        _bookingRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Test]
    public void GetById_NotExistingBooking_CallsGetOnce()
    {
        //arrange
        const int id = 1;
        _bookingRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Booking?)null);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetById(id));
        _bookingRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }
    #endregion
    #region GetByUserIdAsync

    

    [Test]
    public void GetByUserId_NotExistingBooking_CallsGetOnce()
    {
        //arrange
        var booking = new Booking
        {
            UserId = 1
        };
        _userRepoMock.Setup(r => r.ExistsAsync(booking.UserId)).ReturnsAsync(true);
        _bookingRepoMock.Setup(r => r.GetByUserIdAsync(booking.UserId)).
            ReturnsAsync(new List<BookingDto>{});
        //act+assert
        Assert.ThrowsAsync<NotFoundException>(async () => await _bookingService.GetByUserId(booking.UserId));
        _bookingRepoMock.Verify(r => r.GetByUserIdAsync(booking.UserId), Times.Once);
        _userRepoMock.Verify(r=>r.ExistsAsync(booking.UserId), Times.Once);
    }

    [Test]
    public void GetByUserId_NotExistingUser_CallsGetOnce()
    {
        //arrange
        var booking = new Booking
        {
            UserId = 1
        };
        _userRepoMock.Setup(r => r.ExistsAsync(booking.UserId)).ReturnsAsync(false);
        //act+assert
        Assert.ThrowsAsync<NotFoundException>(async () => await _bookingService.GetByUserId(booking.UserId));
        _bookingRepoMock.Verify(r => r.GetByUserIdAsync(booking.UserId), Times.Never);
        _userRepoMock.Verify(r => r.ExistsAsync(booking.UserId), Times.Once);
    }
    
    #endregion
    
    #region GetActiveBooking

    [Test]
    public async Task GetActiveBooking_ExistingActiveBooking_CallsGetOnce()
    {
        var bookings = new List<Booking>();
        var booking = new Booking();
        bookings.Add(booking);
        //arrange
        _bookingRepoMock.Setup(r=> r.GetActiveBookingsAsync()).ReturnsAsync(bookings);
        //act
        var result = await _bookingService.GetActiveBooking();
        //assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<Booking>>());
        Assert.That(result, Is.Not.Empty);
        _bookingRepoMock.Verify(r => r.GetActiveBookingsAsync(), Times.Once);
    }

    [Test]
    public void GetActiveBooking_NotExistingActiveBooking_CallsGetOnce()
    {
        //arrange
        _bookingRepoMock.Setup(r => r.GetActiveBookingsAsync()).ReturnsAsync(new List<Booking>());
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetActiveBooking());
        _bookingRepoMock.Verify(r => r.GetActiveBookingsAsync(), Times.Once);
    }
    
    #endregion
    
    #region GetByEventId

    [Test]
    public async Task GetByEventId_ExistingBookingAndEvent_CallsGetOnce()
    {
        //arrange
        const int id = 1;
        var booking = new Booking();
        var bookings = new List<Booking> { booking };
        _eventRepoMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _bookingRepoMock.Setup(r => r.GetByEventIdAsync(id)).ReturnsAsync(bookings);
        //act
        var result = await _bookingService.GetByEventId(id);
        //assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<List<Booking>>());
        Assert.That(result, Is.Not.Empty);
        _bookingRepoMock.Verify(r => r.GetByEventIdAsync(id), Times.Once);
        
    }

    [Test]
    public void GetByEventId_NotExistingBooking_CallsNever()
    {
        //arrange
        const int id = 1;
        _eventRepoMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(false);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetByEventId(id));
        _bookingRepoMock.Verify(r => r.GetByEventIdAsync(id), Times.Never);
    }

    [Test]
    public void GetByEventId_NotExistingEvent_CallsGetOnce()
    {
        //arrange
        const int id = 1;
        var bookings = new List<Booking>();
        _eventRepoMock.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);
        _bookingRepoMock.Setup(r => r.GetByEventIdAsync(id)).ReturnsAsync(bookings);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetByEventId(id));
        _bookingRepoMock.Verify(r => r.GetByEventIdAsync(id), Times.Once);
        
    }
    #endregion
    
    #region GetByUserAndTickets

    [Test]
    public async Task GetByUserAndTickets_ExistingBooking_CallsGetOnce()
    {
        //arrange
        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _ticketRepoMock.Setup(r=> r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _bookingRepoMock.Setup(r=> r.GetByUserAndTicketAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new Booking());
        //act
        var result = await _bookingService.GetByUserAndTickets(It.IsAny<int>(), It.IsAny<int>());
        //assert
        Assert.That(result?.GetType(), Is.EqualTo(typeof(Booking)) );
        _bookingRepoMock.Verify(r=> r.GetByUserAndTicketAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Test]
    public void GetByUserAndTickets_NotExistingBooking_CallsOnce()
    {
        //arrange
        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _ticketRepoMock.Setup(r=> r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _bookingRepoMock.Setup(r=> r.GetByUserAndTicketAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Booking?) null);
        
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetByUserAndTickets(It.IsAny<int>(), It.IsAny<int>()));
        _bookingRepoMock.Verify(r => r.GetByUserAndTicketAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
    }

    [Test]
    public void GetByUserAndTickets_NotExistingUser_CallsNever()
    {
        //arrange
        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetByUserAndTickets(It.IsAny<int>(), It.IsAny<int>()));
        _bookingRepoMock.Verify(r => r.GetByUserAndTicketAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _ticketRepoMock.Verify(r => r.ExistsAsync( It.IsAny<int>()), Times.Never);

    }

    [Test]
    public void GetByUserAndTickets_NotExistingTicket_CallsNever()
    {
        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _ticketRepoMock.Setup(r=> r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);
        //act + assert
        Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetByUserAndTickets(It.IsAny<int>(), It.IsAny<int>()));
        _bookingRepoMock.Verify(r => r.GetByUserAndTicketAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _ticketRepoMock.Verify(r => r.ExistsAsync( It.IsAny<int>()), Times.Once);
    }
    #endregion
}