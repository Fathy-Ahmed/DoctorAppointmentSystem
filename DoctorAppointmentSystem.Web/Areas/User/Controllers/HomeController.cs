﻿using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentSystem.Web.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
