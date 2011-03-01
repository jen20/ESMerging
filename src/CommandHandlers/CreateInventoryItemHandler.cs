using System;
using Commands;
using Domain;

namespace CommandHandlers
{
    public class CreateInventoryItemHandler : ICommandHandler<CreateInventoryItem>
    {
        private readonly IRepository _repository;

        public CreateInventoryItemHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CreateInventoryItem command, CommandExecutionContext context)
        {

        }
    }
}