using FC.Codeflix.Catalog.Domain.Exceptions;
using System;
using System.Linq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

    public class CategoryTest
    {
        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain","Category - Aggregates")]
        public void Instantiate()
        {
            var validData = new
            {
                Name = "category name",
                Description = "category description"
            };
            var datetimeBefore = DateTime.Now;

            var category = new DomainEntity.Category(validData.Name, validData.Description);
            var datetimeAfter = DateTime.Now;

            Assert.NotNull(category);
            Assert.Equal(validData.Name, category.Name);
            Assert.Equal(validData.Description, category.Description);
            Assert.NotEqual(default(Guid), category.Id);
            Assert.NotEqual(default(DateTime), category.CreatedAt);
            Assert.True(category.CreatedAt > datetimeBefore);
            Assert.True(category.CreatedAt < datetimeAfter);
            Assert.True(category.IsActive);
        }

        [Theory(DisplayName = nameof(InstantiateWithIsActive))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstantiateWithIsActive(bool isActive)
        {
            var validData = new
            {
                Name = "category name",
                Description = "category description"
            };
            var datetimeBefore = DateTime.Now;

            var category = new DomainEntity.Category(validData.Name, validData.Description, isActive);
            var datetimeAfter = DateTime.Now;

            Assert.NotNull(category);
            Assert.Equal(validData.Name, category.Name);
            Assert.Equal(validData.Description, category.Description);
            Assert.NotEqual(default(Guid), category.Id);
            Assert.NotEqual(default(DateTime), category.CreatedAt);
            Assert.True(category.CreatedAt > datetimeBefore);
            Assert.True(category.CreatedAt < datetimeAfter);
            Assert.Equal(isActive, category.IsActive);
        }

        [Theory(DisplayName = nameof(InstantiateThrowErrorWhenNameIsEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("      ")]
        public void InstantiateThrowErrorWhenNameIsEmpty(string? name)
        {
            Action action = () => new DomainEntity.Category(name!, "category description");
            var exception = Assert.Throws<EntityValidationException>(action);
            Assert.Equal("Name should not be empty or null", exception.Message);           
        }


        [Theory(DisplayName = nameof(InstantiateThrowErrorWhenDescriptionIsEmpty))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("      ")]
        public void InstantiateThrowErrorWhenDescriptionIsEmpty(string? description)
        {
            Action action = () => new DomainEntity.Category("category nane", description);
            var exception = Assert.Throws<EntityValidationException>(action);
            Assert.Equal("Description should not be empty or null", exception.Message);
        }

        [Theory(DisplayName = nameof(InstantiateThrowErrorWhenNameIsLessThant3Caracters))]
        [Trait("Domain", "Category - Aggregates")]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("12")]
        public void InstantiateThrowErrorWhenNameIsLessThant3Caracters(string invalidName)
        {
            Action action = () => new DomainEntity.Category(invalidName, "category ok description");
            var exception = Assert.Throws<EntityValidationException>(action);

            Assert.Equal("Name should be at least 3 characters long", exception.Message);
        }

        [Fact(DisplayName = nameof(InstantiateThrowErrorWhenNameIsGreaterThant255Characters))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateThrowErrorWhenNameIsGreaterThant255Characters()
        {
            var invalidName = String.Join(null, Enumerable.Range(1,256).Select(_ => "a").ToArray());
            
            Action action = () => new DomainEntity.Category(invalidName, "category ok description");
            var exception = Assert.Throws<EntityValidationException>(action);

            Assert.Equal("Name should be less ou equal 255 characters long", exception.Message);
        }

        [Fact(DisplayName = nameof(InstantiateThrowErrorWhenDescriptionIsGreaterThant1000Characters))]
        [Trait("Domain", "Category - Aggregates")]
        public void InstantiateThrowErrorWhenDescriptionIsGreaterThant1000Characters()
        {
            var invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
            
            Action action = () => new DomainEntity.Category("category ok name", invalidDescription);
            var exception = Assert.Throws<EntityValidationException>(action);

            Assert.Equal("Description should be less ou equal 10.000 characters long", exception.Message);
        }
}