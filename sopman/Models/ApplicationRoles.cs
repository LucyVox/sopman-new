using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace sopman.Models
{
    public class ApplicationRoles : RoleManager<IdentityRole>
    {
        public ApplicationRoles(IRoleStore<IdentityRole, string> roleStore)
        : base(roleStore) { }

        public static ApplicationRoles Create(
            IdentityFactoryOptions<ApplicationRoles> options,
            IOwinContext context)
        {
            var manager = new ApplicationRoles(
                new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }
}
