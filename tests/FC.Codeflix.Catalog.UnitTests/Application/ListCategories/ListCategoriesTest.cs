using FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

[Collection(nameof(ListCategoriesTest))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;
    public ListCategoriesTest(ListCategoriesTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCategories - Use Cases")]
    public async Task List()
    {
        var categoriesExampleList = _fixture.GetExampleCategoriesList()
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = new ListCategoriesInput(
            page: 2,
            perPage: 15,
            search: "search-example",
            sort: "name",
            dir: SearchOrder.Asc
        );
        var outputRepositorySearch = new OutputSearch<Category>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: (IReadOnlyList<Category>)categoriesExampleList,
            total: 70
        );

        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                SearchInput.Page == input.Page
                && SearchInput.PerPage == input.PerPage
                && SearchInput.Search == input.Search
                && SearchInput.OrderBy == input.Sort
                && SearchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);

        var useCase = new ListCategories(repositoryMock.Object);
        var output = await useCase.Handle(input, CancellationToken.None);
        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.currentPage);
        output.PerPage.Should().Be(outputRepositorySearch.perPage);
        output.Count.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        output.Items.ForEach(outputItem =>
        {
            var repositoryCategory = outputRepositorySearch.Items.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(repositoryCategory.Name);
            outputItem.Description.Should().Be(repositoryCategory.Description);
            outputItem.IsActive.Should().Be(repositoryCategory.IsActive);
            outputItem.Id.Should().Be(repositoryCategory.Id);
            outputItem.CreatedAt.Should().Be(repositoryCategory.CreatedAt);
        });

        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
                SearchInput.Page == input.Page
                && SearchInput.PerPage == input.PerPage
                && SearchInput.Search == input.Search
                && SearchInput.OrderBy == input.Sort
                && SearchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
         ), Times.Once);
    }
}
