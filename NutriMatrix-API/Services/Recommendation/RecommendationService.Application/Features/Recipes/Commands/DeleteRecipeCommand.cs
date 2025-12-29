using MediatR;
using RecommendationService.Domain.Contracts;
using RecommendationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationService.Application.Features.Recipes.Commands
{
    public class DeleteRecipeCommand : IRequest
    {
        public long Id { get; set; }
    }
    public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand>
    {
        private readonly IRepository<Recipe> _repository;

        public DeleteRecipeCommandHandler(IRepository<Recipe> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
        {
            var recipe = await _repository.Delete(request.Id);
            if (recipe == null) throw new Exception("Recipe not found");
        }
    }
}
