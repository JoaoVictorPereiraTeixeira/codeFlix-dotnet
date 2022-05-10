using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.UnitTests.Application.DeleteCategory;
using FluentAssertions;
using Moq;
using System.Threading;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        this._fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public async void UpdateCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock;
        var categoryExample = _fixture.GetExampleCategory();

        repositoryMock
            .Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryExample);

        var input = new UseCase.UpdateCategoryInput(
            categoryExample.Id,
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription(),
            !categoryExample.IsActive
        );

        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);

        repositoryMock.Verify(repository => repository.Get(
            categoryExample.Id,
            It.IsAny<CancellationToken>()),
            Times.Once
        );

        repositoryMock.Verify(repository => repository.Update(
            categoryExample,
            It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

}
