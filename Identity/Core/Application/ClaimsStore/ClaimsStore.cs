using System.Security.Claims;

namespace Identity.Core.Application.ClaimsStore
{
    public static class ClaimsStore
    {
        public static List<Claim> WebsiteClaims =
        [
            new(ClaimTypesStore.ProductCategoriesList,true.ToString()),
            new(ClaimTypesStore.CreateProductCategory,true.ToString()),
            new(ClaimTypesStore.EditProductCategory,true.ToString()),
            new(ClaimTypesStore.DetailProductCategory,true.ToString()),

            new(ClaimTypesStore.AccountsList,true.ToString()),
            new(ClaimTypesStore.DetailAccount,true.ToString()),
            new(ClaimTypesStore.Roles,true.ToString()),
            new(ClaimTypesStore.ManageUserRole,true.ToString()),
            new(ClaimTypesStore.AddClaims,true.ToString()),
            new(ClaimTypesStore.RemoveClaims,true.ToString()),

            new(ClaimTypesStore.ProductsList,true.ToString()),
            new(ClaimTypesStore.CreateProduct,true.ToString()),
            new(ClaimTypesStore.EditProduct,true.ToString()),
            new(ClaimTypesStore.DetailProduct,true.ToString()),
        ];
    }
}
