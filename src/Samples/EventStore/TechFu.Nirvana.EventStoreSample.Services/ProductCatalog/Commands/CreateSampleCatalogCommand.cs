﻿using TechFu.Nirvana.CQRS;

namespace TechFu.Nirvana.EventStoreSample.Services.Shared.ProductCatalog.Commands
{
    [ProductCatalogRoot("CreateSampleCatalogCommand")]
    public class CreateSampleCatalogCommand : NopCommand
    {
    }
}