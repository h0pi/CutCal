using CutCal.Model.Exceptions;
using CutCal.Model.Responses;
using CutCal.Services.Auth;
using CutCal.Services.Database;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CutCal.Services.Services;

public interface IReportService
{
    Task<AppointmentsReportResponse> GetAppointmentsReportAsync(int? salonId, DateTime? dateFrom, DateTime? dateTo);
    Task<ServicesReportResponse> GetServicesReportAsync(int? salonId, int? month, int? year);
    Task<byte[]> GenerateAppointmentsPdfAsync(int? salonId, DateTime? dateFrom, DateTime? dateTo);
    Task<byte[]> GenerateServicesPdfAsync(int? salonId, int? month, int? year);
}

public class ReportService : IReportService
{
    private readonly CutCalDbContext _context;
    private readonly IAuthenticatedUserAccessor _userAccessor;

    static ReportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public ReportService(CutCalDbContext context, IAuthenticatedUserAccessor userAccessor)
    {
        _context = context;
        _userAccessor = userAccessor;
    }

    /// <summary>
    /// Admin can view any salon (or all salons). A SalonManager is restricted to the salon(s)
    /// they own: an explicit salonId must be one of their own, and omitting it scopes to all of them.
    /// </summary>
    private async Task<int[]?> ResolveAllowedSalonIdsAsync(int? requestedSalonId)
    {
        if (_userAccessor.IsInRole("Admin"))
        {
            return requestedSalonId.HasValue ? new[] { requestedSalonId.Value } : null;
        }

        var ownedSalonIds = await _context.Salons
            .Where(s => s.OwnerId == _userAccessor.UserId)
            .Select(s => s.Id)
            .ToArrayAsync();

        if (requestedSalonId.HasValue)
        {
            if (!ownedSalonIds.Contains(requestedSalonId.Value))
            {
                throw new ClientException("You do not manage this salon.");
            }
            return new[] { requestedSalonId.Value };
        }

        return ownedSalonIds;
    }

