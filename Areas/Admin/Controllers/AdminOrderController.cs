using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppleStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppleStore.Areas.Admin.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
    }
}