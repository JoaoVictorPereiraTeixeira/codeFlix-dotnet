using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Application.DeleteCategory;
using FluentAssertions;
using Moq;
using System;
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

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters:10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void UpdateCategory(Category categoryExample, UseCase.UpdateCategoryInput input)
    {
        var repositoryMock = _fixture.GetRepositoryMock;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock;

        repositoryMock
            .Setup(x => x.Get(categoryExample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoryExample);

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

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    public async void ThrowWhenCategoryNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock;
        var input = _fixture.GetValidInput();

        repositoryMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"category '{input.Id} not found'"));

        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(repository => repository.Get(
            input.Id,
            It.IsAny<CancellationToken>()),
            Times.Once
        );

        repositoryMock.Verify(repository => repository.Update(
            It.IsAny<Category>(),
            It.IsAny<CancellationToken>()),
            Times.Never
        );

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(
            It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

}
