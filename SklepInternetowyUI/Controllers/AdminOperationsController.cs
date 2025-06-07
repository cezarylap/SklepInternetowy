using ProductSklepInternetowyUI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductSklepInternetowyUI.Controllers;

[Authorize(Roles = nameof(Roles.Admin))] //dostep tylko dla administratorów
public class AdminOperationsController : Controller
{
    private readonly IUserOrderRepository _userOrderRepository;
    public AdminOperationsController(IUserOrderRepository userOrderRepository)
    {
        _userOrderRepository = userOrderRepository;
    }

    public async Task<IActionResult> AllOrders() //Pobiera wszystkie zamówienia (w tym również zrealizowane) i przekazuje je do widoku.
    {
        var orders = await _userOrderRepository.UserOrders(true);
        return View(orders);
    }

    public async Task<IActionResult> TogglePaymentStatus(int orderId) //Zmienia status opłacenia zamówienia.
    {
        try
        {
            await _userOrderRepository.TogglePaymentStatus(orderId);
        }
        catch (Exception ex)
        {
            // możliwy wyjątek
        }
        return RedirectToAction(nameof(AllOrders));
    }

    public async Task<IActionResult> UpdateOrderStatus(int orderId) //Przygotowuje formularz do zmiany statusu zamówienia
    {
        var order = await _userOrderRepository.GetOrderById(orderId);
        if (order == null)
        {
            throw new InvalidOperationException($"Order with id:{orderId} does not found.");
        }
        var orderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
        {
            return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = order.OrderStatusId == orderStatus.Id };
        });
        var data = new UpdateOrderStatusModel
        {
            OrderId = orderId,
            OrderStatusId = order.OrderStatusId,
            OrderStatusList = orderStatusList
        };
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data) //Obsługuje zapis zmian statusu zamówienia.
    {
        try
        {
            if (!ModelState.IsValid)
            {
                data.OrderStatusList = (await _userOrderRepository.GetOrderStatuses()).Select(orderStatus =>
                {
                    return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = orderStatus.Id == data.OrderStatusId };
                });

                return View(data);
            }
            await _userOrderRepository.ChangeOrderStatus(data);
            TempData["msg"] = "Updated successfully";
        }
        catch (Exception ex)
        {
            // możliwy wyjątek
            TempData["msg"] = "Something went wrong";
        }
        return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
    }


    public IActionResult Dashboard() //widok pulpitu administratora.
    {
        return View();
    }

}
