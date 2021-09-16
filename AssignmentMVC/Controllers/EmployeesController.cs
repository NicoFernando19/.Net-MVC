using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssignmentMVC.Data.DatabaseContext;
using AssignmentMVC.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AssignmentMVC.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmployeesController(DatabaseContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,PhoneNumber,Gender,Email,DoB")] Employee employee, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                employee.Id = Guid.NewGuid();
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    var FolderPath = Path.Combine("Upload", "Images");
                    var pathToSave = Path.Combine(_hostingEnvironment.WebRootPath, FolderPath);
                    var newFileName = Guid.NewGuid().ToString() + extension;
                    var fullPath = Path.Combine(pathToSave, newFileName);
                    var dbPath = Path.Combine(FolderPath, newFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    employee.ImagePath = dbPath;
                    employee.ImageName = newFileName;
                }
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            

            return View(employee);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Address,PhoneNumber,Gender,Email,DoB,IsActive,ImagePath,ImageName,CreatedAt")] Employee employee, List<IFormFile> files)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var extension = Path.GetExtension(file.FileName);
                            var FolderPath = Path.Combine("Upload", "Images");
                            var pathToSave = Path.Combine(_hostingEnvironment.WebRootPath, FolderPath);
                            var newFileName = Guid.NewGuid().ToString() + extension;
                            var fullPath = Path.Combine(pathToSave, newFileName);
                            var dbPath = Path.Combine(FolderPath, newFileName);

                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            //Delete Old file
                            if (!String.IsNullOrEmpty(employee.ImageName))
                            {
                                var oldFullFilePath = Path.Combine(pathToSave, employee.ImageName);
                                if (System.IO.File.Exists(oldFullFilePath))
                                    System.IO.File.Delete(oldFullFilePath);
                            }

                            employee.ImagePath = dbPath;
                            employee.ImageName = newFileName;
                        }
                    }
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            var FolderPath = Path.Combine("Upload", "Images");
            var DeleteImage = Path.Combine(Directory.GetCurrentDirectory(), FolderPath, employee.ImageName);
            if (System.IO.File.Exists(DeleteImage))
                System.IO.File.Delete(DeleteImage);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(Guid id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
