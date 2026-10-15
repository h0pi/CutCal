using CutCal.Model.Constants;
using CutCal.Model.Enums;
using CutCal.Model.Requests;
using CutCal.Services.Database;
using CutCal.Services.Validators;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Tests;

public class StripePaymentModelTests
{
    private static AppointmentInsertRequest Booking(string paymentMethod) => new()
    {
        SalonId = 1,
        StaffId = 1,
        ServiceId = 1,
        PaymentMethod = paymentMethod
    };

    [Fact]
    public void Every_payment_method_name_is_a_member_of_the_enum_and_the_other_way_round()
    {
        var names = typeof(PaymentMethodNames).GetFields().Select(x => (string)x.GetRawConstantValue()!).Order();

        Assert.Equal(Enum.GetNames<PaymentMethod>().Order(), names);
        Assert.Equal("Stripe", PaymentMethodNames.Stripe);
    }

    [Theory]
    [InlineData(PaymentMethodNames.Cash)]
    [InlineData(PaymentMethodNames.PayPal)]
    [InlineData(PaymentMethodNames.Stripe)]
    public void An_appointment_can_be_booked_with_cash_PayPal_or_Stripe(string paymentMethod)
    {
        Assert.True(new AppointmentInsertRequestValidator().Validate(Booking(paymentMethod)).IsValid);
    }

    [Fact]
    public void Any_other_payment_method_is_rejected_with_a_message_that_lists_the_three()
    {
        var result = new AppointmentInsertRequestValidator().Validate(Booking("Bitcoin"));

        Assert.False(result.IsValid);
        Assert.Equal("Payment method must be Cash, PayPal or Stripe.", Assert.Single(result.Errors).ErrorMessage);
    }

    [Fact]
    public void The_migrations_cover_the_whole_model()
    {
        // EF refuses to start the API when the model has changes no migration describes; no database is needed to find that out.
        using var context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseSqlServer("Server=none;Database=none").Options);

        Assert.False(context.Database.HasPendingModelChanges());
    }
}
