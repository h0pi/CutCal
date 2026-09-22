using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CutCal.Model.Constants;
using CutCal.Model.Requests;
using FluentValidation;

namespace CutCal.Services.Validators;

/// <summary>Shared rules, so every form states the same limits and formats in its error messages.</summary>
internal static class Rules
{
    public const int MinPasswordLength = 4;
    public const int NameMaxLength = 50;
    public const int TitleMaxLength = 100;
    public const int AddressMaxLength = 200;
    public const int TextMaxLength = 1000;
    public const int UrlMaxLength = 500;
    public const int MinServiceMinutes = 5;
    public const int MaxServiceMinutes = 480;
    public const decimal MaxServicePrice = 10000m;

    // Optional leading +, then 6-20 digits, spaces or dashes: "+38761123456", "061 123 456".
    private static readonly Regex PhonePattern = new(@"^\+?[0-9][0-9 \-]{5,19}$", RegexOptions.Compiled);
    private static readonly Regex UsernamePattern = new(@"^[A-Za-z0-9._-]+$", RegexOptions.Compiled);
    private static readonly EmailAddressAttribute EmailCheck = new();

    public static IRuleBuilderOptions<T, string?> ValidPhone<T>(this IRuleBuilder<T, string?> rule) =>
        rule.Must(phone => string.IsNullOrWhiteSpace(phone) || PhonePattern.IsMatch(phone))
            .WithMessage("Enter a valid phone number, e.g. +38761123456.");

    public static IRuleBuilderOptions<T, string?> ValidOptionalEmail<T>(this IRuleBuilder<T, string?> rule) =>
        rule.Must(email => string.IsNullOrWhiteSpace(email) || EmailCheck.IsValid(email))
            .WithMessage("Enter a valid e-mail address, e.g. name@example.com.");

    public static IRuleBuilderOptions<T, string> ValidRequiredEmail<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("E-mail is required.")
            .Must(email => EmailCheck.IsValid(email)).WithMessage("Enter a valid e-mail address, e.g. name@example.com.");

    public static IRuleBuilderOptions<T, string> ValidUsername<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Username is required.")
            .Length(3, NameMaxLength).WithMessage($"Username must be 3-{NameMaxLength} characters.")
            .Must(username => UsernamePattern.IsMatch(username ?? string.Empty)).WithMessage("Username may contain only letters, digits, dots, dashes and underscores.");

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Password is required.")
            .MinimumLength(MinPasswordLength).WithMessage($"Password must be at least {MinPasswordLength} characters.");

    public static IRuleBuilderOptions<T, string> RequiredName<T>(this IRuleBuilder<T, string> rule, string label, int maxLength = NameMaxLength) =>
        rule.NotEmpty().WithMessage($"{label} is required.")
            .MaximumLength(maxLength).WithMessage($"{label} can have at most {maxLength} characters.");
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username).ValidUsername();
        RuleFor(x => x.FirstName).RequiredName("First name");
        RuleFor(x => x.LastName).RequiredName("Last name");
        RuleFor(x => x.Email).ValidRequiredEmail();
        RuleFor(x => x.Phone).ValidPhone();
        RuleFor(x => x.Password).ValidPassword();
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}

public class UserInsertRequestValidator : AbstractValidator<UserInsertRequest>
{
    public UserInsertRequestValidator()
    {
        RuleFor(x => x.Username).ValidUsername();
        RuleFor(x => x.FirstName).RequiredName("First name");
        RuleFor(x => x.LastName).RequiredName("Last name");
        RuleFor(x => x.Email).ValidRequiredEmail();
        RuleFor(x => x.Phone).ValidPhone();
        RuleFor(x => x.Password).ValidPassword();
        RuleFor(x => x.Role).Must(role => role is RoleNames.Customer or RoleNames.Staff or RoleNames.SalonManager or RoleNames.Admin)
            .WithMessage($"Role must be one of: {RoleNames.Customer}, {RoleNames.Staff}, {RoleNames.SalonManager}, {RoleNames.Admin}.");
    }
}

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
{
    public UserUpdateRequestValidator()
    {
        RuleFor(x => x.FirstName).RequiredName("First name");
        RuleFor(x => x.LastName).RequiredName("Last name");
        RuleFor(x => x.Email).ValidRequiredEmail();
        RuleFor(x => x.Phone).ValidPhone();
        RuleFor(x => x.ProfileImageUrl).MaximumLength(Rules.UrlMaxLength);
    }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage("Enter your current password.");
        RuleFor(x => x.NewPassword).ValidPassword();
        RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage("New password and confirmation do not match.");
    }
}

