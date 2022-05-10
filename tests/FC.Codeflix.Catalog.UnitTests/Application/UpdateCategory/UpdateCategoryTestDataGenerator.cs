using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using System.Collections;
using System.Collections.Generic;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;
public class UpdateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();

        for(int i = 0; i < times; i++)
        {
            var categoryExample = fixture.GetExampleCategory();
            var inputExample = fixture.GetValidInput(categoryExample.Id);
            yield return new object[] { categoryExample, inputExample };
        }
    }
}
