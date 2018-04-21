using KioskApp.Models;

namespace KioskApp.ViewModels
{
    public interface IProductListItemViewModel
    {
        Product Product { get; set; }
        int Quantity { get; set; }
    }
}