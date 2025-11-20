using FoxTales.Domain.Entities;
using FoxTales.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FoxTales.Infrastructure.Data.Seeders;

public class PublicQuestionsSeeder(FoxTalesDbContext context, ILogger<PublicQuestionsSeeder> logger) : IClearableSeeder
{
    private readonly FoxTalesDbContext _context = context;
    private readonly ILogger<PublicQuestionsSeeder> _logger = logger;
    public async Task ClearAsync()
    {
        List<Question>? publicQuestions = await _context.Questions
            .Where(q => q.IsPublic)
            .ToListAsync();

        if (publicQuestions.Count != 0)
            _context.Questions.RemoveRange(publicQuestions);

        List<Catalog>? publicCatalogs = await _context.Catalogs
            .Where(q => q.CatalogTypeId == (int)CatalogTypeName.Public)
            .ToListAsync();

        if (publicCatalogs.Count != 0)
            _context.Catalogs.RemoveRange(publicCatalogs);
    }

    public async Task SeedAsync()
    {
        if (await _context.Questions.AnyAsync(q => q.IsPublic))
        {
            return;
        }

        _logger.LogInformation("Start public questions seeding");

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "Fox Templates")!;
        if (user == null)
        {
            user = new User
            {
                Username = "Fox Templates",
                AvatarId = 1,
                UserStatus = UserStatus.Active,
                RoleId = (int)RoleName.Admin
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        ICollection<Catalog> catalogs = await CreateCatalogs(user.UserId);
        IEnumerable<Question> questions = GetQuestions(user.UserId, catalogs);
        await _context.Questions.AddRangeAsync(questions);
    }

    private async Task<ICollection<Catalog>> CreateCatalogs(int userId)
    {
        var catalogs = new List<Catalog>
        {
            new()
            {
                Title = "INTYMNE",
                CatalogTypeId = (int)CatalogTypeName.Public,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            },
            new()
            {
                Title = "PUBLIC_CATALOG_1",
                CatalogTypeId = (int)CatalogTypeName.Public,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            },
            new()
            {
                Title = "PUBLIC_CATALOG_2",
                CatalogTypeId = (int)CatalogTypeName.Public,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            },
            new()
            {
                Title = "PUBLIC_CATALOG_3",
                CatalogTypeId = (int)CatalogTypeName.Public,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            }
        };

        await _context.Catalogs.AddRangeAsync(catalogs);
        return catalogs;
    }

    private static List<Question> GetQuestions(int userId, ICollection<Catalog> catalogs)
    {
        var templates = new (string[] Catalogs, string[] Types, string Text)[]

        {
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł nagrać wspólne reality show z tą grupą?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wybrać piosenkę opisującą tę ekipę?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby był postacią z musicalu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby został sędzią w konkursie talentów graczy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś próbuje go przekupić ciastkiem?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby miał stworzyć własny znak zodiaku?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby był głosem nawigacji GPS?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał przez dzień być menadżerem tej grupy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby ktoś pomylił go z celebrytą?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wygłosić mowę na weselu przyjaciela?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby był figurką w planszówce?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy zaczęli mówić jego cytatami?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy słyszy swoje imię w plotkach?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł zaprojektować wspólną koszulkę drużynową?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby jako postać z „Gry o tron”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby był reżyserem filmu o tej ekipie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy coś idzie nie po jego myśli?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wystąpić w teledysku grupowym?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby jako postać z anime?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy mówili do niego jak do króla?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś nie śmieje się z jego żartów?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego imię trafiło do quizu wiedzy o celebrytach?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby był maskotką tej grupy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał napisać poradnik „Jak przetrwać z tą ekipą”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś prosi go o radę w absurdalnej sytuacji?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wystąpić w teleturnieju z resztą graczy jako drużyną?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby został bohaterem reklamy napoju energetycznego?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby jako influencer motywacyjny?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby musiał napisać piosenkę o każdym graczu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby był prowadzącym teleturniej o absurdalnych pytaniach?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby był głównym bohaterem bajki o przyjaźni?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wybrać, kto z tej grupy byłby jego ochroniarzem?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby wszyscy zaczęli go parodiować?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł spędzić jeden dzień w głowie innego gracza?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby w wersji superstylowej na czerwonym dywanie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wymyślić nowy taniec narodowy tej grupy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś wspomina jego dawne wpadki?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby jako złoczyńca w bajce?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał poprowadzić pogadankę o miłości?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy dostaje nieoczekiwany prezent?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego jedynym sposobem komunikacji był taniec?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby musiał zaśpiewać karaoke bez znajomości tekstu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby został burmistrzem miasta, w którym mieszkają gracze?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby był postacią w grze „The Sims”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wymyślić nowy świąteczny zwyczaj?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś próbuje go zawstydzić?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby musiał przez dzień mówić tylko rymami?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś zaczyna śpiewać jego ulubioną piosenkę?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby, gdyby był postacią z bajki Disneya?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego życie miało tytuł filmowy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby ktoś stworzył o nim mema?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy mówili do niego wierszem przez godzinę?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś zaczyna tańczyć obok niego?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał napisać biografię któregoś z graczy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby w filmie o tej grupie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby był duchem, nawiedzającym znajomych?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wymyślić motto dla tej grupy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** brzmiałby jako lektor audiobooków?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał przekonać wszystkich tutaj, że jest kosmitą?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** mówi, gdy próbuje wybrnąć z kłopotliwej sytuacji?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowywałby się jako boss w grze komputerowej?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby cała ta grupa zamieniła się w zombie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby miał poprowadzić ślub jednej z osób stąd?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał opowiedzieć żart na poważnym pogrzebie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** wyglądałby jako emoji?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy z tej grupy byli jego podwładnymi?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby mógł żyć w dowolnym filmie przez tydzień?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby został bohaterem gry RPG, a jego klasa to „Lenistwo”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby musiał przez dzień mówić w stylu komentatora sportowego?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wybrać partnera do apokalipsy spośród tej grupy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś próbuje go poderwać na imprezie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby został zamieniony w pluszowego misia?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby jego głos został podmieniony na głos dziecka?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby jego myśli były wyświetlane nad głową jak napisy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał zostać bohaterem komedii romantycznej?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby jego ulubiona postać fikcyjna stała się prawdziwa?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby musiał wystąpić w konkursie talentów, ale bez talentu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby jego głos zmieniał się za każdym razem, gdy się zdenerwuje?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał stworzyć nowy sport?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby jego odbicie zaczęło mieć własne zdanie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby każdy jego ruch był komentowany przez narratora?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego ulubiony film stał się rzeczywistością?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zareagowałby, gdyby każdy dzień zaczynał się od losowego supermocy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł rozmawiać z duchami?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby musiał wystąpić w reklamie majonezu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł sprawić, że jedna rzecz na świecie zniknie na zawsze?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby został uwięziony w windzie z kimś sławnym?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby nagle znalazł się w świecie baśni?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby mógł zamieniać się miejscami z dowolną osobą przez 10 minut dziennie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zareagowałby, gdyby dostał magiczny długopis, który spełnia życzenia?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby każdy jego żart stawał się prawdą?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby musiał nosić kostium banana przez cały dzień?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby jego dzień powtarzał się w kółko jak w „Dniu świstaka”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał wystąpić w reality show „Przetrwaj tydzień bez internetu”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby został uwięziony w muzeum w nocy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł mieć własną wyspę?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby trafił do średniowiecza z telefonem i powerbankiem?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego sny zaczęły się spełniać w realu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby mógł wymyślić nowe prawo dla całego świata?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby nagle musiał wystąpić na Eurowizji?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reagowałby, gdyby mógł mówić tylko cytatami z filmów?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby dostał list od swojego przyszłego „ja”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby dostał magiczny pilot do zatrzymywania czasu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** uratowałby świat, gdyby miał supermoc, której nikt nie rozumie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł rozmawiać z roślinami?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby przypadkiem został celebrytą?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł teleportować się tylko w miejsca, w których już kiedyś zasnął?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł zamieniać ludzi w dowolne zwierzęta?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zareagowałby, gdyby został królem memów w internecie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowałby się, gdyby dostał list z Hogwartu… ale jako nauczyciel?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł cofnąć się do jednego dnia z dzieciństwa?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał mieszkać w lodówce przez tydzień?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby został porwany przez kosmitów, którzy lubią disco polo?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego najlepszy przyjaciel okazał się robotem?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby został wciągnięty do gry komputerowej?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** poradziłby sobie, gdyby został zamknięty w centrum handlowym na noc?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby nagle potrafił czytać myśli innych?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś mu powie „musimy pogadać”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał iść na randkę z kimś z tej grupy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** mówi, gdy naprawdę nie ma siły na ludzi?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** zachowuje się, gdy dostaje niespodziewany komplement?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego rodzice przeczytali jego czaty?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś daje mu zły prezent?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** mówi, gdy próbuje być grzeczny, ale już nie może?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś wyciąga stare kompromitujące zdjęcia?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby miał odegrać scenę romantyczną na scenie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** mówi, gdy próbuje uniknąć niezręcznej ciszy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał spędzić dzień z osobą, której nie lubi?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** mówi, gdy próbuje wyjść z kłopotliwej sytuacji?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś opowiada żart, który nie jest śmieszny?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby jego przyjaciel wyznał mu, że jest w nim zakochany?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś mu gratuluje za coś, czego nie zrobił?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** mówi, gdy naprawdę nie wie, o co chodzi w rozmowie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby ktoś poprosił go o taniec w środku ulicy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby musiał napisać swoje bio na Tinderze?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby przypadkiem podsłuchał rozmowę o sobie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak **** reaguje, gdy ktoś mu powie: „Nie jesteś taki, jak myślałem”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby ktoś napisał o nim piosenkę miłosną?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby musiał randkować z kimś zupełnie swoim przeciwieństwem?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby dostał 5 minut na przekonanie kogoś, że jest niesamowity?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałaby idealna randka wymyślona przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby przypadkiem wysłał flirtującego SMS-a do złej osoby?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby świat, gdyby wszyscy zachowywali się jak ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby spotkał samego siebie z innego wymiaru?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby idealny dzień wakacji według ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie jedzenie mogłoby zostać symbolem ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby dostał list od samego siebie z przyszłości?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby plakat filmu o życiu ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby świat, gdyby wszyscy mieli poczucie humoru jak ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby został prezenterem prognozy pogody?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby każdy jego ruch miał dźwięk jak w kreskówkach?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie byłoby najgorsze motto firmy założonej przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby poranek **** w alternatywnym wszechświecie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby dostał list z Hogwartu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby musiał prowadzić wesele?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie byłoby hasło kampanii reklamowej stworzonej przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jaka byłaby nazwa zespołu muzycznego założonego przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie byłoby motto religii założonej przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby został spytany o sens życia w telewizji na żywo?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby wygrał dożywotni zapas chipsów?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałaby książka kucharska napisana przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby obudził się w ciele sławnej osoby?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby film o przyjaźni między **** a lodówką?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby mógł wysłać SMS-a do całego świata?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby kosmicznemu przybyszowi na powitanie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł kontrolować pogodę?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby vlog podróżniczy ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby znalazł magiczną lampę z dżinem?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby dzień **** w średniowiecznym zamku?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby był prowadzącym „Familiadę”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby sklep prowadzony przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie byłoby najgorsze możliwe hobby dla ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie byłoby ulubione zaklęcie **** w świecie Harry’ego Pottera?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie byłoby najgorsze hasło motywacyjne wymyślone przez ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł przez jeden dzień łamać wszystkie zasady?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby film dokumentalny o życiu ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby spotkał swoje 10-letnie „ja”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby tatuaż, który idealnie pasuje do ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie jest najbardziej szalone marzenie **** z dzieciństwa?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby dzień **** w średniowieczu?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby wygrał darmowe bilety w jedną stronę w nieznane?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jaką bajkę **** oglądałby po cichu, żeby nikt nie widział?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby poranek **** po wygraniu miliona?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jaką piosenkę **** śpiewałby na karaoke?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** najczęściej odkłada „na później”?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby, gdyby wygrał Oscara?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** robi w sobotnie wieczory, gdy nikt nie ma planów?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby wygrał loterię?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie byłoby najbardziej nietypowe hobby ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby spotkał kosmitę?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zawsze chce spróbować, ale się boi?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł zamienić się ciałem z kimś na dzień?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** powiedziałby swojemu przyszłemu „ja” z 2050 roku?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak by wyglądał wymarzony dom ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** robi, żeby poprawić sobie humor?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jaką najbardziej absurdalną wymówkę wymyśliłby ****, żeby nie przyjść do pracy/szkoły?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jak wyglądałby profil randkowy ****?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby mógł rozmawiać ze zwierzętami?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobił na ostatniej imprezie, co wszyscy pamiętają?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jaką najbardziej absurdalną rzecz kupiłby ****, gdyby miał nieograniczony budżet?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jaką najgłupszą rzecz mógłby zrobić **** po pijaku?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby został bohaterem filmu akcji?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** najczęściej robi w pracy lub szkole, zamiast pracować?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakie jest najbardziej „****owe” powiedzonko?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby znalazł walizkę z milionem złotych?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** zrobiłby, gdyby był przez jeden dzień niewidzialny?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Jakim zwierzęciem byłby ****, gdyby mógł się zamienić?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Co **** mógłby robić zawodowo, gdyby nie to, co robi teraz?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Z kim sławnym **** chciałby zjeść kolację?"),
            // ---------------------------
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakie jest ulubione ciasto gracza ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki fast food **** lubi najbardziej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "O czym najczęściej **** zapomina?"),
            // "Ile **** chce mieć dzieci?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Czego **** wstydzi się najbardziej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Z iloma osobami **** się spotkał w życiu za pomocą aplikacji randkowych?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Z jakiej dziedziny **** ma największą wiedzę?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakiego modelu telefonu **** aktualnie używa?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaka była pierwsza praca ****?"),
            // "Gdzie **** lubi być najbardziej masowany?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki **** ma ulubiony smak lodów?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaką czynność domową **** najmniej lubi?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaką kawę **** najczęściej pije?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakim **** jest kierowcą w skali od 1 do 5?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "O co najczęściej **** potrafi się obrazić?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaką zbędną rzecz **** ostatnio kupił?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaka jest ulubiona piosenka ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Według ****, jakie samochody są najlepsze?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakiego przekleństwa **** używa najczęściej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakiego słowa **** ostatnio nadużywa?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki jest ulubiony serial ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki alkohol **** najczęściej pije?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Co aktualnie najbardziej stresuje ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Bez jakiego sprzętu **** nie wyobraża sobie życia?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "W jaką grę **** gra najczęściej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jak **** ocenia swoje zdolności kulinarne w skali od 1 do 5?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki jest największy sukces ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki jest ulubiony youtuber ****?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Na co **** wydaje najwięcej pieniędzy?"),
            // "Której części ciała **** najbardziej w sobie nie lubi?"
            (["PUBLIC_CATALOG_2"], ["prawda"], "Na czyj koncert **** najchętniej pójdzie w najbliższym czasie?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakie **** ma największe marzenie podróżnicze?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Czego **** nigdy by nie zjadł?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Czego **** chce się nauczyć?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Z kim **** najczęściej rozmawia przez telefon?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Na co zazwyczaj brakuje czasu ****?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Co **** zazwyczaj boli?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Co **** lubi robić w wolnym czasie?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakiego gatunku muzyki **** najbardziej nie lubi?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki sport najbardziej emocjonuje ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakie wino **** lubi najbardziej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Ile kaw **** pije w ciągu dnia?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Gdzie **** zrobiłby sobie tatuaż?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Co **** chciałby dostać na urodziny?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakie jest popisowe danie ****?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Jaka mała rzecz potrafi poprawić humor ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jak się wabiło pierwsze zwierzę ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Który posiłek w ciągu dnia **** czasem pomija?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Co **** zawsze przy sobie nosi?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Co **** chciałby zrobić ale się boi?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Na co **** oszczędza pieniądze?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Którą z Twoich koleżanek on najbardziej lubi?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Którego z Twoich kolegów ona najbardziej lubi?"),
            (["PUBLIC_CATALOG_3"], ["para"], "W czym ostatnio on przyznał Ci rację?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jak **** ocenia swoją sylwetkę od 1 do 5?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Który miesiąc **** lubi najbardziej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Komu **** zazwyczaj powierza swoje sekrety?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Jakich ubrań **** ma w szafie najwięcej?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Czego **** by nigdy nie wybaczył?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Z jakiego tematu **** nie lubi żartować?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Jak **** chciałby spędzić wieczór?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co on najbardziej lubi z Tobą robić?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co ona najbardziej lubi z Tobą robić?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co on w Tobie najbardziej nie lubi?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co ona w Tobie najbardziej nie lubi?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co on w Tobie najbardziej lubi?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co ona w Tobie najbardziej lubi?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Co ostatnio **** zrobił dla siebie?"),
            (["PUBLIC_CATALOG_1"], ["luźne"], "Co ostatnio **** zrobił dla siebie?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "W jakim zawodzie **** chciałby pracować?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakie danie wigilijne **** najbardziej lubi?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Co zwykle **** robi wieczorem przed pójsciem spać?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Czego **** najbardziej nie lubi robić w domu?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Gdzie **** chce zamieszkać na starość?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Gdzie **** chce zamieszkać na starość?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Ile **** miał lat gdy pierwszy raz pił mocniejszy alkohol?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaką pozytywną cechę **** ma po mamie?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Czego on Ci zazdrości?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Czego ona Ci zazdrości?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaką markę odzieżową **** najbardziej lubi?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Jakie danie byłoby nazwane „Specjałem ****”?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Na jaki kolor **** chciałby przefarbować włosy?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Czego **** najbardziej zazdrości sąsiadowi?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Czy **** był kiedyś na striptizie?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "W czym **** realizuje się najbardziej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "W czym **** realizuje się najbardziej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki kraj **** najbardziej chce odwiedzić?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jakie danie z grilla **** lubi najbardziej?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Ile **** miał poważnych związków?"),
            (["INTYMNE"], ["prawda"], "O jakiej porze dnia **** ma największą ochotę na figle?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Kogo **** najbardziej podziwia?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Za co on Cię ostatnio pochwalił?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Za co ona Cię ostatnio pochwaliła?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Ile ona chciałaby żebyś zarabiał?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Ile on chciałby żebyś zarabiała?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Gdzie ona uwielbia być przez Ciebie całowana?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Gdzie on uwielbia być przez Ciebie całowany?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "O której **** zazwyczaj wstaje?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "O której **** zazwyczaj wstaje?"),
            (["PUBLIC_CATALOG_1", "PUBLIC_CATALOG_2"], ["prawda", "luzne"], "Na co **** marnuje najwięcej czasu?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co on chciałby teraz od Ciebie usłyszeć?"),
            (["PUBLIC_CATALOG_3"], ["para"], "Co ona chciałaby teraz od Ciebie usłyszeć?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Czego z życia singla **** najbardziej brakuje?"),
            (["PUBLIC_CATALOG_1"], ["luzne"], "Czego z życia singla **** najbardziej brakuje?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jak miał na imię pierwszy przyjaciel ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Jaki jest ulubiony gadżet ****?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Gdzie **** był ostatnio na randce?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "Kiedy **** był ostatnio na randce?"),
            (["PUBLIC_CATALOG_2"], ["prawda"], "O której **** zazwyczaj wstaje?"),
        }; // TODO: obsluzyc prawda luzne para

        var questions = new List<Question>();

        foreach (var (Catalogs, _, Text) in templates)
        {
            questions.Add(new Question
            {
                Text = Text,
                IsPublic = true,
                Language = Language.PL,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId,
                Catalogs = [.. catalogs.Where(c => Catalogs.Contains(c.Title))]
            });
        }

        return questions;
    }

}
