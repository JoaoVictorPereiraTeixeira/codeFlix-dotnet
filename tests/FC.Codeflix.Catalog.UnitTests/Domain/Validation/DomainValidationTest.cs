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
        Action action = () => DomainValidation.NotNull(value, "Value");
        action.Should().NotThrow();
    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validators")]
    public void NotNullThrowWhenNull()
    {
        string value = null;
        Action action = () => DomainValidation.NotNull(value, "FieldName");
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("FieldName should not be null");
    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validators")]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        Action action = () => DomainValidation.NotNullOrEmpty(target, "FieldName");
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("FieldName should not be null or empty");
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validators")]
    public void NotNullOrEmptyOk()
    {
        var target = Faker.Commerce.ProductName();

        Action action = () => DomainValidation.NotNullOrEmpty(target, "FieldName");
        action.Should()
            .NotThrow<EntityValidationException>();
    }


    [Theory(DisplayName = nameof(MinLengthThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validators")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10)]
    public void MinLengthThrowWhenLess(string target, int minLength)
    {
        Action action = () => DomainValidation.MinLength(target, minLength, "fieldName");
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"fieldName should be at least {minLength} characters long");
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

    public static IEnumerable<object[]> GetValuesGreaterThanMin(int numberOfTests = 5)
    {
        var faker = new Faker();
        for (int i = 0; i < numberOfTests; i++)
        {
            var example = faker.Commerce.ProductName();
            var minLength = example.Length - 1;
            yield return new object[] { example, minLength };
        }
    }
}
