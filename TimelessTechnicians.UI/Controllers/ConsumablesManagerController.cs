using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimelessTechnicians.UI.Data;
using TimelessTechnicians.UI.Models;
using TimelessTechnicians.UI.ViewModel;

namespace TimelessTechnicians.UI.Controllers
{
    [Authorize(Roles = "CONSUMABLESMANAGER")]
    public class ConsumablesManagerController : Controller
    {
       
        private readonly ApplicationDbContext _context;

        public ConsumablesManagerController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> ConsumablesManagerDashboard()
        {
            var currentUserName = User.Identity.Name;
            ViewData["ShowSidebar"] = true;

            // Fetch statistics
            var totalConsumables = await _context.Consumables.CountAsync(c => c.DeletionStatus == ConsumableStatus.Active);
            var totalStockLogs = await _context.StockLogs.CountAsync();

            // Fetch consumable status counts (for example: Active, Low Stock, Expired)
            var totalLowStock = await _context.Consumables.CountAsync(c => c.DeletionStatus == ConsumableStatus.Active && c.Quantity < 5); // Assuming 5 is the low stock threshold
            var totalExpired = await _context.Consumables.CountAsync(c => c.DeletionStatus == ConsumableStatus.Active && c.ExpiryDate < DateTime.Now);

            // Store statistics in ViewData
            ViewData["TotalConsumables"] = totalConsumables;
            ViewData["TotalStockLogs"] = totalStockLogs;
            ViewData["TotalLowStock"] = totalLowStock;
            ViewData["TotalExpired"] = totalExpired;

            return View();
        }



        public async Task<IActionResult> Consumables()
        {
            ViewData["ShowSidebar"] = true;

            var consumables = await _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active)
                .ToListAsync();

            return View(consumables);
        }





        public IActionResult CreateStockRequest()
        {
            ViewData["ShowSidebar"] = true;

            // Load consumables for dropdown
            var consumables = _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active)
                .Select(c => new { c.ConsumableId, c.Name })
                .ToList();

