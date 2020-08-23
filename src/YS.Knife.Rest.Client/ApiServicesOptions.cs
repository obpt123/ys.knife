﻿using System;
using System.Collections.Generic;

namespace YS.Knife.Rest.Client
{
    [OptionsClass()]
    public class ApiServicesOptions
    {
        public Dictionary<string, ServiceOptions> Services { get; set; } = new Dictionary<string, ServiceOptions>();
    }
    public class ServiceOptions
    {
        public string BaseAddress { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
