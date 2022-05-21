using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

[Collection(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTest
{
    private readonly CreateCategoryApiTestFixture _fixture;

    public CreateCategoryApiTest(CreateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "")]
    [Trait("EndToEnd/API", "Category Endpoints")]
    public async Task CreateCategory()
    {
        var input = _fixture.getExampleInput();

        var output = await _fixture.Api.Post<CategoryModelOutput>("/categories", input);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive().Should().Be(input.IsActive);
        output.CategoryId.NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
        DomainEntity.Category dbCategory = await _fixture.Persistence.GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive().Should().Be(input.IsActive);
        dbCategory.CategoryId.NotBeEmpty();
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default);


    }
}