    public async Task<AppointmentsReportResponse> GetAppointmentsReportAsync(int? salonId, DateTime? dateFrom, DateTime? dateTo)
    {
        var allowedSalonIds = await ResolveAllowedSalonIdsAsync(salonId);

        var query = _context.Appointments
            .Include(x => x.Customer)
            .Include(x => x.Salon)
            .Include(x => x.Staff).ThenInclude(s => s.User)
            .Include(x => x.Service)
            .AsQueryable();

        if (allowedSalonIds is not null) query = query.Where(x => allowedSalonIds.Contains(x.SalonId));
        if (dateFrom.HasValue) query = query.Where(x => x.ScheduledAt >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(x => x.ScheduledAt <= dateTo.Value);

        var appointments = await query.OrderBy(x => x.ScheduledAt).ToListAsync();

        return new AppointmentsReportResponse
        {
            TotalAppointments = appointments.Count,
            ConfirmedCount = appointments.Count(x => x.StateName == "Confirmed"),
            CancelledCount = appointments.Count(x => x.StateName == "Cancelled"),
            CompletedCount = appointments.Count(x => x.StateName == "Completed"),
            TotalRevenue = appointments.Where(x => x.PaymentStatus == "Paid").Sum(x => x.Price),
            Appointments = appointments.Select(a => new AppointmentResponse
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                CustomerName = $"{a.Customer.FirstName} {a.Customer.LastName}",
                SalonId = a.SalonId,
                SalonName = a.Salon.Name,
                StaffId = a.StaffId,
                StaffName = $"{a.Staff.User.FirstName} {a.Staff.User.LastName}",
                ServiceId = a.ServiceId,
                ServiceName = a.Service.Name,
                ScheduledAt = a.ScheduledAt,
                DurationMinutes = a.DurationMinutes,
                Price = a.Price,
                StateName = a.StateName,
                PaymentMethod = a.PaymentMethod,
                PaymentStatus = a.PaymentStatus,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }

    public async Task<ServicesReportResponse> GetServicesReportAsync(int? salonId, int? month, int? year)
    {
        var allowedSalonIds = await ResolveAllowedSalonIdsAsync(salonId);

        var query = _context.Appointments.Include(x => x.Service).AsQueryable();
        if (allowedSalonIds is not null) query = query.Where(x => allowedSalonIds.Contains(x.SalonId));
        if (month.HasValue) query = query.Where(x => x.ScheduledAt.Month == month.Value);
        if (year.HasValue) query = query.Where(x => x.ScheduledAt.Year == year.Value);

        var appointments = await query.ToListAsync();

        var ratings = await _context.Reviews.Where(r => !r.IsRemoved && (allowedSalonIds == null || allowedSalonIds.Contains(r.SalonId)))
            .Include(r => r.Appointment)
            .ToListAsync();

        var grouped = appointments.GroupBy(x => new { x.ServiceId, x.Service.Name })
            .Select(g => new ServiceReportItem
            {
                ServiceName = g.Key.Name,
                AppointmentCount = g.Count(),
                TotalRevenue = g.Where(a => a.PaymentStatus == "Paid").Sum(a => a.Price),
                AverageRating = ratings.Where(r => r.Appointment.ServiceId == g.Key.ServiceId).Select(r => (double)r.Rating).DefaultIfEmpty(0).Average()
            })
            .OrderByDescending(x => x.AppointmentCount)
            .ToList();

        return new ServicesReportResponse
        {
            MostPopularService = grouped.FirstOrDefault()?.ServiceName,
            TotalRevenue = grouped.Sum(x => x.TotalRevenue),
            Services = grouped
        };
    }

    public async Task<byte[]> GenerateAppointmentsPdfAsync(int? salonId, DateTime? dateFrom, DateTime? dateTo)
    {
        var report = await GetAppointmentsReportAsync(salonId, dateFrom, dateTo);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Text("CutCal - Appointments Report").FontSize(18).Bold();
                page.Content().Column(column =>
                {
                    column.Item().Text($"Total: {report.TotalAppointments}  Confirmed: {report.ConfirmedCount}  Cancelled: {report.CancelledCount}  Completed: {report.CompletedCount}  Revenue: {report.TotalRevenue:C}");
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Date").Bold();
                            header.Cell().Text("Customer").Bold();
                            header.Cell().Text("Service").Bold();
                            header.Cell().Text("Staff").Bold();
                            header.Cell().Text("Status").Bold();
                            header.Cell().Text("Price").Bold();
                        });

                        foreach (var a in report.Appointments)
                        {
                            table.Cell().Text(a.ScheduledAt.ToString("g"));
                            table.Cell().Text(a.CustomerName);
                            table.Cell().Text(a.ServiceName);
                            table.Cell().Text(a.StaffName);
                            table.Cell().Text(a.StateName);
                            table.Cell().Text(a.Price.ToString("C"));
                        }
                    });
                });
                page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateServicesPdfAsync(int? salonId, int? month, int? year)
    {
        var report = await GetServicesReportAsync(salonId, month, year);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Text("CutCal - Services & Revenue Report").FontSize(18).Bold();
                page.Content().Column(column =>
                {
                    column.Item().Text($"Most popular: {report.MostPopularService ?? "N/A"}  Total revenue: {report.TotalRevenue:C}");
                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Service").Bold();
                            header.Cell().Text("Appointments").Bold();
                            header.Cell().Text("Revenue").Bold();
                            header.Cell().Text("Avg Rating").Bold();
                        });

                        foreach (var s in report.Services)
                        {
                            table.Cell().Text(s.ServiceName);
                            table.Cell().Text(s.AppointmentCount.ToString());
                            table.Cell().Text(s.TotalRevenue.ToString("C"));
                            table.Cell().Text(s.AverageRating.ToString("F1"));
                        }
                    });
                });
                page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
            });
        });

        return document.GeneratePdf();
    }
}
