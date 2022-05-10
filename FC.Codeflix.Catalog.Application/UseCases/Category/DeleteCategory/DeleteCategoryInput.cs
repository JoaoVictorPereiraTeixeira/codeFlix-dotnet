using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
public class DeleteCategoryInput : IRequest
{

    public Guid Id { get; set; }
    public DeleteCategoryInput(Guid id)
    {
        Id = id;
    }
}
