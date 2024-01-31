using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReadRent.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Xml.Linq;

namespace ReadRent.Controllers
{
    public class BooksController : Controller
    {
        private ReadRentEntities db = new ReadRentEntities();

        // GET: Books
        public ActionResult Index(string sortOrder, string sortColumn, string searchString)
        {
            if (Session["user_id"] != null)
            {
                IQueryable<ReadRent.Models.Book> books = db.Books;
                bool ascending = sortOrder == "asc";
                if (!String.IsNullOrEmpty(searchString))
                {
                    books = books.Where(s => s.title.Contains(searchString)
                                          || s.author.Contains(searchString));
                }

                switch (sortColumn)
                {
                    case "title":
                        books = ascending ? books.OrderBy(b => b.title) : books.OrderByDescending(b => b.title);
                        break;
                    case "author":
                        books = ascending ? books.OrderBy(b => b.author) : books.OrderByDescending(b => b.author);
                        break;
                    case "price":
                        books = ascending ? books.OrderBy(b => b.price) : books.OrderByDescending(b => b.price);
                        break;
                    default:
                        books = books.OrderBy(b => b.title);
                        break;
                }

                ViewData["SortOrder"] = sortOrder;
                ViewData["SortColumn"] = sortColumn;

                return View(books.ToList());
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        public FileStreamResult DownloadWord()
        {
            IQueryable<ReadRent.Models.Book> books = db.Books;
            int rows = books.Count();
            int columns = 4;
            string[,] data = new string[rows, columns];
            int i = 0;
            foreach (var book in books)
            {
                data[i, 0] = book.id.ToString();
                data[i, 1] = book.title;
                data[i, 2] = book.author;
                data[i, 3] = book.price.ToString();
                i++;
            }
            MemoryStream memoryStream = GenerateWord(data);
            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                FileDownloadName = "demo.docx"
            };
        }

        private MemoryStream GenerateWord(string[,] data)
        {
            MemoryStream mStream = new MemoryStream();
            // Создаем документ
            using (WordprocessingDocument document = WordprocessingDocument.Create(mStream, WordprocessingDocumentType.Document, true))
            {
                // Добавляется главная часть документа. 
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                // Создаем таблицу. 
                Table table = new Table();
                body.AppendChild(table);

                // Устанавливаем свойства таблицы(границы и размер).
                TableProperties props = new TableProperties(
                    new TableBorders(
                    new TopBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 15
                    },
                    new BottomBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 15
                    },
                    new LeftBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 15
                    },
                    new RightBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 15
                    },
                    new InsideHorizontalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 15
                    },
                    new InsideVerticalBorder
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 1
                    }));

                // Назначаем свойства props объекту table
                table.AppendChild<TableProperties>(props);

                // Заполняем ячейки таблицы.
                for (var i = 0; i <= data.GetUpperBound(0); i++)
                {
                    var tr = new TableRow();
                    for (var j = 0; j <= data.GetUpperBound(1); j++)
                    {
                        var tc = new TableCell();
                        tc.Append(new Paragraph(new Run(new Text(data[i, j]))));

                        // размер колонок определяется автоматически.
                        tc.Append(new TableCellProperties(
                            new TableCellWidth { Type = TableWidthUnitValues.Auto }));

                        tr.Append(tc);
                    }
                    table.Append(tr);
                }

                mainPart.Document.Save();
                //document.Close();
                //mStream.Position = 0;
                return mStream;
            }
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Book book = db.Books.Find(id);
                if (book == null)
                {
                    return HttpNotFound();
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            if (Session["user_id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Books/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,author,price")] Book book)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Books.Add(book);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(book);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Book book = db.Books.Find(id);
                if (book == null)
                {
                    return HttpNotFound();
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Books/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,author,price")] Book book)
        {
            if (Session["user_id"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(book).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["user_id"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Book book = db.Books.Find(id);
                if (book == null)
                {
                    return HttpNotFound();
                }
                return View(book);
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["user_id"] != null)
            {
                Book book = db.Books.Find(id);
                db.Books.Remove(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("../Home/Login");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
