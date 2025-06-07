using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductSklepInternetowyUI.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepo;

        public UserOrderController(IUserOrderRepository userOrderRepo)
        {
            _userOrderRepo = userOrderRepo;
        }
        public async Task<IActionResult> UserOrders() //Zwraca listę zamówień danego użytkownika (zalogowanego).
        {
            var orders = await _userOrderRepo.UserOrders();
            return View(orders);
        }
    }
}