            ViewBag.Consumables = consumables;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStockRequest(StockRequest request)
        {
            
                _context.StockRequests.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListStockRequests));
            

            // If model validation fails, reload consumables for dropdown
            var consumables = _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active)
                .Select(c => new { c.ConsumableId, c.Name })
                .ToList();

            ViewBag.Consumables = consumables;

            return View(request);
        }


        public async Task<IActionResult> ListStockRequests()
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve only requests that are Active or Approved
            var requests = await _context.StockRequests
                .Include(sr => sr.Consumable) // Load related consumable data
                .Where(sr => sr.RequestStatus == ConsumableStatus.Active || sr.RequestStatus == ConsumableStatus.Approved) // Filter by status
                .ToListAsync();

            return View(requests);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateStockRequest(int id, bool approve)
        {
            var request = await _context.StockRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            // Change the status to Approved if approve is true, otherwise set it to Rejected
            request.RequestStatus = approve ? ConsumableStatus.Approved : ConsumableStatus.Rejected;

            if (approve)
            {
                // Find the corresponding consumable
                var consumable = await _context.Consumables.FindAsync(request.ConsumableId);
                if (consumable != null)
                {
                    // Add the requested quantity to the consumable's stock
                    consumable.Quantity += request.RequestedQuantity;

                    // Update the consumable in the context
                    _context.Consumables.Update(consumable);
                }
            }

            // Update the request in the context
            _context.StockRequests.Update(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListStockRequests));
        }


        // GET: EditStockRequest
        [HttpGet]
        public async Task<IActionResult> EditStockRequest(int id)
        {
            ViewData["ShowSidebar"] = true;
            // Retrieve the stock request by ID
            var request = await _context.StockRequests
                .Include(sr => sr.Consumable) // Load related consumable data
                .FirstOrDefaultAsync(sr => sr.StockRequestId == id);

            if (request == null)
            {
                return NotFound(); // Return 404 if not found
            }

            // Load consumables for dropdown
            var consumables = await _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active)
                .Select(c => new { c.ConsumableId, c.Name })
                .ToListAsync();

            ViewBag.Consumables = consumables; // Pass consumables to the view

            return View(request); // Return the stock request for editing
        }

        // POST: EditStockRequest
        [HttpPost]
        public async Task<IActionResult> EditStockRequest(int id, StockRequest updatedRequest)
        {
            if (id != updatedRequest.StockRequestId)
            {
                return NotFound(); // Return 404 if IDs do not match
            }

           
                try
                {
                    // Update the request in the context
                    _context.StockRequests.Update(updatedRequest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ListStockRequests)); // Redirect to the list of requests
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockRequestExists(updatedRequest.StockRequestId))
                    {
                        return NotFound(); // Handle concurrency issues and return 404 if the request no longer exists
                    }
                    throw; // Re-throw if it's not a concurrency issue
                }
            

            // If model validation fails, reload consumables for dropdown
            var consumables = await _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active)
                .Select(c => new { c.ConsumableId, c.Name })
                .ToListAsync();

            ViewBag.Consumables = consumables; // Pass consumables back to the view

            return View(updatedRequest); // Return the updated request with validation errors
        }

        // Helper method to check if a stock request exists
        private bool StockRequestExists(int id)
        {
            return _context.StockRequests.Any(e => e.StockRequestId == id);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteStockRequest(int id)
        {
            var request = await _context.StockRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            // Set the deletion status to Deleted
            request.RequestStatus = ConsumableStatus.Deleted;

            // No need to remove it from the context
            _context.StockRequests.Update(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListStockRequests));
        }



        [HttpGet]
        public async Task<IActionResult> TakeStock(int id)
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve the consumable by its ID
            var consumable = await _context.Consumables.FindAsync(id);

            if (consumable == null || consumable.DeletionStatus != ConsumableStatus.Active)
            {
                return NotFound("Consumable not found or inactive.");
            }

            return View(consumable);
        }

        // POST: TakeStock (Reduce the quantity of the consumable based on the amount taken)
        [HttpPost]
        public async Task<IActionResult> TakeStock(int id, int quantityTaken)
        {
            var consumable = await _context.Consumables.FindAsync(id);

            if (consumable == null || consumable.DeletionStatus != ConsumableStatus.Active)
            {
                return NotFound("Consumable not found or inactive.");
            }

            if (quantityTaken <= 0)
            {
                ModelState.AddModelError("", "The quantity taken must be greater than zero.");
            }

            if (quantityTaken > consumable.Quantity)
            {
                ModelState.AddModelError("", "The quantity taken exceeds the available stock.");
            }

            
                consumable.Quantity -= quantityTaken;

                // Create a new stock log entry
                var stockLog = new StockLog
                {
                    ConsumableId = consumable.ConsumableId,
                    QuantityTaken = quantityTaken,
                    DateTaken = DateTime.Now
                };

                // Add the stock log entry to the context
                _context.StockLogs.Add(stockLog);

                // Update the consumable in the context
                _context.Consumables.Update(consumable);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ListStockLogs)); // Redirect back to consumables list
            

            return View(consumable);
        }
        public async Task<IActionResult> ListStockLogs()
        {
            ViewData["ShowSidebar"] = true;

            var logs = await _context.StockLogs
                .Include(sl => sl.Consumable) // Include related consumable data
                .ToListAsync();

            return View(logs);
        }


        // GET: CheckConsumables
        public async Task<IActionResult> CheckConsumables()
        {
            ViewData["ShowSidebar"] = true;

            // Retrieve all active consumables
            var consumables = await _context.Consumables
                .Where(c => c.DeletionStatus == ConsumableStatus.Active)
                .Select(c => new
                {
                    c.ConsumableId,
                    c.Name,
                    c.Quantity,
                    c.ExpiryDate,
                    c.Description
                })
                .ToListAsync();

            return View(consumables);
        }

    }
}