public class SalonWorkingHoursUpsertRequestValidator : AbstractValidator<SalonWorkingHoursUpsertRequest>
{
    public SalonWorkingHoursUpsertRequestValidator()
    {
        RuleFor(x => x.DayOfWeek).InclusiveBetween(0, 6).WithMessage("Day of week must be between 0 (Sunday) and 6 (Saturday).");
        When(x => !x.IsClosed, () =>
        {
            RuleFor(x => x.OpenTime).NotNull().WithMessage("Opening time is required for an open day.");
            RuleFor(x => x.CloseTime).NotNull().WithMessage("Closing time is required for an open day.");
            RuleFor(x => x.CloseTime).Must((x, close) => close is null || x.OpenTime is null || close > x.OpenTime)
                .WithMessage("Closing time must be after opening time.");
        });
    }
}

public class SalonInsertRequestValidator : AbstractValidator<SalonInsertRequest>
{
    public SalonInsertRequestValidator()
    {
        RuleFor(x => x.Name).RequiredName("Salon name", Rules.TitleMaxLength);
        RuleFor(x => x.SalonCategoryId).GreaterThan(0).WithMessage("Choose a category.");
        RuleFor(x => x.CityId).GreaterThan(0).WithMessage("Choose a city.");
        RuleFor(x => x.Address).RequiredName("Address", Rules.AddressMaxLength);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(x => x.Description).MaximumLength(Rules.TextMaxLength);
        RuleFor(x => x.Phone).ValidPhone();
        RuleFor(x => x.Email).ValidOptionalEmail();
        RuleFor(x => x.ProfileImageUrl).MaximumLength(Rules.UrlMaxLength);
        RuleForEach(x => x.WorkingHours).SetValidator(new SalonWorkingHoursUpsertRequestValidator());
    }
}

public class SalonUpdateRequestValidator : AbstractValidator<SalonUpdateRequest>
{
    public SalonUpdateRequestValidator()
    {
        RuleFor(x => x.Name).RequiredName("Salon name", Rules.TitleMaxLength);
        RuleFor(x => x.SalonCategoryId).GreaterThan(0).WithMessage("Choose a category.");
        RuleFor(x => x.CityId).GreaterThan(0).WithMessage("Choose a city.");
        RuleFor(x => x.Address).RequiredName("Address", Rules.AddressMaxLength);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(x => x.Description).MaximumLength(Rules.TextMaxLength);
        RuleFor(x => x.Phone).ValidPhone();
        RuleFor(x => x.Email).ValidOptionalEmail();
        RuleFor(x => x.ProfileImageUrl).MaximumLength(Rules.UrlMaxLength);
        RuleForEach(x => x.WorkingHours).SetValidator(new SalonWorkingHoursUpsertRequestValidator());
    }
}

public class SalonGalleryInsertRequestValidator : AbstractValidator<SalonGalleryInsertRequest>
{
    public SalonGalleryInsertRequestValidator()
    {
        RuleFor(x => x.ImageUrl).RequiredName("Image URL", Rules.UrlMaxLength);
        RuleFor(x => x.Caption).MaximumLength(Rules.TitleMaxLength);
    }
}

public class SalonCategoryInsertRequestValidator : AbstractValidator<SalonCategoryInsertRequest>
{
    public SalonCategoryInsertRequestValidator() => RuleFor(x => x.Name).RequiredName("Category name", Rules.TitleMaxLength);
}

public class SalonCategoryUpdateRequestValidator : AbstractValidator<SalonCategoryUpdateRequest>
{
    public SalonCategoryUpdateRequestValidator() => RuleFor(x => x.Name).RequiredName("Category name", Rules.TitleMaxLength);
}

public class CityInsertRequestValidator : AbstractValidator<CityInsertRequest>
{
    public CityInsertRequestValidator()
    {
        RuleFor(x => x.Name).RequiredName("City name", Rules.TitleMaxLength);
        RuleFor(x => x.Country).RequiredName("Country", Rules.TitleMaxLength);
    }
}

public class CityUpdateRequestValidator : AbstractValidator<CityUpdateRequest>
{
    public CityUpdateRequestValidator()
    {
        RuleFor(x => x.Name).RequiredName("City name", Rules.TitleMaxLength);
        RuleFor(x => x.Country).RequiredName("Country", Rules.TitleMaxLength);
    }
}

