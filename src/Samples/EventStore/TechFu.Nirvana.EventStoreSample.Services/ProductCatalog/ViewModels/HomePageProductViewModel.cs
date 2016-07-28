﻿using System;
using TechFu.Nirvana.Domain;

namespace TechFu.Nirvana.EventStoreSample.Services.Shared.ProductCatalog.ViewModels
{
    public class HomePageProductViewModel : ViewModel<Guid>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
    }
}