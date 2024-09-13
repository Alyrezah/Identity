using Identity.Core.Application.ClaimsStore;
using Identity.Core.Application.Contracts;
using Identity.Core.Application.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        public ProductController(IProductService productService, IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
        }


        // GET: ProductController
        [Authorize(Policy = ClaimTypesStore.ProductsList)]
        public async Task<ActionResult> Index()
        {
            var model = await _productService.GetList();
            return View(model);
        }

        // GET: ProductController/Details/5
        [Authorize(Policy = ClaimTypesStore.DetailProduct)]
        public async Task<ActionResult> Details(int id)
        {
            var model = await _productService.GetBy(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // GET: ProductController/Create
        [Authorize(Policy = ClaimTypesStore.CreateProduct)]
        public async Task<ActionResult> Create()
        {
            return View(new CreateProductDto()
            {
                SelectCategory = await FillSelectCategory()
            });
        }

        // POST: ProductController/Create
        [Authorize(Policy = ClaimTypesStore.CreateProduct)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductDto product)
        {
            try
            {
                var result = await _productService.Create(product);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                product.SelectCategory = await FillSelectCategory();
                return View(product);
            }
            catch
            {
                product.SelectCategory = await FillSelectCategory();
                return View();
            }
        }

        // GET: ProductController/Edit/5
        [Authorize(Policy = ClaimTypesStore.EditProduct)]
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _productService.GetForUpdate(id);
            if (model == null)
            {
                return NotFound();
            }
            model.SelectCategory = await FillSelectCategory();
            return View(model);
        }

        // POST: ProductController/Edit/5
        [Authorize(Policy = ClaimTypesStore.EditProduct)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, UpdateProductDto product)
        {
            try
            {
                var result = await _productService.Update(product);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                product.SelectCategory = await FillSelectCategory();
                return View(product);
            }
            catch
            {
                product.SelectCategory = await FillSelectCategory();
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductController/Delete/5
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

        private async Task<SelectList> FillSelectCategory()
        {
            var productCategories = await _productCategoryService.GetList();
            return new SelectList(productCategories, "Id", "Name");
        }
    }
}
