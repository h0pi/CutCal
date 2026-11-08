using CutCal.Model.Common;
using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Requests;
using CutCal.Model.SearchObjects;
using CutCal.Model.Responses;
using CutCal.Services.Database;
using CutCal.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

public class AppointmentServiceTests : IDisposable
{
    private sealed class RecordingPayments : IPaymentService
    {
        public List<int> Refunded { get; } = new();

        public Task<PaymentResponse> RefundAsync(int appointmentId)
        {
            Refunded.Add(appointmentId);
            return Task.FromResult(new PaymentResponse());
        }

        public List<int> RefundedAfterCancellation { get; } = new();

        public Task<PaymentResponse> RefundCancelledAppointmentAsync(int appointmentId)
        {
            RefundedAfterCancellation.Add(appointmentId);
            return Task.FromResult(new PaymentResponse());
        }

        public Task<PaymentQuoteResponse> GetQuoteAsync(PaymentQuoteRequest request) => throw new NotSupportedException();
        public Task<CreateOrderResponse> CreateOrderAsync(PaymentQuoteRequest request) => throw new NotSupportedException();
        public Task<PaymentResponse> CaptureOrderAsync(string paypalOrderId, int appointmentId) => throw new NotSupportedException();
        public Task<CreateCheckoutSessionResponse> CreateStripeSessionAsync(PaymentQuoteRequest request) => throw new NotSupportedException();
        public Task<PaymentResponse> ConfirmStripePaymentAsync(int appointmentId) => throw new NotSupportedException();
        public Task HandleStripeWebhookAsync(string payload, string signatureHeader) => throw new NotSupportedException();
    }

    private readonly CutCalDbContext _context;
    private readonly RecordingPublisher _publisher = new();
    private readonly RecordingPayments _payments = new();
    private readonly Appointment _seeded;
    private readonly DateTime _day = DateTime.UtcNow.Date.AddDays(7);

