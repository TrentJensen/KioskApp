using KioskApp.Models;
using KioskApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KioskApp.Components
{
    public class CategoryMenu : ViewComponent
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager; 

        public CategoryMenu(ICategoryRepository categoryRepository, UserManager<ApplicationUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            //Only show products dropdown if the user is logged in
            var userGuid = _userManager.GetUserId(HttpContext.User);

            var categories = _categoryRepository.Categories.OrderBy(c => c.CategoryName);
            CategoriesViewModel categoryViewModel = new CategoriesViewModel
            {
                Categories = categories,
                UserGuid = userGuid
            };

            return View(categoryViewModel);
        }
    }
}
