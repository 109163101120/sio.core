using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Swastika.Extension.Blog.Data;
using Swastika.Extension.Blog.Models;
using Swastika.Extensions.Blog.Repositories;
using Swastika.Extension.Blog.ViewModels;

namespace Swastika.Extensions.Blog.Web.Controllers
{
    public class ManageController : Controller
    {
        //private readonly BlogDbContext _context;
        private readonly BlogPostRepository _repo;

        public ManageController(BlogDbContext context)
        {
            //_context = context;
            _repo = BlogPostRepository.GetInstance(context);
        }

        // GET: Manage
        public async Task<IActionResult> Index()
        {
            return View(await _repo.GetViewModelListAsync(false));
        }

        // GET: Manage/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _repo.GetSingleModelAsync(m => m.Id == id, false);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Manage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manage/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogViewModel vmBlog)
        {
            if (ModelState.IsValid)
            {
                vmBlog.ParseModel();
                var blog = vmBlog.Model;
                blog.Id = Guid.NewGuid();
                blog.CreatedByUserId = Guid.NewGuid().ToString();
                await _repo.CreateModelAsync(blog);
                return RedirectToAction("Index");
            }
            return View(vmBlog);
        }

        // GET: Manage/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _repo.GetSingleModelAsync(m => m.Id == id, false);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Manage/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BlogViewModel vmBlog)
        {
            vmBlog.ParseModel();
            var blog = vmBlog.Model;

            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repo.EditModelAsync(blog);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_repo.CheckExists(vmBlog.Model))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(vmBlog);
        }

        // GET: Manage/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _repo.GetSingleModelAsync(m => m.Id == id, false);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Manage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var blog = await _repo.GetSingleModelAsync(m => m.Id == id, false);
            await _repo.RemoveModelAsync(blog.Model);
            return RedirectToAction("Index");
        }
    }
}
