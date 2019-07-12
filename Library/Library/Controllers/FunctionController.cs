using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Controllers
{
    public class FunctionController : Controller
    {
        Models.BookService bookservice = new Models.BookService();

        /// <summary>
        /// index畫面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// BookClass的DropDownList
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DropForClass()
        {

            return Json(this.bookservice.GetClassTable());
        }

        /// <summary>
        /// User的DropDownList
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DropForUser()
        {

            return Json(this.bookservice.GetUserTable());
        }

        /// <summary>
        /// BookStatus的DropDownList
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DropForStatus()
        {

            return Json(this.bookservice.GetStatusTable());
        }

        /// <summary>
        /// Grid
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Get_data_grid(Models.BookSearch arg)
        {
            return Json(this.bookservice.GetBookData(arg));
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Book_Delete(int BookId)
        {
            return Json(this.bookservice.DeleteBook(BookId));
        }

        /// <summary>
        /// 新增畫面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult InsertBook()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InsertBook(Models.BOOK arg)
        {
            if (arg.BookName == null || arg.Author == null || arg.Publisher == null || arg.Introduction == null || arg.BuyDate == null || arg.BookClassId == null)
            {
                return Json(false);
            }

            return Json(this.bookservice.InsertBook(arg));
        }

        /// <summary>
        /// 借閱紀錄
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Record(int BookId)
        {
            return Json(this.bookservice.GetBookLendRecord(BookId));
        }

        /// <summary>
        /// 明細
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BookDetail(int BookId)
        {
            return Json(this.bookservice.GetOriginData(BookId));
        }
    }
}