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
                        if (user != null && !userManager.HasPassword(user.Id))
                        {
                            var result = userManager.ResetPassword(user.Id, userManager.GeneratePasswordResetToken(user.Id), data.Password);
                            if (!result.Succeeded)
                            {
                                throw new Exception("The password is too short!");
                            }
                            userManager.Update(user);
                            dc.SaveChanges();
                        }
                        else
                        {
                            throw new Exception("Invalid user name or password!");
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