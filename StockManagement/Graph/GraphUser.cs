﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Graph
{
    public class GraphUser
    {
        public string Id { get; set; }

        public string[] BusinessPhones { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }
        public string MobilePhone { get; set; }
        public object OfficeLocation { get; set; }
        public string PreferredLanguage { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
    }
}