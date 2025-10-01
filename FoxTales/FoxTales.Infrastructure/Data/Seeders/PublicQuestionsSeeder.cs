using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class PublicQuestionsSeeder(FoxTalesDbContext context, ILogger<PublicQuestionsSeeder> logger)
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<PublicQuestionsSeeder> _logger = logger;

    public async Task SeedAsync(bool clearQuestions)
    {
        if (clearQuestions)
        {
            var publicQuestions = await _context.Questions
                .Where(q => q.IsPublic)
                .ToListAsync();

            _context.Questions.RemoveRange(publicQuestions);
            await _context.SaveChangesAsync();
        }

        if (_context.Questions.Any(q => q.IsPublic))
        {
            return;
        }

        _logger.LogInformation("Start public questions seeding");

        if (!_context.Users.Any(u => u.Username == "Fox Templates"))
        {
            var newUser = new User
            {
                Username = "Fox Templates",
                AvatarId = 1,
                UserStatus = UserStatus.Active,
                RoleId = (int)RoleName.Admin
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "Fox Templates")!;
        if (user == null)
        {
            _logger.LogError("User 'Fox Templates' was not found.");
            return;
        }

        IEnumerable<Question> questions = GetQuestions(user.UserId);
        await _context.Questions.AddRangeAsync(questions);
        await _context.SaveChangesAsync();
    }

    private static List<Question> GetQuestions(int userId)
    {
        var templates = new List<string>
        {
            "Co **** najczęściej robi po przebudzeniu?",
            "Jaka jest największa dziwna fobia ****?",
            "Co **** zrobił na ostatniej imprezie, co wszyscy pamiętają?",
            "Jaką najdziwniejszą rzecz **** kiedykolwiek zjadł?",
            "Jaka jest ulubiona przekąska ****?",
            "Co **** zawsze zapomina zabrać ze sobą?",
            "Jakie jest najbardziej nietypowe hobby ****?",
            "Co **** robi, gdy jest sam w domu?",
            "Jaki jest sekret ****, którego nikt nie zna?",
            "Co **** zrobiłby, gdyby wygrał milion dolarów?",
            "Jakie dziwne imię **** nadałby swojemu zwierzakowi?",
            "Co **** najczęściej śpiewa pod prysznicem?",
            "Jaką najbardziej absurdalną rzecz **** kiedykolwiek kupił?",
            "Co **** robi, gdy nikt nie patrzy?",
            "Jakie jest ulubione niezdrowe jedzenie ****?",
            "Jaka jest najbardziej śmieszna kłótnia ****?",
            "Co **** robi, żeby się uspokoić?",
            "Jakie jest najbardziej niepraktyczne ubranie ****, które kiedykolwiek nosił?",
            "Co **** zrobiłby na scenie talent show?",
            "Jakie jest największe marzenie ****?",
            "Co **** zawsze mówi, ale nikt go nie słucha?",
            "Jakie jest najbardziej dziwne miejsce, w którym **** zasnął?",
            "Co **** najbardziej lubi w weekendy?",
            "Jakie jest najbardziej wstydliwe zdjęcie ****?",
            "Co **** zrobiłby, gdyby mógł być niewidzialny przez jeden dzień?",
            "Jakie jest ulubione powiedzonko ****?",
            "Co **** najczęściej gubi?",
            "Jaką najdziwniejszą kombinację jedzenia **** uwielbia?",
            "Co **** zrobił, żeby zaimponować komuś?",
            "Jakie jest największe szaleństwo, w którym **** wziął udział?",
            "Co **** zawsze chciał spróbować, ale się boi?",
            "Jakie jest najbardziej absurdalne kłamstwo ****?",
            "Co **** robi, gdy się nudzi?",
            "Jakie jest ulubione zwierzę fantastyczne ****?",
            "Co **** powiedziałby w filmie akcji?",
            "Jakie jest najbardziej dziwne miejsce, w którym **** jadł posiłek?",
            "Co **** zrobiłby, gdyby spotkał kosmitę?",
            "Jakie jest najbardziej nieoczekiwane osiągnięcie ****?",
            "Co **** najbardziej lubi w deszczowe dni?",
            "Jakie jest najdziwniejsze przezwisko ****?",
            "Co **** zawsze bierze ze sobą do szkoły/pracy?",
            "Jakie jest najbardziej dramatyczne wydarzenie w życiu ****?",
            "Co **** robi, gdy nikt go nie widzi?",
            "Jakie jest ulubione śmieszne słowo ****?",
            "Co **** zrobiłby, gdyby mógł cofnąć czas o jeden dzień?",
            "Jakie jest największe wyzwanie, z którym **** musiał się zmierzyć?",
            "Co **** zrobił na ostatnich wakacjach, co wszyscy zapamiętali?",
            "Jakie jest ulubione miejsce relaksu ****?",
            "Co **** najbardziej lubi w swojej pracy/szkole?",
            "Jakie jest najbardziej nietypowe zwierzę domowe, które chciałby mieć ****?",
            "Co **** zawsze robi źle?",
            "Jakie jest najbardziej ekstremalne doświadczenie ****?",
            "Co **** zrobiłby, gdyby wygrał w loterii?",
            "Jakie jest ulubione śniadanie ****?",
            "Co **** mówi, gdy się zdenerwuje?",
            "Jakie jest najbardziej niepraktyczne gadżet, który kupił ****?",
            "Co **** robi, gdy próbuje zaimponować znajomym?",
            "Jakie jest najdziwniejsze miejsce, w którym **** spał?",
            "Co **** zawsze chce zmienić w swoim życiu?",
            "Jakie jest najbardziej absurdalne marzenie ****?",
            "Co **** zrobiłby, gdyby mógł zamienić się ciałami z kimś na jeden dzień?",
            "Jakie jest ulubione jedzenie **** na święta?",
            "Co **** mówi, gdy jest przestraszony?",
            "Jakie jest najbardziej zabawne wspomnienie z dzieciństwa ****?",
            "Co **** zrobiłby, gdyby mógł mieszkać gdziekolwiek na świecie?",
            "Jakie jest najbardziej dziwne hobby ****?",
            "Co **** najczęściej robi w social mediach?",
            "Jakie jest ulubione nieoczekiwane miejsce spacerów ****?",
            "Co **** zrobiłby, gdyby spotkał swojego sobowtóra?",
            "Jakie jest najbardziej szalone zadanie, które **** podjął?",
            "Co **** zawsze chce mieć w swojej torbie/plecaku?",
            "Jakie jest ulubione sportowe osiągnięcie ****?",
            "Co **** zrobiłby, gdyby mógł podróżować w czasie?",
            "Jakie jest najbardziej wstydliwe zachowanie **** w restauracji?",
            "Co **** zrobił, żeby zaimponować swojej sympatii?",
            "Jakie jest ulubione miejsce na wakacje ****?",
            "Co **** mówi, gdy się zgubi?",
            "Jakie jest najbardziej absurdalne ubranie, które **** kiedykolwiek nosił?",
            "Co **** zrobiłby, gdyby mógł kontrolować pogodę?",
            "Jakie jest ulubione dziwne zwierzę ****?",
            "Co **** zawsze zostawia w domu?",
            "Jakie jest najbardziej nieoczekiwane wydarzenie, które spotkało ****?",
            "Co **** robi w sobotnie wieczory?",
            "Jakie jest ulubione miejsce do jedzenia ****?",
            "Co **** zrobiłby, gdyby mógł rozmawiać ze zwierzętami?",
            "Jakie jest najbardziej zabawne powiedzonko ****?",
            "Co **** zrobił, żeby zdobyć nagrodę lub wyróżnienie?",
            "Jakie jest ulubione miejsce do spania ****?",
            "Co **** zawsze robi, gdy jest w złym humorze?",
            "Jakie jest najbardziej nieoczekiwane zdarzenie w życiu ****?"
        };

        var questions = new List<Question>();

        foreach (var template in templates)
        {
            questions.Add(new Question
            {
                Text = template,
                IsPublic = true,
                Language = Language.PL,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            });
        }

        return questions;
    }

}
