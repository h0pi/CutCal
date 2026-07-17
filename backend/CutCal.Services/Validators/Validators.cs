using CutCal.Model.Requests;
using FluentValidation;

namespace CutCal.Services.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}

public class AppointmentInsertRequestValidator : AbstractValidator<AppointmentInsertRequest>
{
    public AppointmentInsertRequestValidator()
    {
        RuleFor(x => x.SalonId).GreaterThan(0);
        RuleFor(x => x.StaffId).GreaterThan(0);
        RuleFor(x => x.ServiceId).GreaterThan(0);
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.UtcNow).WithMessage("Appointment must be scheduled in the future.");
        RuleFor(x => x.PaymentMethod).NotEmpty().Must(x => x is "Cash" or "PayPal").WithMessage("Payment method must be Cash or PayPal.");
    }
}

public class ReviewInsertRequestValidator : AbstractValidator<ReviewInsertRequest>
{
    public ReviewInsertRequestValidator()
    {
        RuleFor(x => x.AppointmentId).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}

public class AppointmentCancelRequestValidator : AbstractValidator<AppointmentCancelRequest>
{
    public AppointmentCancelRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Cancellation reason is required.");
    }
}
