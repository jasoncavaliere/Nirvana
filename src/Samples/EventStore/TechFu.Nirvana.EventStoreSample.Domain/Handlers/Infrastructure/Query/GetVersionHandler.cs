﻿using TechFu.Nirvana.CQRS;
using TechFu.Nirvana.EventStoreSample.Services.Shared.Services.Infrastructure.Queries;
using TechFu.Nirvana.Mediation;

namespace TechFu.Nirvana.EventStoreSample.Domain.Handlers.Infrastructure.Query
{
    public class GetVersionHandler : QueryHandlerBase<GetVersionQuery, VersionModel>
    {
        public GetVersionHandler(IChildMediatorFactory mediator) : base(mediator)
        {
        }

        public override QueryResponse<VersionModel> Handle(GetVersionQuery query)
        {
            return QueryResponse.Success(new VersionModel {Version = "1.0"});
        }
    }
}