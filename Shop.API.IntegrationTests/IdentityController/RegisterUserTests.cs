namespace Shop.API.IntegrationTests.IdentityController;

using API.Models.Requests;

public class RegisterUserTests : TestBase
{
    private readonly Faker<RegisterUserRequest> _faker;

    public RegisterUserTests(ShopApiFactory factory) : base(factory)
    {
        _faker = new Faker<RegisterUserRequest>()
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Password, _ => "Abc123!");
    }

    [Fact]
    public async Task Register_ShouldReturn200_WhenRequestValid()
    {
        var request = _faker.Generate();
        
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var cookie = response.Headers
            .FirstOrDefault(h => h.Key == "Set-Cookie").Value
            .FirstOrDefault(c => c.StartsWith("token="));

        cookie.Should().NotBeNull();
        cookie.Should().Contain("httponly");
    }

    [Fact]
    public async Task Register_ShouldReturn400_WhenFirstNameInvalid()
    {
        var request = _faker.Clone().RuleFor(r => r.FirstName, "").Generate();
        
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Register_ShouldReturn400_WhenLastNameInvalid()
    {
        var request = _faker.Clone().RuleFor(r => r.LastName, "").Generate();
        
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Register_ShouldReturn400_WhenEmailInvalid()
    {
        var request = _faker.Clone().RuleFor(r => r.Email, "").Generate();
        
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Register_ShouldReturn400_WhenPasswordInvalid()
    {
        var request = _faker.Clone().RuleFor(r => r.Password, "").Generate();
        
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}