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
                Title = "Intymne",
                CatalogTypeId = (int)CatalogTypeName.Draft,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            },
            new()
            {
                Title = "Impreza",
                CatalogTypeId = (int)CatalogTypeName.Public,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            },
            new()
            {
                Title = "Poznanie się",
                CatalogTypeId = (int)CatalogTypeName.Public,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId
            },
            new()
            {
                Title = "Dla par",
                CatalogTypeId = (int)CatalogTypeName.Draft,
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
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł nagrać wspólne reality show z tą grupą?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wybrać piosenkę opisującą tę ekipę?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby był postacią z musicalu?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby został sędzią w konkursie talentów graczy?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś próbuje go przekupić ciastkiem?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby miał stworzyć własny znak zodiaku?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby był głosem nawigacji GPS?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał przez dzień być menadżerem tej grupy?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby ktoś pomylił go z celebrytą?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wygłosić mowę na weselu przyjaciela?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby był figurką w planszówce?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy zaczęli mówić jego cytatami?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy słyszy swoje imię w plotkach?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł zaprojektować wspólną koszulkę drużynową?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby jako postać z „Gry o tron”?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby był reżyserem filmu o tej ekipie?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy coś idzie nie po jego myśli?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wystąpić w teledysku grupowym?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby jako postać z anime?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy mówili do niego jak do króla?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś nie śmieje się z jego żartów?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego imię trafiło do quizu wiedzy o celebrytach?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby był maskotką tej grupy?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał napisać poradnik „Jak przetrwać z tą ekipą”?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś prosi go o radę w absurdalnej sytuacji?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wystąpić w teleturnieju z resztą graczy jako drużyną?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby został bohaterem reklamy napoju energetycznego?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby jako influencer motywacyjny?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby musiał napisać piosenkę o każdym graczu?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby był prowadzącym teleturniej o absurdalnych pytaniach?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby był głównym bohaterem bajki o przyjaźni?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wybrać, kto z tej grupy byłby jego ochroniarzem?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby wszyscy zaczęli go parodiować?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł spędzić jeden dzień w głowie innego gracza?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby w wersji superstylowej na czerwonym dywanie?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wymyślić nowy taniec narodowy tej grupy?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś wspomina jego dawne wpadki?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby jako złoczyńca w bajce?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał poprowadzić pogadankę o miłości?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy dostaje nieoczekiwany prezent?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego jedynym sposobem komunikacji był taniec?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby musiał zaśpiewać karaoke bez znajomości tekstu?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby został burmistrzem miasta, w którym mieszkają gracze?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby był postacią w grze „The Sims”?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wymyślić nowy świąteczny zwyczaj?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś próbuje go zawstydzić?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby musiał przez dzień mówić tylko rymami?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś zaczyna śpiewać jego ulubioną piosenkę?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby, gdyby był postacią z bajki Disneya?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego życie miało tytuł filmowy?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby ktoś stworzył o nim mema?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy mówili do niego wierszem przez godzinę?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś zaczyna tańczyć obok niego?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał napisać biografię któregoś z graczy?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby w filmie o tej grupie?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby był duchem, nawiedzającym znajomych?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wymyślić motto dla tej grupy?"),
            (["Impreza"], ["luzne"], "Jak **** brzmiałby jako lektor audiobooków?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał przekonać wszystkich tutaj, że jest kosmitą?"),
            (["Impreza"], ["luzne"], "Co **** mówi, gdy próbuje wybrnąć z kłopotliwej sytuacji?"),
            (["Impreza"], ["luzne"], "Jak **** zachowywałby się jako boss w grze komputerowej?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby cała ta grupa zamieniła się w zombie?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby miał poprowadzić ślub jednej z osób stąd?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał opowiedzieć żart na poważnym pogrzebie?"),
            (["Impreza"], ["luzne"], "Jak **** wyglądałby jako emoji?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby wszyscy z tej grupy byli jego podwładnymi?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby mógł żyć w dowolnym filmie przez tydzień?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby został bohaterem gry RPG, a jego klasa to „Lenistwo”?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby musiał przez dzień mówić w stylu komentatora sportowego?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wybrać partnera do apokalipsy spośród tej grupy?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś próbuje go poderwać na imprezie?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby został zamieniony w pluszowego misia?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby jego głos został podmieniony na głos dziecka?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby jego myśli były wyświetlane nad głową jak napisy?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał zostać bohaterem komedii romantycznej?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby jego ulubiona postać fikcyjna stała się prawdziwa?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby musiał wystąpić w konkursie talentów, ale bez talentu?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby jego głos zmieniał się za każdym razem, gdy się zdenerwuje?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał stworzyć nowy sport?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby jego odbicie zaczęło mieć własne zdanie?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby każdy jego ruch był komentowany przez narratora?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego ulubiony film stał się rzeczywistością?"),
            (["Impreza"], ["luzne"], "Jak **** zareagowałby, gdyby każdy dzień zaczynał się od losowego supermocy?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł rozmawiać z duchami?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby musiał wystąpić w reklamie majonezu?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł sprawić, że jedna rzecz na świecie zniknie na zawsze?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby został uwięziony w windzie z kimś sławnym?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby nagle znalazł się w świecie baśni?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby mógł zamieniać się miejscami z dowolną osobą przez 10 minut dziennie?"),
            (["Impreza"], ["luzne"], "Jak **** zareagowałby, gdyby dostał magiczny długopis, który spełnia życzenia?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby każdy jego żart stawał się prawdą?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby musiał nosić kostium banana przez cały dzień?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby jego dzień powtarzał się w kółko jak w „Dniu świstaka”?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał wystąpić w reality show „Przetrwaj tydzień bez internetu”?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby został uwięziony w muzeum w nocy?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł mieć własną wyspę?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby trafił do średniowiecza z telefonem i powerbankiem?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego sny zaczęły się spełniać w realu?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby mógł wymyślić nowe prawo dla całego świata?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby nagle musiał wystąpić na Eurowizji?"),
            (["Impreza"], ["luzne"], "Jak **** reagowałby, gdyby mógł mówić tylko cytatami z filmów?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby dostał list od swojego przyszłego „ja”?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby dostał magiczny pilot do zatrzymywania czasu?"),
            (["Impreza"], ["luzne"], "Jak **** uratowałby świat, gdyby miał supermoc, której nikt nie rozumie?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł rozmawiać z roślinami?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby przypadkiem został celebrytą?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł teleportować się tylko w miejsca, w których już kiedyś zasnął?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł zamieniać ludzi w dowolne zwierzęta?"),
            (["Impreza"], ["luzne"], "Jak **** zareagowałby, gdyby został królem memów w internecie?"),
            (["Impreza"], ["luzne"], "Jak **** zachowałby się, gdyby dostał list z Hogwartu… ale jako nauczyciel?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł cofnąć się do jednego dnia z dzieciństwa?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał mieszkać w lodówce przez tydzień?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby został porwany przez kosmitów, którzy lubią disco polo?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego najlepszy przyjaciel okazał się robotem?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby został wciągnięty do gry komputerowej?"),
            (["Impreza"], ["luzne"], "Jak **** poradziłby sobie, gdyby został zamknięty w centrum handlowym na noc?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby nagle potrafił czytać myśli innych?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś mu powie „musimy pogadać”?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał iść na randkę z kimś z tej grupy?"),
            (["Impreza"], ["luzne"], "Co **** mówi, gdy naprawdę nie ma siły na ludzi?"),
            (["Impreza"], ["luzne"], "Jak **** zachowuje się, gdy dostaje niespodziewany komplement?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego rodzice przeczytali jego czaty?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś daje mu zły prezent?"),
            (["Impreza"], ["luzne"], "Co **** mówi, gdy próbuje być grzeczny, ale już nie może?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś wyciąga stare kompromitujące zdjęcia?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby miał odegrać scenę romantyczną na scenie?"),
            (["Impreza"], ["luzne"], "Co **** mówi, gdy próbuje uniknąć niezręcznej ciszy?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał spędzić dzień z osobą, której nie lubi?"),
            (["Impreza"], ["luzne"], "Co **** mówi, gdy próbuje wyjść z kłopotliwej sytuacji?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś opowiada żart, który nie jest śmieszny?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby jego przyjaciel wyznał mu, że jest w nim zakochany?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś mu gratuluje za coś, czego nie zrobił?"),
            (["Impreza"], ["luzne"], "Co **** mówi, gdy naprawdę nie wie, o co chodzi w rozmowie?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby ktoś poprosił go o taniec w środku ulicy?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby musiał napisać swoje bio na Tinderze?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby przypadkiem podsłuchał rozmowę o sobie?"),
            (["Impreza"], ["luzne"], "Jak **** reaguje, gdy ktoś mu powie: „Nie jesteś taki, jak myślałem”?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby ktoś napisał o nim piosenkę miłosną?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby musiał randkować z kimś zupełnie swoim przeciwieństwem?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby dostał 5 minut na przekonanie kogoś, że jest niesamowity?"),
            (["Impreza"], ["luzne"], "Jak wyglądałaby idealna randka wymyślona przez ****?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby przypadkiem wysłał flirtującego SMS-a do złej osoby?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby świat, gdyby wszyscy zachowywali się jak ****?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby spotkał samego siebie z innego wymiaru?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby idealny dzień wakacji według ****?"),
            (["Impreza"], ["luzne"], "Jakie jedzenie mogłoby zostać symbolem ****?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby dostał list od samego siebie z przyszłości?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby plakat filmu o życiu ****?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby świat, gdyby wszyscy mieli poczucie humoru jak ****?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby został prezenterem prognozy pogody?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby każdy jego ruch miał dźwięk jak w kreskówkach?"),
            (["Impreza"], ["luzne"], "Jakie byłoby najgorsze motto firmy założonej przez ****?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby poranek **** w alternatywnym wszechświecie?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby dostał list z Hogwartu?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby musiał prowadzić wesele?"),
            (["Impreza"], ["luzne"], "Jakie byłoby hasło kampanii reklamowej stworzonej przez ****?"),
            (["Impreza"], ["luzne"], "Jaka byłaby nazwa zespołu muzycznego założonego przez ****?"),
            (["Impreza"], ["luzne"], "Jakie byłoby motto religii założonej przez ****?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby został spytany o sens życia w telewizji na żywo?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby wygrał dożywotni zapas chipsów?"),
            (["Impreza"], ["luzne"], "Jak wyglądałaby książka kucharska napisana przez ****?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby obudził się w ciele sławnej osoby?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby film o przyjaźni między **** a lodówką?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby mógł wysłać SMS-a do całego świata?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby kosmicznemu przybyszowi na powitanie?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł kontrolować pogodę?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby vlog podróżniczy ****?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby znalazł magiczną lampę z dżinem?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby dzień **** w średniowiecznym zamku?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby był prowadzącym „Familiadę”?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby sklep prowadzony przez ****?"),
            (["Impreza"], ["luzne"], "Jakie byłoby najgorsze możliwe hobby dla ****?"),
            (["Impreza"], ["luzne"], "Jakie byłoby ulubione zaklęcie **** w świecie Harry’ego Pottera?"),
            (["Impreza"], ["luzne"], "Jakie byłoby najgorsze hasło motywacyjne wymyślone przez ****?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł przez jeden dzień łamać wszystkie zasady?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby film dokumentalny o życiu ****?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby spotkał swoje 10-letnie „ja”?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby tatuaż, który idealnie pasuje do ****?"),
            (["Impreza"], ["luzne"], "Jakie jest najbardziej szalone marzenie **** z dzieciństwa?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby dzień **** w średniowieczu?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby wygrał darmowe bilety w jedną stronę w nieznane?"),
            (["Impreza"], ["luzne"], "Jaką bajkę **** oglądałby po cichu, żeby nikt nie widział?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby poranek **** po wygraniu miliona?"),
            (["Impreza"], ["luzne"], "Jaką piosenkę **** śpiewałby na karaoke?"),
            (["Impreza"], ["luzne"], "Co **** najczęściej odkłada „na później”?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby, gdyby wygrał Oscara?"),
            (["Impreza"], ["luzne"], "Co **** robi w sobotnie wieczory, gdy nikt nie ma planów?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby wygrał loterię?"),
            (["Impreza"], ["luzne"], "Jakie byłoby najbardziej nietypowe hobby ****?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby spotkał kosmitę?"),
            (["Impreza"], ["luzne"], "Co **** zawsze chce spróbować, ale się boi?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł zamienić się ciałem z kimś na dzień?"),
            (["Impreza"], ["luzne"], "Co **** powiedziałby swojemu przyszłemu „ja” z 2050 roku?"),
            (["Impreza"], ["luzne"], "Jak by wyglądał wymarzony dom ****?"),
            (["Impreza"], ["luzne"], "Co **** robi, żeby poprawić sobie humor?"),
            (["Impreza"], ["luzne"], "Jaką najbardziej absurdalną wymówkę wymyśliłby ****, żeby nie przyjść do pracy/szkoły?"),
            (["Impreza"], ["luzne"], "Jak wyglądałby profil randkowy ****?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby mógł rozmawiać ze zwierzętami?"),
            (["Impreza"], ["luzne"], "Co **** zrobił na ostatniej imprezie, co wszyscy pamiętają?"),
            (["Impreza"], ["luzne"], "Jaką najbardziej absurdalną rzecz kupiłby ****, gdyby miał nieograniczony budżet?"),
            (["Impreza"], ["luzne"], "Jaką najgłupszą rzecz mógłby zrobić **** po pijaku?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby został bohaterem filmu akcji?"),
            (["Impreza"], ["luzne"], "Co **** najczęściej robi w pracy lub szkole, zamiast pracować?"),
            (["Impreza"], ["luzne"], "Jakie jest najbardziej „****owe” powiedzonko?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby znalazł walizkę z milionem złotych?"),
            (["Impreza"], ["luzne"], "Co **** zrobiłby, gdyby był przez jeden dzień niewidzialny?"),
            (["Impreza"], ["luzne"], "Jakim zwierzęciem byłby ****, gdyby mógł się zamienić?"),
            (["Impreza"], ["luzne"], "Co **** mógłby robić zawodowo, gdyby nie to, co robi teraz?"),
            (["Impreza"], ["luzne"], "Z kim sławnym **** chciałby zjeść kolację?"),
            // ---------------------------
            (["Poznanie się"], ["prawda"], "Jakie jest ulubione ciasto gracza ****?"),
            (["Poznanie się"], ["prawda"], "Jaki fast food **** lubi najbardziej?"),
            (["Poznanie się"], ["prawda"], "O czym najczęściej **** zapomina?"),
            // "Ile **** chce mieć dzieci?"),
            (["Poznanie się"], ["prawda"], "Czego **** wstydzi się najbardziej?"),
            (["Poznanie się"], ["prawda"], "Z iloma osobami **** się spotkał w życiu za pomocą aplikacji randkowych?"),
            (["Poznanie się"], ["prawda"], "Z jakiej dziedziny **** ma największą wiedzę?"),
            (["Poznanie się"], ["prawda"], "Jakiego modelu telefonu **** aktualnie używa?"),
            (["Poznanie się"], ["prawda"], "Jaka była pierwsza praca ****?"),
            // "Gdzie **** lubi być najbardziej masowany?"),
            (["Poznanie się"], ["prawda"], "Jaki **** ma ulubiony smak lodów?"),
            (["Poznanie się"], ["prawda"], "Jaką czynność domową **** najmniej lubi?"),
            (["Poznanie się"], ["prawda"], "Jaką kawę **** najczęściej pije?"),
            (["Poznanie się"], ["prawda"], "Jakim **** jest kierowcą w skali od 1 do 5?"),
            (["Poznanie się"], ["prawda"], "O co najczęściej **** potrafi się obrazić?"),
            (["Poznanie się"], ["prawda"], "Jaką zbędną rzecz **** ostatnio kupił?"),
            (["Poznanie się"], ["prawda"], "Jaka jest ulubiona piosenka ****?"),
            (["Poznanie się"], ["prawda"], "Według ****, jakie samochody są najlepsze?"),
            (["Poznanie się"], ["prawda"], "Jakiego przekleństwa **** używa najczęściej?"),
            (["Poznanie się"], ["prawda"], "Jakiego słowa **** ostatnio nadużywa?"),
            (["Poznanie się"], ["prawda"], "Jaki jest ulubiony serial ****?"),
            (["Poznanie się"], ["prawda"], "Jaki alkohol **** najczęściej pije?"),
            (["Poznanie się"], ["prawda"], "Co aktualnie najbardziej stresuje ****?"),
            (["Poznanie się"], ["prawda"], "Bez jakiego sprzętu **** nie wyobraża sobie życia?"),
            (["Poznanie się"], ["prawda"], "W jaką grę **** gra najczęściej?"),
            (["Poznanie się"], ["prawda"], "Jak **** ocenia swoje zdolności kulinarne w skali od 1 do 5?"),
            (["Poznanie się"], ["prawda"], "Jaki jest największy sukces ****?"),
            (["Poznanie się"], ["prawda"], "Jaki jest ulubiony youtuber ****?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Na co **** wydaje najwięcej pieniędzy?"),
            // "Której części ciała **** najbardziej w sobie nie lubi?"
            (["Poznanie się"], ["prawda"], "Na czyj koncert **** najchętniej pójdzie w najbliższym czasie?"),
            (["Poznanie się"], ["prawda"], "Jakie **** ma największe marzenie podróżnicze?"),
            (["Poznanie się"], ["prawda"], "Czego **** nigdy by nie zjadł?"),
            (["Poznanie się"], ["prawda"], "Czego **** chce się nauczyć?"),
            (["Poznanie się"], ["prawda"], "Z kim **** najczęściej rozmawia przez telefon?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Na co zazwyczaj brakuje czasu ****?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Co **** zazwyczaj boli?"),
            (["Poznanie się"], ["prawda"], "Co **** lubi robić w wolnym czasie?"),
            (["Poznanie się"], ["prawda"], "Jakiego gatunku muzyki **** najbardziej nie lubi?"),
            (["Poznanie się"], ["prawda"], "Jaki sport najbardziej emocjonuje ****?"),
            (["Poznanie się"], ["prawda"], "Jakie wino **** lubi najbardziej?"),
            (["Poznanie się"], ["prawda"], "Ile kaw **** pije w ciągu dnia?"),
            (["Poznanie się"], ["prawda"], "Gdzie **** zrobiłby sobie tatuaż?"),
            (["Poznanie się"], ["prawda"], "Co **** chciałby dostać na urodziny?"),
            (["Poznanie się"], ["prawda"], "Jakie jest popisowe danie ****?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Jaka mała rzecz potrafi poprawić humor ****?"),
            (["Poznanie się"], ["prawda"], "Jak się wabiło pierwsze zwierzę ****?"),
            (["Poznanie się"], ["prawda"], "Który posiłek w ciągu dnia **** czasem pomija?"),
            (["Poznanie się"], ["prawda"], "Co **** zawsze przy sobie nosi?"),
            (["Poznanie się"], ["prawda"], "Co **** chciałby zrobić ale się boi?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Na co **** oszczędza pieniądze?"),
            (["Dla par"], ["para"], "Którą z Twoich koleżanek on najbardziej lubi?"),
            (["Dla par"], ["para"], "Którego z Twoich kolegów ona najbardziej lubi?"),
            (["Dla par"], ["para"], "W czym ostatnio on przyznał Ci rację?"),
            (["Poznanie się"], ["prawda"], "Jak **** ocenia swoją sylwetkę od 1 do 5?"),
            (["Poznanie się"], ["prawda"], "Który miesiąc **** lubi najbardziej?"),
            (["Poznanie się"], ["prawda"], "Komu **** zazwyczaj powierza swoje sekrety?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Jakich ubrań **** ma w szafie najwięcej?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Czego **** by nigdy nie wybaczył?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Z jakiego tematu **** nie lubi żartować?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Jak **** chciałby spędzić wieczór?"),
            (["Dla par"], ["para"], "Co on najbardziej lubi z Tobą robić?"),
            (["Dla par"], ["para"], "Co ona najbardziej lubi z Tobą robić?"),
            (["Dla par"], ["para"], "Co on w Tobie najbardziej nie lubi?"),
            (["Dla par"], ["para"], "Co ona w Tobie najbardziej nie lubi?"),
            (["Dla par"], ["para"], "Co on w Tobie najbardziej lubi?"),
            (["Dla par"], ["para"], "Co ona w Tobie najbardziej lubi?"),
            (["Poznanie się"], ["prawda"], "Co ostatnio **** zrobił dla siebie?"),
            (["Impreza"], ["luźne"], "Co ostatnio **** zrobił dla siebie?"),
            (["Poznanie się"], ["prawda"], "W jakim zawodzie **** chciałby pracować?"),
            (["Poznanie się"], ["prawda"], "Jakie danie wigilijne **** najbardziej lubi?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Co zwykle **** robi wieczorem przed pójsciem spać?"),
            (["Poznanie się"], ["prawda"], "Czego **** najbardziej nie lubi robić w domu?"),
            (["Poznanie się"], ["prawda"], "Gdzie **** chce zamieszkać na starość?"),
            (["Impreza"], ["luzne"], "Gdzie **** chce zamieszkać na starość?"),
            (["Poznanie się"], ["prawda"], "Ile **** miał lat gdy pierwszy raz pił mocniejszy alkohol?"),
            (["Poznanie się"], ["prawda"], "Jaką pozytywną cechę **** ma po mamie?"),
            (["Dla par"], ["para"], "Czego on Ci zazdrości?"),
            (["Dla par"], ["para"], "Czego ona Ci zazdrości?"),
            (["Poznanie się"], ["prawda"], "Jaką markę odzieżową **** najbardziej lubi?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Jakie danie byłoby nazwane „Specjałem ****”?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Na jaki kolor **** chciałby przefarbować włosy?"),
            (["Impreza"], ["luzne"], "Czego **** najbardziej zazdrości sąsiadowi?"),
            (["Poznanie się"], ["prawda"], "Czy **** był kiedyś na striptizie?"),
            (["Impreza"], ["luzne"], "W czym **** realizuje się najbardziej?"),
            (["Poznanie się"], ["prawda"], "W czym **** realizuje się najbardziej?"),
            (["Poznanie się"], ["prawda"], "Jaki kraj **** najbardziej chce odwiedzić?"),
            (["Poznanie się"], ["prawda"], "Jakie danie z grilla **** lubi najbardziej?"),
            (["Poznanie się"], ["prawda"], "Ile **** miał poważnych związków?"),
            (["Intymne"], ["prawda"], "O jakiej porze dnia **** ma największą ochotę na figle?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Kogo **** najbardziej podziwia?"),
            (["Dla par"], ["para"], "Za co on Cię ostatnio pochwalił?"),
            (["Dla par"], ["para"], "Za co ona Cię ostatnio pochwaliła?"),
            (["Dla par"], ["para"], "Ile ona chciałaby żebyś zarabiał?"),
            (["Dla par"], ["para"], "Ile on chciałby żebyś zarabiała?"),
            (["Dla par"], ["para"], "Gdzie ona uwielbia być przez Ciebie całowana?"),
            (["Dla par"], ["para"], "Gdzie on uwielbia być przez Ciebie całowany?"),
            (["Poznanie się"], ["prawda"], "O której **** zazwyczaj wstaje?"),
            (["Impreza"], ["luzne"], "O której **** zazwyczaj wstaje?"),
            (["Impreza", "Poznanie się"], ["prawda", "luzne"], "Na co **** marnuje najwięcej czasu?"),
            (["Dla par"], ["para"], "Co on chciałby teraz od Ciebie usłyszeć?"),
            (["Dla par"], ["para"], "Co ona chciałaby teraz od Ciebie usłyszeć?"),
            (["Poznanie się"], ["prawda"], "Czego z życia singla **** najbardziej brakuje?"),
            (["Impreza"], ["luzne"], "Czego z życia singla **** najbardziej brakuje?"),
            (["Poznanie się"], ["prawda"], "Jak miał na imię pierwszy przyjaciel ****?"),
            (["Poznanie się"], ["prawda"], "Jaki jest ulubiony gadżet ****?"),
            (["Poznanie się"], ["prawda"], "Gdzie **** był ostatnio na randce?"),
            (["Poznanie się"], ["prawda"], "Kiedy **** był ostatnio na randce?"),
            (["Poznanie się"], ["prawda"], "O której **** zazwyczaj wstaje?"),
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
