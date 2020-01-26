using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private ILibraryBranch _branch;
        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }
        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(branch => new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                IsOpen= _branch.IsBranchOpen(branch.Id),
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons =_branch.Getpatron(branch.Id).Count()

            });

            var model = new BranchIndexModel()
            {
                Branches = branches
            };
            return View(model);
        }

        public IActionResult Detail(int Id)
        {
            var branch = _branch.Get(Id);
            var model = new BranchDetailModel
            {
              Id = branch.Id,
              Name = branch.Name,
              Address = branch.Address,
              Telephone = branch.Telephone,
              OpenedDate = branch.OpenDate.ToString("yyyy-MM-dd"),
              NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
              NumberOfPatrons = _branch.Getpatron(branch.Id).Count(),
              TotalAssetValue = _branch.GetAssets(Id).Sum(a => a.Cost),
              ImageUrl = branch.ImageUrl,
              HoursOpen = _branch.GetBranchHours(Id)
            };

            return View(model);
        }
    }
}