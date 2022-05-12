using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using Entity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void UpdateCategory(Entity.Category categoryExample, UpdateCategoryInput input)
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
        output.IsActive.Should().Be((bool)input.IsActive);

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

    [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActive))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void UpdateCategoryWithoutProvidingIsActive(Entity.Category categoryExample, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name,
            exampleInput.Description
        );

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
        output.IsActive.Should().Be((bool)categoryExample.IsActive!);

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

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void UpdateCategoryOnlyName(Entity.Category categoryExample, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name
        );

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
        output.Description.Should().Be(categoryExample.Description);
        output.IsActive.Should().Be((bool)categoryExample.IsActive!);

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
            It.IsAny<Entity.Category>(),
            It.IsAny<CancellationToken>()),
            Times.Never
        );

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(
            It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
    [Trait("Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
    {
        var exempleCategory = _fixture.GetExampleCategory();
        input.Id = exempleCategory.Id;

        var repositoryMock = _fixture.GetRepositoryMock;
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock;

        repositoryMock
            .Setup(x => x.Get(exempleCategory.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exempleCategory);

        var useCase = new UseCase.UpdateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);

        repositoryMock.Verify(x => x.Get(
            exempleCategory.Id,
            It.IsAny<CancellationToken>()),
            Times.Once
        );

    }

}
