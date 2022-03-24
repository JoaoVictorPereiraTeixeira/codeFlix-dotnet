using FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using System;
using System.Threading;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.CreateCategory;
namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTest
{
    [Fact(DisplayName = nameof(CreateCategory)]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);

        var input = CreateCategoryInput(
            "category name",
            "category description",
            true
        );            
        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Create(
                It.IsAny<Category>(), 
                It.IsAny<CancellationToken>()), 
                Times.Once
        );

        unitOfWorkMock.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()), Times.Once);

        output.ShouldNotBeNull();
        output.Name.Should().Be("category name");
        output.Description.Should().Be("category description");
        output.IsActive.Should().Be(true);
        (output.Id != null && output.Id != Guid.Empty).Should().Be(true);
        (output.CreatedAt != null && output.CreatedAt != default(DateTime)).Should().Be(true);

    }
}
