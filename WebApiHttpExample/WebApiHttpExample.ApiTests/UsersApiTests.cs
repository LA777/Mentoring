using AutoFixture;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using WebApiHttpExample.Models;
using WebApiHttpExample.Repositories;

namespace WebApiHttpExample.ApiTests;

public class UsersApiTests : TestBase, IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public UsersApiTests(CustomWebApplicationFactory factory)
    {
        // Create a client to interact with the in-memory API.
        _client = factory.CreateClient();
        _mockUsersRepository = factory.MockUsersRepository;
        jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetUserById_Returns_Correct_Data()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var user = _fixture.Build<User>()
            .With(x => x.UserId, userId)
            .Create();

        _mockUsersRepository.Setup(x => x.GetUserByIdFromDatabaseAsync(It.IsAny<int>()))
            .ReturnsAsync(user)
            .Verifiable();

        // Act
        var response = await _client.GetAsync($"/api/users/{userId}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var contentTypeHeader = response.Content.Headers.ContentType?.ToString();
        contentTypeHeader.Should().BeEquivalentTo("application/json; charset=utf-8");

        var content = await response.Content.ReadAsStringAsync();
        var userActual = JsonSerializer.Deserialize<User>(content, jsonSerializerOptions);
        userActual.Should().BeEquivalentTo(user);

        _mockUsersRepository.Verify(x=>x.GetUserByIdFromDatabaseAsync(userId), Times.Once());
    }

    [Fact]
    public async Task CreateUser_Creates_User_And_Returns_Correct_Data()
    {
        // Arrange
        var userId = _fixture.Create<int>();
        var user = _fixture.Build<User>()
            .Without(x => x.UserId)
            .Create();
        var jsonUser = JsonSerializer.Serialize(user);
        var requestContent = new StringContent(jsonUser, Encoding.UTF8, MediaTypeNames.Application.Json);
        var userExpected = JsonSerializer.Deserialize<User>(jsonUser);
        userExpected!.UserId = userId;

        _mockUsersRepository.Setup(x => x.CreateUserInDatabaseAsync(It.IsAny<User>()))
            .ReturnsAsync(userExpected)
            .Callback((User userPassed) =>
            {
                user.Should().BeEquivalentTo(userPassed);
            })
            .Verifiable();

        // Act
        var response = await _client.PostAsync($"/api/users", requestContent);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var contentTypeHeader = response.Content.Headers.ContentType?.ToString();
        contentTypeHeader.Should().BeEquivalentTo("application/json; charset=utf-8");

        var content = await response.Content.ReadAsStringAsync();
        var userActual = JsonSerializer.Deserialize<User>(content, jsonSerializerOptions);
        userActual.Should().BeEquivalentTo(userExpected);

        _mockUsersRepository.Verify(x => x.CreateUserInDatabaseAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        // Create a JSON string with an invalid DateOfBirth format to trigger ModelState.IsValid = false
        string invalidJson = "{\"userId\":1,\"firstName\":\"Invalid User\",\"dateOfBirth\":\"invalid-date\"}";
        var requestContent = new StringContent(invalidJson, Encoding.UTF8, MediaTypeNames.Application.Json);

        _mockUsersRepository.Setup(x => x.GetUserByIdFromDatabaseAsync(It.IsAny<int>()))
            .Verifiable();

        // Act
        var response = await _client.PostAsync($"/api/users", requestContent);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        _mockUsersRepository.Verify(x => x.GetUserByIdFromDatabaseAsync(It.IsAny<int>()), Times.Never());
    }
}