    public AppointmentServiceTests()
    {
        _context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _context.Database.EnsureCreated();

        _seeded = _context.Appointments.Single(x => x.Id == 1);
        _context.SalonWorkingHours.RemoveRange(_context.SalonWorkingHours.Where(x => x.SalonId == _seeded.SalonId && x.DayOfWeek == (int)_day.DayOfWeek));
        _context.SalonWorkingHours.Add(new SalonWorkingHours
        {
            SalonId = _seeded.SalonId, DayOfWeek = (int)_day.DayOfWeek, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(20, 0)
        });
        _context.SalonServices.Single(x => x.Id == _seeded.ServiceId).DurationMinutes = 60;
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    private AppointmentService ServiceFor(int userId, string role) =>
        new(_context, _publisher, new NotificationService(_context), new SignedInUser(userId, role), _payments, NullLogger<AppointmentService>.Instance);

    private Task<AppointmentResponse> BookAsync(DateTime at) => ServiceFor(_seeded.CustomerId, RoleNames.Customer).InsertAsync(new AppointmentInsertRequest
    {
        SalonId = _seeded.SalonId,
        StaffId = _seeded.StaffId,
        ServiceId = _seeded.ServiceId,
        ScheduledAt = at,
        PaymentMethod = PaymentMethodNames.Cash
    }, _seeded.CustomerId);

    [Fact]
    public async Task Books_an_appointment_inside_working_hours()
    {
        var booked = await BookAsync(_day.AddHours(10));

        Assert.Equal(_day.AddHours(10), booked.ScheduledAt);
    }

    [Theory]
    [InlineData(23, 30)] // starts after closing and ends after midnight
    [InlineData(19, 30)] // starts while open but ends after closing
    [InlineData(8, 30)] // starts before opening
    public async Task Rejects_an_appointment_that_is_not_fully_inside_working_hours(int hour, int minute)
    {
        var error = await Assert.ThrowsAsync<ClientException>(() => BookAsync(_day.AddHours(hour).AddMinutes(minute)));

        Assert.Equal("Selected time is outside of salon working hours.", error.Message);
    }

    [Fact]
    public async Task Rejects_a_service_the_salon_has_deactivated()
    {
        _context.SalonServices.Single(x => x.Id == _seeded.ServiceId).IsActive = false;
        await _context.SaveChangesAsync();

        var error = await Assert.ThrowsAsync<ClientException>(() => BookAsync(_day.AddHours(10)));

        Assert.Equal("Service not found for this salon.", error.Message);
    }

    [Fact]
    public async Task Rejects_a_staff_member_who_has_been_deactivated()
    {
        _context.Staff.Single(x => x.Id == _seeded.StaffId).IsActive = false;
        await _context.SaveChangesAsync();

        var error = await Assert.ThrowsAsync<ClientException>(() => BookAsync(_day.AddHours(10)));

        Assert.Equal("Selected staff member is not available for this service.", error.Message);
    }

    [Fact]
    public async Task Rejects_a_staff_member_who_does_not_perform_the_service()
    {
        _context.StaffServices.RemoveRange(_context.StaffServices.Where(x => x.StaffId == _seeded.StaffId && x.ServiceId == _seeded.ServiceId));
        await _context.SaveChangesAsync();

        var error = await Assert.ThrowsAsync<ClientException>(() => BookAsync(_day.AddHours(10)));

        Assert.Equal("Selected staff member is not available for this service.", error.Message);
    }

    // Who may change an appointment (cancel, confirm, complete).
    private Salon Salon => _context.Salons.Single(x => x.Id == _seeded.SalonId);
    private int AssignedStaffUserId => _context.Staff.Single(x => x.Id == _seeded.StaffId).UserId;
    private int ManagerOfAnotherSalonId => _context.Salons.First(x => x.OwnerId != Salon.OwnerId).OwnerId;
    private int OtherStaffUserId => _context.Staff.First(x => x.UserId != AssignedStaffUserId).UserId;
    private int OtherCustomerId => _context.Users.First(x => x.Id != _seeded.CustomerId && x.UserRoles.Any(r => r.Role.Name == RoleNames.Customer)).Id;

    private Task ArrangeAsync(string state, string paymentStatus = PaymentStatusNames.Unpaid)
    {
        var appointment = _context.Appointments.Single(x => x.Id == _seeded.Id);
        appointment.StateName = state;
        appointment.PaymentStatus = paymentStatus;
        return _context.SaveChangesAsync();
    }

    private string StateOfAppointment => _context.Appointments.Single(x => x.Id == _seeded.Id).StateName;

    private async Task AssertNotAllowedAsync(Func<Task> action, string unchangedState)
    {
        var error = await Assert.ThrowsAsync<ClientException>(action);

        Assert.Equal("You are not allowed to change this appointment.", error.Message);
        Assert.Equal(unchangedState, StateOfAppointment);
        Assert.Empty(_payments.RefundedAfterCancellation);
    }

    [Fact]
    public async Task Another_customer_cannot_cancel_the_appointment_or_trigger_its_refund()
    {
        await ArrangeAsync(AppointmentStateNames.Confirmed, PaymentStatusNames.Paid);

        await AssertNotAllowedAsync(() => ServiceFor(OtherCustomerId, RoleNames.Customer).CancelAsync(_seeded.Id, "no", OtherCustomerId), AppointmentStateNames.Confirmed);
    }

    [Fact]
    public async Task The_customer_cancels_their_own_paid_appointment_and_it_is_refunded()
    {
        await ArrangeAsync(AppointmentStateNames.Confirmed, PaymentStatusNames.Paid);

        await ServiceFor(_seeded.CustomerId, RoleNames.Customer).CancelAsync(_seeded.Id, "plans changed", _seeded.CustomerId);

        Assert.Equal(AppointmentStateNames.Cancelled, StateOfAppointment);
        Assert.Equal(_seeded.Id, Assert.Single(_payments.RefundedAfterCancellation));
    }

    [Fact]
    public async Task A_manager_cancels_only_appointments_of_their_own_salon()
    {
        await ArrangeAsync(AppointmentStateNames.Pending);
        await AssertNotAllowedAsync(() => ServiceFor(ManagerOfAnotherSalonId, RoleNames.SalonManager).CancelAsync(_seeded.Id, "no", ManagerOfAnotherSalonId), AppointmentStateNames.Pending);

        await ServiceFor(Salon.OwnerId, RoleNames.SalonManager).CancelAsync(_seeded.Id, "closed", Salon.OwnerId);

        Assert.Equal(AppointmentStateNames.Cancelled, StateOfAppointment);
    }

    [Fact]
    public async Task Staff_cancel_only_appointments_assigned_to_them()
    {
        await ArrangeAsync(AppointmentStateNames.Pending);
        await AssertNotAllowedAsync(() => ServiceFor(OtherStaffUserId, RoleNames.Staff).CancelAsync(_seeded.Id, "no", OtherStaffUserId), AppointmentStateNames.Pending);

        await ServiceFor(AssignedStaffUserId, RoleNames.Staff).CancelAsync(_seeded.Id, "sick", AssignedStaffUserId);

        Assert.Equal(AppointmentStateNames.Cancelled, StateOfAppointment);
    }

    [Fact]
    public async Task An_administrator_cancels_any_appointment()
    {
        await ArrangeAsync(AppointmentStateNames.Pending);

        await ServiceFor(0, RoleNames.Admin).CancelAsync(_seeded.Id, "policy", 0);

        Assert.Equal(AppointmentStateNames.Cancelled, StateOfAppointment);
    }

    [Fact]
    public async Task A_manager_confirms_only_appointments_of_their_own_salon()
    {
        await ArrangeAsync(AppointmentStateNames.Pending);
        await AssertNotAllowedAsync(() => ServiceFor(ManagerOfAnotherSalonId, RoleNames.SalonManager).ConfirmAsync(_seeded.Id, ManagerOfAnotherSalonId), AppointmentStateNames.Pending);

        await ServiceFor(Salon.OwnerId, RoleNames.SalonManager).ConfirmAsync(_seeded.Id, Salon.OwnerId);

        var appointment = _context.Appointments.Single(x => x.Id == _seeded.Id);
        Assert.Equal(AppointmentStateNames.Confirmed, appointment.StateName);
        Assert.Equal(Salon.OwnerId, appointment.ApprovedById);
    }

    [Fact]
    public async Task A_customer_cannot_confirm_their_own_appointment()
    {
        await ArrangeAsync(AppointmentStateNames.Pending);

        await AssertNotAllowedAsync(() => ServiceFor(_seeded.CustomerId, RoleNames.Customer).ConfirmAsync(_seeded.Id, _seeded.CustomerId), AppointmentStateNames.Pending);
    }

    [Fact]
    public async Task Staff_and_managers_complete_only_appointments_they_are_responsible_for()
    {
        await ArrangeAsync(AppointmentStateNames.Confirmed);
        await AssertNotAllowedAsync(() => ServiceFor(OtherStaffUserId, RoleNames.Staff).CompleteAsync(_seeded.Id), AppointmentStateNames.Confirmed);
        await AssertNotAllowedAsync(() => ServiceFor(ManagerOfAnotherSalonId, RoleNames.SalonManager).CompleteAsync(_seeded.Id), AppointmentStateNames.Confirmed);

        await ServiceFor(AssignedStaffUserId, RoleNames.Staff).CompleteAsync(_seeded.Id);

        Assert.Equal(AppointmentStateNames.Completed, StateOfAppointment);
    }

    private AppointmentService AdminAppointments => ServiceFor(0, RoleNames.Admin);

    [Fact]
    public async Task Searching_by_customer_name_filters_before_paging()
    {
        var customer = _context.Users.Single(x => x.Id == _context.Appointments.OrderBy(a => a.ScheduledAt).First().CustomerId);
        var expected = _context.Appointments.Count(x => x.CustomerId == customer.Id);

        var page = await AdminAppointments.GetPagedAsync(new AppointmentSearchObject { CustomerName = $"{customer.FirstName} {customer.LastName}", PageSize = 1 });

        Assert.Equal(expected, page.TotalCount);
        Assert.Equal(customer.Id, Assert.Single(page.Items).CustomerId);
    }

    [Fact]
    public async Task The_end_of_the_period_is_exclusive_so_the_start_of_the_next_day_includes_the_whole_last_day()
    {
        var booked = await BookAsync(_day.AddHours(18));
        bool Found(PageResult<AppointmentResponse> page) => page.Items.Any(x => x.Id == booked.Id);

        Assert.True(Found(await AdminAppointments.GetPagedAsync(new AppointmentSearchObject { DateFrom = _day, DateTo = _day.AddDays(1), PageSize = 100 })));
        Assert.False(Found(await AdminAppointments.GetPagedAsync(new AppointmentSearchObject { DateFrom = _day, DateTo = _day, PageSize = 100 })));
    }

    [Fact]
    public async Task Both_reports_cover_only_the_chosen_period_including_its_last_day()
    {
        await BookAsync(_day.AddHours(18));
        var reports = new ReportService(_context, new SignedInUser(0, RoleNames.Admin));

        var appointments = await reports.GetAppointmentsReportAsync(_seeded.SalonId, _day, _day.AddDays(1));
        var services = await reports.GetServicesReportAsync(_seeded.SalonId, _day, _day.AddDays(1));
        var otherDay = await reports.GetServicesReportAsync(_seeded.SalonId, _day.AddDays(1), _day.AddDays(2));

        Assert.Equal(1, appointments.TotalAppointments);
        Assert.Equal(1, Assert.Single(services.Services).AppointmentCount);
        Assert.Empty(otherDay.Services);
    }
}
