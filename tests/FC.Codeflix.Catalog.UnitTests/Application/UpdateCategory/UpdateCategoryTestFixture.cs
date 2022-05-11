using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using Moq;
using System;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { };

public class UpdateCategoryTestFixture : BaseFixture
{
    public Category GetExampleCategory() => new(
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        GetRandomBoolean()
    );
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3) categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255) categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();

        if (categoryDescription.Length > 10_000) categoryDescription = categoryDescription[..10_000];
        return categoryDescription;
    }

    public bool GetRandomBoolean() => (new Random()).NextDouble() < 0.5;

    public UpdateCategoryInput GetValidInput(Guid? id = null) => new(
        id ?? Guid.NewGuid(),
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        GetRandomBoolean()
    );

    public UpdateCategoryInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetValidInput();
        invalidInputShortName.Name = invalidInputShortName.Name.Substring(0, 2);
        return invalidInputShortName;
    }

    public UpdateCategoryInput GetInvalidInputTooLongName()
    {
        var invalidInputTooLongName = GetValidInput();
        invalidInputTooLongName.Name = Faker.Lorem.Letter(256);
        return invalidInputTooLongName;
    }

    public UpdateCategoryInput GetInvalidInputTooLongDescription()
    {
        var invalidInputsTooLongDescription = GetValidInput();
        invalidInputsTooLongDescription.Description = Faker.Lorem.Letter(10001);
        return invalidInputsTooLongDescription;
    }

    public Mock<ICategoryRepository> GetRepositoryMock => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock => new();
}