public class SalonServiceInsertRequestValidator : AbstractValidator<SalonServiceInsertRequest>
{
    public SalonServiceInsertRequestValidator()
    {
        RuleFor(x => x.SalonId).GreaterThan(0).WithMessage("Choose a salon.");
        RuleFor(x => x.Name).RequiredName("Service name", Rules.TitleMaxLength);
        RuleFor(x => x.Description).MaximumLength(Rules.TextMaxLength);
        RuleFor(x => x.DurationMinutes).InclusiveBetween(Rules.MinServiceMinutes, Rules.MaxServiceMinutes)
            .WithMessage($"Duration must be between {Rules.MinServiceMinutes} and {Rules.MaxServiceMinutes} minutes.");
        RuleFor(x => x.Price).Must(price => price > 0 && price <= Rules.MaxServicePrice)
            .WithMessage($"Price must be greater than 0 and at most {Rules.MaxServicePrice:0}.");
    }
}

public class SalonServiceUpdateRequestValidator : AbstractValidator<SalonServiceUpdateRequest>
{
    public SalonServiceUpdateRequestValidator()
    {
        RuleFor(x => x.Name).RequiredName("Service name", Rules.TitleMaxLength);
        RuleFor(x => x.Description).MaximumLength(Rules.TextMaxLength);
        RuleFor(x => x.DurationMinutes).InclusiveBetween(Rules.MinServiceMinutes, Rules.MaxServiceMinutes)
            .WithMessage($"Duration must be between {Rules.MinServiceMinutes} and {Rules.MaxServiceMinutes} minutes.");
        RuleFor(x => x.Price).Must(price => price > 0 && price <= Rules.MaxServicePrice)
            .WithMessage($"Price must be greater than 0 and at most {Rules.MaxServicePrice:0}.");
    }
}

public class StaffInsertRequestValidator : AbstractValidator<StaffInsertRequest>
{
    public StaffInsertRequestValidator()
    {
        RuleFor(x => x.SalonId).GreaterThan(0).WithMessage("Choose a salon.");
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Choose the user account of the staff member.");
        RuleFor(x => x.Role).RequiredName("Role or title");
        RuleFor(x => x.Bio).MaximumLength(Rules.TextMaxLength);
        RuleFor(x => x.ProfileImageUrl).MaximumLength(Rules.UrlMaxLength);
    }
}

public class StaffUpdateRequestValidator : AbstractValidator<StaffUpdateRequest>
{
    public StaffUpdateRequestValidator()
    {
        RuleFor(x => x.Role).RequiredName("Role or title");
        RuleFor(x => x.Bio).MaximumLength(Rules.TextMaxLength);
        RuleFor(x => x.ProfileImageUrl).MaximumLength(Rules.UrlMaxLength);
    }
}

public class AppointmentInsertRequestValidator : AbstractValidator<AppointmentInsertRequest>
{
    public AppointmentInsertRequestValidator()
    {
        RuleFor(x => x.SalonId).GreaterThan(0).WithMessage("Choose a salon.");
        RuleFor(x => x.StaffId).GreaterThan(0).WithMessage("Choose a stylist.");
        RuleFor(x => x.ServiceId).GreaterThan(0).WithMessage("Choose a service.");
        RuleFor(x => x.PaymentMethod).Must(x => x is PaymentMethodNames.Cash or PaymentMethodNames.PayPal)
            .WithMessage($"Payment method must be {PaymentMethodNames.Cash} or {PaymentMethodNames.PayPal}.");
    }
}

public class ReviewInsertRequestValidator : AbstractValidator<ReviewInsertRequest>
{
    public ReviewInsertRequestValidator()
    {
        RuleFor(x => x.AppointmentId).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");
        RuleFor(x => x.Comment).MaximumLength(Rules.TextMaxLength).WithMessage($"Comment can have at most {Rules.TextMaxLength} characters.");
    }
}

public class ReviewReplyRequestValidator : AbstractValidator<ReviewReplyRequest>
{
    public ReviewReplyRequestValidator() => RuleFor(x => x.Reply).RequiredName("Reply", Rules.TextMaxLength);
}

public class AppointmentCancelRequestValidator : AbstractValidator<AppointmentCancelRequest>
{
    public AppointmentCancelRequestValidator() => RuleFor(x => x.Reason).RequiredName("Cancellation reason", Rules.TextMaxLength);
}
