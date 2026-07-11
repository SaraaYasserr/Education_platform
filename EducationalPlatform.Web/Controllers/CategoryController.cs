using EducationalPlatform.Models.Entities;
using EducationalPlatform.Models.ViewModels.Category;
using EducationalPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationalPlatform.Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View(new CategoryViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _categoryService.CreateAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Could not create the category.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Category created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _categoryService.UpdateAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Could not update the category.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _categoryService.DeleteAsync(id);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Category deleted." : error;

            return RedirectToAction("Index");
        }
    }
}
