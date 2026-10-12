using System.Reflection;
using CutCal.Messaging;
using CutCal.Model.Constants;
using CutCal.Model.Exceptions;
using CutCal.Model.Responses;
using CutCal.Services.Auth;
using CutCal.Services.Database;
using CutCal.Services.Messaging;
using CutCal.Services.Services;
using CutCal.WebAPI.Controllers;
using CutCal.WebAPI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace CutCal.Tests;

public class RecommenderRetrainTests
{
    private sealed class FakePublisher : IRabbitMqPublisher
    {
        public int RetrainRequests { get; private set; }
        public Exception? FailWith { get; init; }

        public Task PublishAsync(NotificationMessage message) => Task.CompletedTask;

        public Task RequestRecommenderRetrainAsync()
        {
            RetrainRequests++;
            return FailWith is null ? Task.CompletedTask : Task.FromException(FailWith);
        }
    }

    private sealed class FakeRecommendationService : IRecommendationService
    {
        public int RetrainRequests { get; private set; }

        public Task<List<RecommendationResponse>> GetRecommendationsAsync(int customerId, double? lat, double? lng) => Task.FromResult(new List<RecommendationResponse>());

        public Task RequestRetrainAsync()
        {
            RetrainRequests++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUser : IAuthenticatedUserAccessor
    {
        public int UserId => 1;
        public string? Role => RoleNames.Admin;
        public bool IsInRole(string role) => role == Role;
    }

    private static MethodInfo ControllerAction(string name) => typeof(RecommendationsController).GetMethod(name)!;

    [Fact]
    public void The_recommendations_controller_requires_a_signed_in_user()
    {
        Assert.NotNull(typeof(RecommendationsController).GetCustomAttribute<AuthorizeAttribute>());
    }

    [Fact]
    public void Only_customers_can_read_their_recommendations()
    {
        Assert.Equal(RoleNames.Customer, ControllerAction(nameof(RecommendationsController.Get)).GetCustomAttribute<AuthorizeAttribute>()!.Roles);
    }

    [Fact]
    public void Only_admins_can_request_a_retrain_and_only_by_post()
    {
        var retrain = ControllerAction(nameof(RecommendationsController.Retrain));

        Assert.Equal(RoleNames.Admin, retrain.GetCustomAttribute<AuthorizeAttribute>()!.Roles);
        Assert.NotNull(retrain.GetCustomAttribute<HttpPostAttribute>());
    }

    [Fact]
    public async Task Retrain_queues_the_request_and_answers_202()
    {
        var service = new FakeRecommendationService();
        var controller = new RecommendationsController(service, new FakeUser());

        var result = await controller.Retrain();

        Assert.IsType<AcceptedResult>(result);
        Assert.Equal(1, service.RetrainRequests);
    }

    [Fact]
    public async Task The_service_hands_the_request_to_the_publisher()
    {
        var publisher = new FakePublisher();
        await using var context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var service = new RecommendationService(context, publisher, new FixedModelProvider(null), new MemoryCache(new MemoryCacheOptions()));

        await service.RequestRetrainAsync();

        Assert.Equal(1, publisher.RetrainRequests);
    }

    [Fact]
    public async Task The_service_lets_a_broker_failure_reach_the_caller()
    {
        var publisher = new FakePublisher { FailWith = new ServiceUnavailableException("broker down") };
        await using var context = new CutCalDbContext(new DbContextOptionsBuilder<CutCalDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var service = new RecommendationService(context, publisher, new FixedModelProvider(null), new MemoryCache(new MemoryCacheOptions()));

        await Assert.ThrowsAsync<ServiceUnavailableException>(service.RequestRetrainAsync);
    }

    [Fact]
    public void An_unreachable_service_is_reported_as_503_with_its_message()
    {
        var filter = new ExceptionFilter(NullLogger<ExceptionFilter>.Instance);
        var context = new ExceptionContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>())
        {
            Exception = new ServiceUnavailableException("broker down")
        };

        filter.OnException(context);

        var result = Assert.IsType<ObjectResult>(context.Result);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, result.StatusCode);
        Assert.Equal("broker down", result.Value!.GetType().GetProperty("message")!.GetValue(result.Value));
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void The_retrain_queue_is_named_and_distinct_from_the_e_mail_queues()
    {
        Assert.Equal("cutcal.recommender.retrain", MessagingTopology.RetrainQueueName);
        Assert.DoesNotContain(MessagingTopology.RetrainQueueName, new[] { MessagingTopology.EmailQueueName, MessagingTopology.DeadLetterQueueName });
    }
}
