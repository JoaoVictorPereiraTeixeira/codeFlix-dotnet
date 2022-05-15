using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using System.Threading;
using Xunit;
using Entity = FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture) => _fixture = fixture;

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void UpdateCategory(Entity.Category categoryExample, UpdateCategoryInput input)
    {
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trakingInfo = await dbContext.AddAsync(categoryExample);
        dbContext.SaveChanges();
        trakingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(
            repository,
            unitOfWork
        );

        var output = await useCase.Handle(input, CancellationToken.None);
        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool) input.IsActive!);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive);
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void UpdateCategoryWithoutIsActive(Entity.Category categoryExample, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name, exampleInput.Description);
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trakingInfo = await dbContext.AddAsync(categoryExample);
        dbContext.SaveChanges();
        trakingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(
            repository,
            unitOfWork
        );

        var output = await useCase.Handle(input, CancellationToken.None);
        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)categoryExample.IsActive!);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)categoryExample.IsActive);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
     )]
    public async void UpdateCategoryOnlyName(Entity.Category categoryExample, UpdateCategoryInput exampleInput)
    {
        var input = new UpdateCategoryInput(exampleInput.Id, exampleInput.Name);
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        var trakingInfo = await dbContext.AddAsync(categoryExample);
        dbContext.SaveChanges();
        trakingInfo.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(
            repository,
            unitOfWork
        );

        var output = await useCase.Handle(input, CancellationToken.None);
        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(categoryExample.Description);
        dbCategory.IsActive.Should().Be((bool)categoryExample.IsActive!);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(categoryExample.Description);
        output.IsActive.Should().Be((bool)categoryExample.IsActive);
    }

    [Fact]
    public async void UpdateThrowsWhenNotFoundCategory()
    {
        var input = _fixture.GetValidInput();
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoriesList());
        dbContext.SaveChanges();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(
            repository,
            unitOfWork
        );

        var task = async () => await useCase.Handle(input, CancellationToken.None);
        await task.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Category '{input.Id}' not found.");        
    }

    [Theory(DisplayName = nameof(UpdateThrowsWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
       nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
       parameters: 6,
       MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async void UpdateThrowsWhenCantInstantiateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategories = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(exampleCategories);
        dbContext.SaveChanges();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UseCase.UpdateCategory(repository,unitOfWork);
        input.Id = exampleCategories[0].Id;

        var task = async () => await useCase.Handle(input, CancellationToken.None);
        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);
    }
}
