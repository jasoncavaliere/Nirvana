﻿using System;
using TechFu.Nirvana.CQRS;
using TechFu.Nirvana.EventStoreSample.Services.Shared.Services.Infrastructure.Commands;
using TechFu.Nirvana.EventStoreSample.Services.Shared.Services.Infrastructure.UiNotifications;
using TechFu.Nirvana.Mediation;

namespace TechFu.Nirvana.EventStoreSample.Domain.Handlers.Infrastructure.Command
{
    public class TestHandler : BaseCommandHandler<TestCommand, TestResult>
    {
        public TestHandler(IChildMediatorFactory mediator) : base(mediator)
        {
        }

        public override CommandResponse<TestResult> Handle(TestCommand task)
        {
            Mediator.Notification(new TestUiEvent {Message = $"UI NOtification from {Guid.NewGuid()}"});

            return CommandResponse.Succeeded(new TestResult
            {
                Message = "Worked"
            });
        }
    }
}