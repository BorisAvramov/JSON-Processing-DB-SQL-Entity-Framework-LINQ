using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile() 
        {
            // IMPORT

            // User
            CreateMap<ImportUserDto, User>();
            //Product
            CreateMap<ImportProductDto, Product>();
            //Category
            CreateMap<ImportCategoryDto, Category>();
            //CategoryProduct
            CreateMap<ImportCategoryProductDto, CategoryProduct>();

            // EXPORT

            //Products

            CreateMap<Product, ExportProductDto>()
                .ForMember(d => d.ProductName,
                    options => options.MapFrom(s => s.Name))
                .ForMember(d => d.ProducgtPrice,
                    options => options.MapFrom(s => s.Price))
                .ForMember(d => d.SellerName,
                    options => options.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            // SoldProduct

            CreateMap<Product, ExportSoldProductDto>()
                .ForMember(d => d.SoldProductName,
                    opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.SoldProducPrice,
                    opt => opt.MapFrom(s => s.Price))
                .ForMember(d => d.SoldProducBuyerFirstName,
                    opt => opt.MapFrom(s => s.Buyer.FirstName))
                .ForMember(d => d.SoldProducBuyerLastName,
                    opt => opt.MapFrom(s => s.Buyer.LastName));
            // UserWithSoldProducts

            CreateMap<User, ExportUserWithSoldProductsDto>()
                .ForMember(d => d.UserFirstName,
                    opt => opt.MapFrom(s => s.FirstName))
                .ForMember(d => d.UserLastName,
                    opt => opt.MapFrom(s => s.LastName))
                .ForMember(d => d.UserSoldProducts,
                    opt => opt.MapFrom(s => s.ProductsSold));

            //CategoriesWithProductsCountAvaregaPriceTotalRevenue

            CreateMap<Category, ExportCategorieWithProductsCounsAveragePriceTotalRevenue>()
                .ForMember(d => d.ProductsCount,
                    opt => opt.MapFrom(s => s.CategoriesProducts.Count))
                .ForMember(d => d.AveragePrice,
                    opt => opt.MapFrom(s => (s.CategoriesProducts.Any() 
                        ? s.CategoriesProducts.Average(p => p.Product.Price)
                        : 0).ToString("f2")))
                 .ForMember(d => d.TotalRevenue,
                    opt => opt.MapFrom(s => (s.CategoriesProducts.Any()
                        ? s.CategoriesProducts.Sum(p => p.Product.Price)
                        : 0).ToString("f2")));

        }
    }
}
