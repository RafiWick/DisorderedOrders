using DisorderedOrdersMVC.DataAccess;
using DisorderedOrdersMVC.Models;
using DisorderedOrdersMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DisorderedOrdersMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DisorderedOrdersContext _context;

        public OrdersController(DisorderedOrdersContext context)
        {
            _context = context;
        }

        public IActionResult New(int customerId)
        {
            var products = _context.Products.Where(p => p.StockQuantity > 0);
            ViewData["CustomerId"] = customerId;

            return View(products);
        }

        [HttpPost]
        [Route("/orders")]
        public IActionResult Create(IFormCollection collection, string paymentType)
        {
            // create order
            var order = CreateOrder(collection);

            // verify stock available
            order.VerifyStock();

            // process payment
            IPaymentProcessor processor = CreateProcessor(paymentType);
            

            processor.ProcessPayment(order.Total());

            _context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("Show", new { id = order.Id});
        }

        public Order CreateOrder(IFormCollection collection)
        {
            // create order
            int customerId = Convert.ToInt16(collection["CustomerId"]);
            Customer customer = _context.Customers.Find(customerId);
            
            var order = new Order() { Customer = customer };

            
            order.PopulateOrderItems(collection, _context);
            
            return order;
        }
        public IPaymentProcessor CreateProcessor(string paymentType)
        {
            IPaymentProcessor processor;
            if (paymentType == "bitcoin")
            {
                processor = new BitcoinProcessor();
            }
            else if (paymentType == "paypal")
            {
                processor = new PayPalProcessor();
            }
            else
            {
                processor = new CreditCardProcessor();
            }
            return processor;
        }

        [Route("/orders/{id:int}")]
        public IActionResult Show(int id)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Item)
                .Where(o => o.Id == id).First();

            ViewData["total"] = order.Total();

            return View(order);
        }
    }
}
