﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechFu.Nirvana.CQRS;
using TechFu.Nirvana.EventStoreSample.Services.Shared.ProductCatalog.Commands;
using TechFu.Nirvana.EventStoreSample.Services.Shared.ProductCatalog.InternalEvents;
using TechFu.Nirvana.EventStoreSample.Services.Shared.ProductCatalog.UINotifications;
using TechFu.Nirvana.Mediation;

namespace TechFu.Nirvana.EventStoreSample.Domain.Handlers.ProductCatalog.Event
{
    public class HandleCartViewModelUpdatedEvent : IEventHandler<CartViewModelUpdatedEvent>
    {
        private readonly IMediatorFactory _mediator;

        public HandleCartViewModelUpdatedEvent(IMediatorFactory mediator)
        {
            _mediator = mediator;
        }

        public InternalEventResponse Handle(CartViewModelUpdatedEvent command)
        {
            _mediator.Notification(new CartNeedsUpdateUiEvent {UserId= command.UserId});
            return InternalEventResponse.Succeeded();
        }
    }

    public class HandleCatalogUpdatedEvent: IEventHandler<CatalogUpdatedEvent>
    {
        private readonly IMediatorFactory _mediator;

        public HandleCatalogUpdatedEvent(IMediatorFactory mediator)
        {
            _mediator = mediator;
        }

        public InternalEventResponse Handle(CatalogUpdatedEvent command)
        {
            _mediator.Command(new UpdateHomePageViewModelCommand());
            return InternalEventResponse.Succeeded();
        }
    }
}
