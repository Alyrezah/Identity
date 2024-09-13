using Identity.Core.Application.ClaimsStore;
using Identity.Core.Application.Contracts;
using Identity.Core.Application.DTOs.ProductCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class ProductCategoryController : Controller
    {
        private readonly IProductCategoryService _productCategoryService;
        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }


        // GET: ProductCategoryController
        [Authorize(Policy = ClaimTypesStore.ProductCategoriesList)]
        public async Task<ActionResult> Index()
        {
            var model = await _productCategoryService.GetList();
            return View(model);
        }

        // GET: ProductCategoryController/Details/5
        [Authorize(Policy = ClaimTypesStore.DetailProductCategory)]
        public async Task<ActionResult> Details(int id)
        {
            var model = await _productCategoryService.GetBy(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: ProductCategoryController/Create
        [Authorize(Policy = ClaimTypesStore.CreateProductCategory)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductCategoryController/Create
        [Authorize(Policy = ClaimTypesStore.CreateProductCategory)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductCategoryDto productCategory)
        {
            try
            {

                var result = await _productCategoryService.Create(productCategory);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(productCategory);
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductCategoryController/Edit/5
        [Authorize(Policy = ClaimTypesStore.EditProductCategory)]
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _productCategoryService.GetForUpdate(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: ProductCategoryController/Edit/5
        [Authorize(Policy = ClaimTypesStore.EditProductCategory)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, UpdateProductCategoryDto productCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _productCategoryService.Update(productCategory);
                    if (result.IsSuccess)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.ErrorMessages)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                return View(productCategory);
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductCategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductCategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
