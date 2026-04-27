using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Latihan_Crud.Data;
using Latihan_Crud.Models;

namespace Latihan_Crud.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Menampilkan daftar produk
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            try {
                // Ambil semua data produk sebagai query
                var produkQuery = from p in _context.Products select p;
                // logika pencarian
                if (!string.IsNullOrEmpty(searchString)) 
                {
                    // Mencari berdasarkan Nama Produk  
                    produkQuery = produkQuery.Where(s => s.Name.ToLower().Contains(searchString.ToLower()));

                    // Simpan kata kunci di viewbag agar search tidak kosong setelah di cari
                    ViewBag.CurrentFilter = searchString;
                }
                    // pagin
                    int pageSize = 5; // Jumlah produk per halaman
                    int pageNumber = (page ?? 1); // Halaman saat ini, default ke 1 jika null
                    
                    // hitung data di filter
                    int totalData = await produkQuery.CountAsync();
                    ViewBag.TotalPages = (int)Math.Ceiling((double)totalData / pageSize );
                    ViewBag.CurrentPage = pageNumber;

                    // Eksekusi ke database
                    var ListProduk = produkQuery
                                    .OrderBy(p => p.Id) // Urutkan berdasarkan Id
                                    .Skip((pageNumber - 1) * pageSize) // Lewati data untuk halaman sebelumnya
                                    .Take(pageSize) // Ambil data untuk halaman saat ini
                                    .ToList();
                    return View(ListProduk);

            } catch (Exception ex) {
                    TempData["TipeNotifikasi"] = "error";
                    TempData["Pesan"] = $"Koneksi ke database gagal!Pastikan database aktif!. {ex.Message}";

                    System.Diagnostics.Debug.WriteLine(ex.Message); // Log error ke konsol
                    return View(new List<Product>()); // Mengembalikan daftar kosong jika terjadi kesalahan
            }
        }

        // Menampilkan Halaman detail produk
        public async Task<IActionResult> DetailProduk(int id)
        {
            var produk =  await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (produk == null) return NotFound();
            return View(produk);
        }

        // Menampilkan Halaman form tambah produk
        [HttpGet]
        public IActionResult Create(Product produk)
        {
            return View();
        }

        // Konfirmasi Simpan data ke database
        [HttpPost]
        public async Task<IActionResult> Save(Product produk)
        {
            try {
                if (ModelState.IsValid)
                {
                     _context.Products.Add(produk); // menambahkan ke memori
                     await _context.SaveChangesAsync(); // menyimpan permanen ke postgre/database

                     TempData["TipeNotifikasi"] = "tambah";
                     TempData["Pesan"] = "Data produk baru berhasil ditambahkan!";
                     return RedirectToAction("Index");
                }
            } catch (Exception ex) {
                     TempData["TipeNotifikasi"] = "error";
                     TempData["Pesan"] = $"Terjadi kesalahan saat menyimpan data: {ex.Message}";
                Console.WriteLine(ex.Message);            
            }

            return View(produk);
        }

        // Menampilkan Halaman edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var produk = await _context.Products.FindAsync(id);
            if (produk == null) return NotFound();
            return View(produk);
        }

        // Konfirmasi update data ke database
        [HttpPost]
        public async Task<IActionResult> Edit(Product produk)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(produk); // menambahkan ke memori
                await _context.SaveChangesAsync(); // menyimpan permanen ke postgre/database
                TempData["TipeNotifikasi"] = "edit";
                TempData["Pesan"] = "Data berhasil diubah!";
                return RedirectToAction("Index");
            }
            return View(produk);
        }

        //// Hapus Data - GET
        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    var produk = _context.Products.Find(id);
        //    if (produk == null) return NotFound();
        //    return View(produk);
        //} 

        // Konfirmasi Hapus Data - Post
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try {
                var produk = await _context.Products.FindAsync(id);
            if (produk != null)
                {
                _context.Products.Remove(produk); // menambahkan ke memori
                await _context.SaveChangesAsync(); // menyimpan permanen ke postgre/database
                TempData["TipeNotifikasi"] = "hapus";
                TempData["Pesan"] = "Data berhasil dihapus!";
                }
            } catch (Exception ex) { 
                TempData["TipeNotifikasi"] = "error";
                TempData["Pesan"] = $"Data tidak bisa dihapus karena sedang digunakan di lain data. {ex.Message}";
            }
            return RedirectToAction("Index");

        }
    }
}
