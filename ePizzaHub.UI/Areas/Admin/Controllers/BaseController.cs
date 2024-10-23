﻿using ePizzaHub.Models;
using ePizzaHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ePizzaHub.UI.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    [Area("Admin")]
    public class BaseController : Controller
    {
        public UserModel CurrentUser
        {
            get
            {
                if (User.Claims.Count() > 0)
                {
                    string userData = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.UserData).Value;
                    return JsonSerializer.Deserialize<UserModel>(userData);
                }
                return null;
            }
        }
    }
}
