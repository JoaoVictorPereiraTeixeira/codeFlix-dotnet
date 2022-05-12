using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using Moq;
using System;
using System.Threading;
using Xunit;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory;
[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async void DeleteCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock;
        var categoryExample = _fixture.GetExampleCategory();

        repositoryMock
            .Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryExample);

        var input = new UseCase.DeleteCategoryInput(categoryExample.Id);
        var useCase = new UseCase.DeleteCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(
            categoryExample.Id,
            It.IsAny<CancellationToken>()),
            Times.Once
        );

        repositoryMock.Verify(repository => repository.Delete(
            categoryExample,
            It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async void ThrowWhenCategoryNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock;
        var guidExample = Guid.NewGuid();

        repositoryMock
            .Setup(x => x.Get(guidExample, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"Category '{guidExample} not found'"));

        var input = new UseCase.DeleteCategoryInput(guidExample);
        var useCase = new UseCase.DeleteCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );


        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{guidExample} not found'");

        repositoryMock.Verify(repository => repository.Get(
            guidExample,
            It.IsAny<CancellationToken>()),
            Times.Once
        );

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(
            It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
