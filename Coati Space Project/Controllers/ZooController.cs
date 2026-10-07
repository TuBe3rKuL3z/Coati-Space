using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Coati_Space_Project.Models;

namespace Coati_Space_Project.Controllers
{
    public class ZooController : Controller
    {
        ApplicationContext db;

        public ZooController(ApplicationContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var coati = await db.Coatis.FirstOrDefaultAsync();
            return View(coati);
        }

        public async Task<IActionResult> Coati()
        {
            var coati = await db.Coatis.FirstOrDefaultAsync();
            if (coati == null)
            {
                coati = new Coati
                {
                    Name = "Чип",
                    Species = "Южноамериканская носуха (Nasua nasua)",
                    Gender = "Самец",
                    BirthDate = new DateOnly(2021, 5, 14),
                    Biography = "Чип — всеобщий любимец зоопарка. Активный, любознательный и дружелюбный обитатель вольера.",
                    Diet = "Фрукты, перепелиные яйца, насекомые, нежирное мясо.",
                    Habitat = "Вольер №7, Сектор «Южная Америка».",
                    HealthStatus = "Отличное, активен",
                    PhotoUrl = "/images/coati-main.jpg",
                    VideoUrl = "/videos/coati-stream.mp4"
                };
            }

            var recentEntries = await db.DiaryEntries
                .OrderByDescending(e => e.EventDate)
                .Take(4)
                .ToListAsync();

            var totalDonations = await db.Donations.SumAsync(d => (decimal?)d.Amount) ?? 0;
            var donationsCount = await db.Donations.CountAsync();

            var viewModel = new CoatiViewModel
            {
                Coati = coati,
                GalleryImages = new List<string>
                {
                    "/images/coati-gallery-1.jpg",
                    "/images/coati-gallery-2.jpg",
                    "/images/coati-gallery-3.jpg"
                },
                RecentDiaryEntries = recentEntries,
                TotalDonations = totalDonations,
                DonationsCount = donationsCount
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Donate(string donorName, string? email, decimal amount, string target, string? message)
        {
            if (amount <= 0)
            {
                amount = 100;
            }

            var donation = new Donation
            {
                DonorName = string.IsNullOrWhiteSpace(donorName) ? "Анонимный друг" : donorName,
                Email = email,
                Amount = amount,
                Target = string.IsNullOrWhiteSpace(target) ? "На лакомства и фрукты" : target,
                Message = message,
                CreatedAt = DateTime.Now
            };

            db.Donations.Add(donation);
            await db.SaveChangesAsync();

            TempData["DonationSuccess"] = $"[РЕЖИМ ЗАГЛУШКИ] Спасибо, {donation.DonorName}! Форма работает в демонстрационном режиме. Ваш виртуальный взнос {donation.Amount:N0} ₽ («{donation.Target}») успешно принят. Чип шлет вам благодарность!";
            return Redirect("/stav-zoo/coati#donation-section");
        }
    }
}
