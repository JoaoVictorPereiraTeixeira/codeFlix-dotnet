using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;
    
[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{
    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
    {
        _categoryTestFixture = categoryTestFixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain","Category - Aggregates")]
    public void Instantiate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime)); 
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        category.IsActive.Should().BeTrue();
    }   

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        category.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateThrowErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void InstantiateThrowErrorWhenNameIsEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(name!, validCategory.Description);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(InstantiateThrowErrorWhenDescriptionIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void InstantiateThrowErrorWhenDescriptionIsEmpty(string? description)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(validCategory.Name, description);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should not be empty or null");        
    }

    [Theory(DisplayName = nameof(InstantiateThrowErrorWhenNameIsLessThant3Caracters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("12")]
    public void InstantiateThrowErrorWhenNameIsLessThant3Caracters(string invalidName)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateThrowErrorWhenNameIsGreaterThant255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateThrowErrorWhenNameIsGreaterThant255Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var invalidName = String.Join(null, Enumerable.Range(1,256).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less ou equal 255 characters long");
    }

    [Fact(DisplayName = nameof(InstantiateThrowErrorWhenDescriptionIsGreaterThant1000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateThrowErrorWhenDescriptionIsGreaterThant1000Characters()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less ou equal 10.000 characters long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();

        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, true);
        category.Deactivate();
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        var category = _categoryTestFixture.GetValidCategory();

        var newValues = new { Name = "New name", Description = "New Description" };

        category.Update(newValues.Name, newValues.Description);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newValues = new { Name = "New name"};
        var currentDescription = category.Description;

        category.Update(newValues.Name);

        category.Name.Should().Be(newValues.Name);
        category.Description.Should().Be(currentDescription);
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();

        Action action = () => category.Update(name!, "category description");
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");

    }

    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Caracters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("12")]
    public void UpdateErrorWhenNameIsLessThan3Caracters(string invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();

        Action action = () => category.Update(invalidName, "category ok description");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }


    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());

        Action action = () => category.Update(invalidName, "category ok description");

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name should be less ou equal 255 characters long");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10000Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();

        var invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        Action action = () => category.Update("category ok name", invalidDescription);
        action.Should().Throw<EntityValidationException>()
            .WithMessage("Description should be less ou equal 10.000 characters long");
    }
}