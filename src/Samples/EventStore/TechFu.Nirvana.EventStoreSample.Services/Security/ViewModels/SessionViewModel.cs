﻿using System;
using TechFu.Nirvana.Domain;

namespace TechFu.Nirvana.EventStoreSample.Services.Shared.Security.ViewModels
{
    public class SessionViewModel:ViewModel<Guid>
    {
        public string Name { get; set; }
    }
}