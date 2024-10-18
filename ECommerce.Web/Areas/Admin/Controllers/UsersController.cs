using ECommerce.DataAccess.Data;
using ECommerce.Entities.Models;
using ECommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userid = claim.Value;

            return View(_context.ApplicationUsers.Where(x => x.Id != userid).ToList());
        }

        public IActionResult LockUnlock(string? id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound(); // Return 404 if the user is not found
            }

            _context.Users.Remove(user);
            _context.SaveChanges(); // Save changes to the database

            // Redirect to the index or users list page after deletion
            return RedirectToAction("Index"); // Change "Index" to your relevant action name
        }
        public IActionResult Edit(string id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound(); // Return 404 if the user is not found
            }

            return View(user); // Pass the user object to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, ApplicationUser updatedUser)
        //{
        //    if (id != updatedUser.Id)
        //    {
        //        return NotFound(); // Ensure the ID matches the updated user's ID
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var user = await _context.ApplicationUsers.FindAsync(id); // Find the user by Id
        //            if (user == null)
        //            {
        //                return NotFound(); // Return 404 if the user is not found
        //            }

        //            // Update relevant properties (example: updating only some fields)
        //            user.UserName = updatedUser.UserName;
        //            user.Email = updatedUser.Email;
        //            // Add other properties you want to update

        //            await _context.ApplicationUsers.ExecuteUpdateAsync(user)// Update user using UserManager

        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction(nameof(Index)); // Redirect to the users list after successful update
        //            }

        //            // Add errors to the ModelState if update failed
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            // Handle concurrency exception if another process modified the user
        //            if (!await _userManager.Users.AnyAsync(e => e.Id == id))
        //            {
        //                return NotFound(); // Return 404 if the user is no longer found
        //            }

        //            // Log the concurrency error (optional)
        //            ModelState.AddModelError(string.Empty, "A concurrency error occurred. Please try again.");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log the exception (optional)
        //            // _logger.LogError(ex, "An error occurred while updating the user");

        //            ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
        //        }
        //    }

        //    return View(updatedUser); // Return the view with the updated user if model state is invalid or update failed
        //}


        public IActionResult Edit(string id, ApplicationUser updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return NotFound(); // Ensure the ID matches
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Users.Update(updatedUser); // Update the user in the context
                    _context.SaveChanges(); // Save changes to the database
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Id == id))
                    {
                        return NotFound(); // Return 404 if the user is no longer found
                    }
                    throw; // Otherwise, rethrow the exception
                }

                return RedirectToAction("Index"); // Redirect to the users list after successful update
            }

            return View(updatedUser); // Return to the view with the updated user if model state is invalid
        }
    }
}
