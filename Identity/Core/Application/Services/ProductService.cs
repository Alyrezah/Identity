using AutoMapper;
using Identity.Core.Application.Contracts;
using Identity.Core.Application.Contracts.Persistence;
using Identity.Core.Application.DTOs.Product;
using Identity.Core.Application.DTOs.Product.Validators;
using Identity.Core.Domain;

namespace Identity.Core.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IMapper _mapper;
        public ProductService(IMapper mapper, IProductRepository productRepository, IProductCategoryRepository productCategoryRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _productCategoryRepository = productCategoryRepository;
        }


        public async Task<CommandResponse> Create(CreateProductDto command)
        {
            var validator = new CreateProductValidator(_productCategoryRepository);
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

            var product = _mapper.Map<Product>(command);
            await _productRepository.Add(product);
            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }

        public async Task<ProductDto> GetBy(int id)
        {
            var product = await _productRepository.GetProductDetail(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<UpdateProductDto> GetForUpdate(int id)
        {
            var product = await _productRepository.Get(id);
            return _mapper.Map<UpdateProductDto>(product);
        }

        public async Task<List<ProductDto>> GetList()
        {
            var products = await _productRepository.GetProductList();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<CommandResponse> Update(UpdateProductDto command)
        {
            var validator = new UpdateProductValidator(_productCategoryRepository);
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

            var product = await _productRepository.Get(command.Id);
            product.Edit(command.Name, command.Price, command.Description, command.CategoryId);
            await _productRepository.Update(product);

            return new CommandResponse()
            {
                IsSuccess = true,
                Message = CommandMessages.CommandSuccess,
            };
        }
    }
}
