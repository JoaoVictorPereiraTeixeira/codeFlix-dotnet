using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using System;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTest.Api.Category.UpdateCategory
{

    [CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
    public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture> { }

    public class UpdateCategoryApiTestFixture: CategoryBaseFixture
    {
        public UpdateCategoryInput GetExampleInput(Guid? id = null) 
            => new UpdateCategoryInput
                (
                    id ?? Guid.NewGuid(),
                    GetValidCategoryName(),
                    GetValidCategoryDescription(),
                    GetRandomBoolean()
                );
    }
}
