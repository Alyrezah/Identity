using AutoMapper;
using Identity.Core.Application.Contracts;
using Identity.Core.Application.Contracts.Persistence;
using Identity.Core.Application.DTOs.ProductCategory;
using Identity.Core.Application.DTOs.ProductCategory.Validators;
using Identity.Core.Domain;

namespace Identity.Core.Application.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IMapper _mapper;
        public ProductCategoryService(IProductCategoryRepository productCategoryRepository, IMapper mapper)
        {
            _productCategoryRepository = productCategoryRepository;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Create(CreateProductCategoryDto command)
        {
            var validator = new CreateProductCategoryValidator();
            var validatorResult = await validator.ValidateAsync(command);

            if (!validatorResult.IsValid)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList()
                };
            }

            var productCategory = _mapper.Map<ProductCategory>(command);
            await _productCategoryRepository.Add(productCategory);

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess
            };
        }

        public async Task<ProductCategoryDto> GetBy(int id)
        {
            var productCategory = await _productCategoryRepository.Get(id);
            return _mapper.Map<ProductCategoryDto>(productCategory);
        }

        public async Task<UpdateProductCategoryDto> GetForUpdate(int id)
        {
            var productCategory = await _productCategoryRepository.Get(id);
            return _mapper.Map<UpdateProductCategoryDto>(productCategory);
        }

        public async Task<List<ProductCategoryDto>> GetList()
        {
            var productCategories = await _productCategoryRepository.GetAll();
            return _mapper.Map<List<ProductCategoryDto>>(productCategories);
        }

        public async Task<CommandResponse> Update(UpdateProductCategoryDto command)
        {
            var validator = new UpdateProductCategoryValidator();
            var validatorResult = await validator.ValidateAsync(command);

            if (!validatorResult.IsValid)
            {
                return new CommandResponse()
                {
                    IsSuccess = false,
                    Message = CommandMessages.CommandFailer,
                    ErrorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList()
                };
            }

            var productCategory = await _productCategoryRepository.Get(command.Id);
            productCategory.Edit(command.Name);
            await _productCategoryRepository.Update(productCategory);

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess
            };
        }
    }
}
