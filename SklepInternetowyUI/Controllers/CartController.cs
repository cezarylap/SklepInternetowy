using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductSklepInternetowyUI.Controllers
{
    [Authorize] //tylko zalogowaniu użytkownicy
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepo;

        public CartController(ICartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> AddItem(int ProductId, int qty = 1, int redirect = 0) //Dodaje produkt do koszyka.
        {
            var cartCount = await _cartRepo.AddItem(ProductId, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int ProductId) //Usuwa produkt z koszyka.
        {
            var cartCount = await _cartRepo.RemoveItem(ProductId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart() //Zwraca zawartość koszyka użytkownika.
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        public  async Task<IActionResult> GetTotalItemInCart() // liczbę przedmiotów w koszyku.
        {
            int cartItem = await _cartRepo.GetCartItemCount();
            return Ok(cartItem);
        }

        public  IActionResult Checkout() //Obsługuje proces składania zamówienia.
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            bool isCheckedOut = await _cartRepo.DoCheckout(model);
            if (!isCheckedOut)
                return RedirectToAction(nameof(OrderFailure));
            return RedirectToAction(nameof(OrderSuccess));
        }

        public IActionResult OrderSuccess() //Widok sukcesu składania zamówienia
        {
            return View();
        }

        public IActionResult OrderFailure() //Widok błędu składania zamówienia
        {
            return View();
        }

    }
}
