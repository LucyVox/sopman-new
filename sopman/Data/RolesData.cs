using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using sopman.Models;

namespace sopman.Data
{
    public static class RolesData
    {
        private static readonly string[] Roles = new string[] { "SOPSuperAdmin", "SOPAdmin", "SOPCreator", "SOPUser" };
           

    }
}
