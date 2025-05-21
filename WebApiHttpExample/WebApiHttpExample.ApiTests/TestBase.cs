using AutoFixture;

namespace WebApiHttpExample.ApiTests;

public abstract class TestBase
{
    protected Fixture _fixture { get; set; }

    protected TestBase()
    {
        _fixture = new Fixture();
        _fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
    }
}
