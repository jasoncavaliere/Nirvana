﻿using TechFu.Nirvana.CQRS;
using TechFu.Nirvana.EventStoreSample.Services.Shared.Services.ProductCatalog.Commands;
using TechFu.Nirvana.EventStoreSample.Services.Shared.Services.ProductCatalog.InternalEvents;
using TechFu.Nirvana.Mediation;

namespace TechFu.Nirvana.EventStoreSample.Domain.Handlers.ProductCatalog.Event
{
    public class HandleCatalogUpdatedEvent : BaseEventHandler<CatalogUpdatedEvent>
    {
        public HandleCatalogUpdatedEvent(IChildMediatorFactory mediator) : base(mediator)
        {
        }

        public override InternalEventResponse Handle(CatalogUpdatedEvent internalEvent)
        {
            Mediator.Command(new UpdateHomePageViewModelCommand());
            return InternalEventResponse.Succeeded();
        }
    }
}