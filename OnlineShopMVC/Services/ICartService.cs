namespace OnlineShopMVC.Services
{
    public interface ICartService
    {
        Task<int> GetCartItemCountAsync(int userId);
    }
}
