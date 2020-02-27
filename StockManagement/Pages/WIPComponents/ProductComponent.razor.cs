using Microsoft.AspNetCore.Components;
using StockManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Pages.WIPComponents
{
    public class ProductComponentBase : ComponentBase
    {
        [Parameter]
        public Product Product { get; set; }

    }
}
