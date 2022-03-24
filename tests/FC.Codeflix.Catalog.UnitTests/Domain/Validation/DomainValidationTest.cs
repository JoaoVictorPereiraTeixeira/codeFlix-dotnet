using Bogus;
using Xunit;
using FC.Codeflix.Catalog.Domain.Validation;
using System;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using System.Collections.Generic;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validators")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName;
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action = () => DomainValidation.NotNull(value, fieldName);
        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validators")]
    public void NotNullThrowWhenNull()
    {
        string value = null;
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action = () => DomainValidation.NotNull(value, fieldName);
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null");
    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validators")]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be empty or null");
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validators")]
    public void NotNullOrEmptyOk()
    {
        var target = Faker.Commerce.ProductName();
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should()
            .NotThrow<EntityValidationException>();
    }

    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validators")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10)]
    public void MinLengthThrowWhenLess(string target, int minLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.MinLength(target, minLength, fieldName);

        action.Should()
            .Throw<EntityValidationException>()//should be at least 3 characters long
            .WithMessage($"{fieldName} should be at least {minLength} characters long");
    }

    [Theory(DisplayName = nameof(MaxLengthThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validators")]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10)]
    public void MaxLengthThrowWhenGreater(string target, int maxLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be less or equal {maxLength} characters long");
    }

    [Theory(DisplayName = nameof(MaxLengthOk))]
    [Trait("Domain", "DomainValidation - Validators")]
    [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
    public void MaxLengthOk(string target, int maxLength)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.MaxLength(target, maxLength, fieldName);

        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLengthOk))]
    [Trait("Domain", "DomainValidation - Validators")]
    [MemberData(nameof(GetValuesGreaterThanMin), parameters: 10)]
    public void MinLengthOk(string target, int minLength)
    {
        Action action = () => DomainValidation.MinLength(target, minLength, "fieldName");
        action.Should()
            .NotThrow<EntityValidationException>();
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 10 };
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length + (new Random()).Next(1, 20);
            yield return new object[] { example, minLength };
        }
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 5 };
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length - (new Random()).Next(1, 5);
            yield return new object[] { example, maxLength };
        }
    }

    public static IEnumerable<object[]> GetValuesLessThanMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLength = example.Length + (new Random()).Next(0, 5);
            yield return new object[] { example, maxLength };
        }
    }
}
