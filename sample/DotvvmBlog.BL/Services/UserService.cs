using System;
using System.Security.Claims;
using DotvvmBlog.BL.DTO;
using DotvvmBlog.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DotvvmBlog.BL.Services
{
    public class UserService : BaseService
    {
        public ClaimsIdentity SignIn(SignInDTO data)
        {
            using (var dc = CreateDbContext())
            {
                using (var userManager = CreateUserManager(dc))
                {
                    var user = userManager.Find(data.UserName, data.Password);
                    if (user == null)
                    {
                        // if the user doesn't have the password and signs in for the first time, save it
                        user = userManager.FindByName(data.UserName);
                        if (!userManager.HasPassword(user.Id))
                        {
                            userManager.ResetPassword(user.Id, userManager.GeneratePasswordResetToken(user.Id), data.Password);
                            dc.SaveChanges();
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                }
            }
        }

        private UserManager<IdentityUser, string> CreateUserManager(BlogDbContext dc)
        {
            return new UserManager<IdentityUser, string>(new UserStore<IdentityUser>(dc))
            {
                UserTokenProvider = new TotpSecurityStampBasedTokenProvider<IdentityUser, string>()
            };
        }
    }
}