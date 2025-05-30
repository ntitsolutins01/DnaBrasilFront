using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Enumerators;
using WebApp.Models;

namespace WebApp.Controllers
{
    public abstract partial class BaseController : Controller
    {

        public BaseController()
        {
        }

        public String ErrorMessage
        {
            get { return TempData["ErrorMessage"] == null ? String.Empty : TempData["ErrorMessage"].ToString(); }
            set { TempData["ErrorMessage"] = value; }
        }

        public void SetCrudMessage(int? crud)
        {
            switch (crud)
            {
                case (int)EnumCrud.Created:
                    ViewBag.CrudMessage = (int)EnumCrud.Created;
                    break;
                case (int)EnumCrud.Updated:
                    ViewBag.CrudMessage = (int)EnumCrud.Updated;
                    break;
                case (int)EnumCrud.Deleted:
                    ViewBag.CrudMessage = (int)EnumCrud.Deleted;
                    break;
                default:
                    ViewBag.CrudMessage = -1;
                    break;
            }
        }
        public void SetNotifyMessage(int? notify, string message)
        {
            switch (notify)
            {
                case (int)EnumNotify.Success:
                    ViewBag.NotifyMessage = (int)EnumNotify.Success;
                    ViewBag.Notify = message;
                    break;
                case (int)EnumNotify.Error:
                    ViewBag.NotifyMessage = (int)EnumNotify.Error;
                    ViewBag.Notify = message;
                    break;
                case (int)EnumNotify.Warning:
                    ViewBag.NotifyMessage = (int)EnumNotify.Warning;
                    ViewBag.Notify = message;
                    break;
                default:
                    ViewBag.NotifyMessage = -1;
                    ViewBag.Notify = "null";
                    break;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}