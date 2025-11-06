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

        IEnumerable<Question> questions = GetQuestions(user.UserId);
        await _context.Questions.AddRangeAsync(questions);
    }

    private static List<Question> GetQuestions(int userId)
    {
        var templates = new List<string>
        {
            "Co **** zrobiłby, gdyby mógł nagrać wspólne reality show z tą grupą?",
            "Co **** zrobiłby, gdyby musiał wybrać piosenkę opisującą tę ekipę?",
            "Jak **** wyglądałby, gdyby był postacią z musicalu?",
            "Co **** zrobiłby, gdyby został sędzią w konkursie talentów graczy?",
            "Jak **** reaguje, gdy ktoś próbuje go przekupić ciastkiem?",
            "Co **** zrobiłby, gdyby miał stworzyć własny znak zodiaku?",
            "Jak **** wyglądałby, gdyby był głosem nawigacji GPS?",
            "Co **** zrobiłby, gdyby musiał przez dzień być menadżerem tej grupy?",
            "Jak **** reagowałby, gdyby ktoś pomylił go z celebrytą?",
            "Co **** zrobiłby, gdyby musiał wygłosić mowę na weselu przyjaciela?",
            "Jak **** wyglądałby, gdyby był figurką w planszówce?",
            "Co **** zrobiłby, gdyby wszyscy zaczęli mówić jego cytatami?",
            "Jak **** reaguje, gdy słyszy swoje imię w plotkach?",
            "Co **** zrobiłby, gdyby mógł zaprojektować wspólną koszulkę drużynową?",
            "Jak **** wyglądałby jako postać z „Gry o tron”?",
            "Co **** zrobiłby, gdyby był reżyserem filmu o tej ekipie?",
            "Jak **** reaguje, gdy coś idzie nie po jego myśli?",
            "Co **** zrobiłby, gdyby musiał wystąpić w teledysku grupowym?",
            "Jak **** wyglądałby jako postać z anime?",
            "Co **** zrobiłby, gdyby wszyscy mówili do niego jak do króla?",
            "Jak **** reaguje, gdy ktoś nie śmieje się z jego żartów?",
            "Co **** zrobiłby, gdyby jego imię trafiło do quizu wiedzy o celebrytach?",
            "Jak **** wyglądałby, gdyby był maskotką tej grupy?",
            "Co **** zrobiłby, gdyby musiał napisać poradnik „Jak przetrwać z tą ekipą”?",
            "Jak **** reaguje, gdy ktoś prosi go o radę w absurdalnej sytuacji?",
            "Co **** zrobiłby, gdyby musiał wystąpić w teleturnieju z resztą graczy jako drużyną?",
            "Jak **** wyglądałby, gdyby został bohaterem reklamy napoju energetycznego?",
            "Jak **** wyglądałby jako influencer motywacyjny?",
            "Jak **** zachowałby się, gdyby musiał napisać piosenkę o każdym graczu?",
            "Co **** zrobiłby, gdyby był prowadzącym teleturniej o absurdalnych pytaniach?",
            "Jak **** wyglądałby, gdyby był głównym bohaterem bajki o przyjaźni?",
            "Co **** zrobiłby, gdyby musiał wybrać, kto z tej grupy byłby jego ochroniarzem?",
            "Jak **** reagowałby, gdyby wszyscy zaczęli go parodiować?",
            "Co **** zrobiłby, gdyby mógł spędzić jeden dzień w głowie innego gracza?",
            "Jak **** wyglądałby w wersji superstylowej na czerwonym dywanie?",
            "Co **** zrobiłby, gdyby musiał wymyślić nowy taniec narodowy tej grupy?",
            "Jak **** reaguje, gdy ktoś wspomina jego dawne wpadki?",
            "Jak **** wyglądałby jako złoczyńca w bajce?",
            "Co **** zrobiłby, gdyby musiał poprowadzić pogadankę o miłości?",
            "Jak **** reaguje, gdy dostaje nieoczekiwany prezent?",
            "Co **** zrobiłby, gdyby jego jedynym sposobem komunikacji był taniec?",
            "Jak **** zachowałby się, gdyby musiał zaśpiewać karaoke bez znajomości tekstu?",
            "Co **** zrobiłby, gdyby został burmistrzem miasta, w którym mieszkają gracze?",
            "Jak **** wyglądałby, gdyby był postacią w grze „The Sims”?",
            "Co **** zrobiłby, gdyby musiał wymyślić nowy świąteczny zwyczaj?",
            "Jak **** reaguje, gdy ktoś próbuje go zawstydzić?",
            "Jak **** zachowałby się, gdyby musiał przez dzień mówić tylko rymami?",
            "Jak **** reaguje, gdy ktoś zaczyna śpiewać jego ulubioną piosenkę?",
            "Jak **** wyglądałby, gdyby był postacią z bajki Disneya?",
            "Co **** zrobiłby, gdyby jego życie miało tytuł filmowy?",
            "Jak **** reagowałby, gdyby ktoś stworzył o nim mema?",
            "Co **** zrobiłby, gdyby wszyscy mówili do niego wierszem przez godzinę?",
            "Jak **** reaguje, gdy ktoś zaczyna tańczyć obok niego?",
            "Co **** zrobiłby, gdyby musiał napisać biografię któregoś z graczy?",
            "Jak **** wyglądałby w filmie o tej grupie?",
            "Co **** zrobiłby, gdyby był duchem, nawiedzającym znajomych?",
            "Co **** zrobiłby, gdyby musiał wymyślić motto dla tej grupy?",
            "Jak **** brzmiałby jako lektor audiobooków?",
            "Co **** zrobiłby, gdyby musiał przekonać wszystkich tutaj, że jest kosmitą?",
            "Co **** mówi, gdy próbuje wybrnąć z kłopotliwej sytuacji?",
            "Jak **** zachowywałby się jako boss w grze komputerowej?",
            "Co **** zrobiłby, gdyby cała ta grupa zamieniła się w zombie?",
            "Jak **** reagowałby, gdyby miał poprowadzić ślub jednej z osób stąd?",
            "Co **** zrobiłby, gdyby musiał opowiedzieć żart na poważnym pogrzebie?",
            "Jak **** wyglądałby jako emoji?",
            "Co **** zrobiłby, gdyby wszyscy z tej grupy byli jego podwładnymi?",
            "Jak **** zachowałby się, gdyby mógł żyć w dowolnym filmie przez tydzień?",
            "Co **** zrobiłby, gdyby został bohaterem gry RPG, a jego klasa to „Lenistwo”?",
            "Jak **** reagowałby, gdyby musiał przez dzień mówić w stylu komentatora sportowego?",
            "Co **** zrobiłby, gdyby musiał wybrać partnera do apokalipsy spośród tej grupy?",
            "Jak **** reaguje, gdy ktoś próbuje go poderwać na imprezie?",
            "Co **** zrobiłby, gdyby został zamieniony w pluszowego misia?",
            "Jak **** reagowałby, gdyby jego głos został podmieniony na głos dziecka?",
            "Jak **** poradziłby sobie, gdyby jego myśli były wyświetlane nad głową jak napisy?",
            "Co **** zrobiłby, gdyby musiał zostać bohaterem komedii romantycznej?",
            "Jak **** reagowałby, gdyby jego ulubiona postać fikcyjna stała się prawdziwa?",
            "Jak **** zachowałby się, gdyby musiał wystąpić w konkursie talentów, ale bez talentu?",
            "Jak **** zachowałby się, gdyby jego głos zmieniał się za każdym razem, gdy się zdenerwuje?",
            "Co **** zrobiłby, gdyby musiał stworzyć nowy sport?",
            "Jak **** reagowałby, gdyby jego odbicie zaczęło mieć własne zdanie?",
            "Jak **** poradziłby sobie, gdyby każdy jego ruch był komentowany przez narratora?",
            "Co **** zrobiłby, gdyby jego ulubiony film stał się rzeczywistością?",
            "Jak **** zareagowałby, gdyby każdy dzień zaczynał się od losowego supermocy?",
            "Co **** zrobiłby, gdyby mógł rozmawiać z duchami?",
            "Jak **** poradziłby sobie, gdyby musiał wystąpić w reklamie majonezu?",
            "Co **** zrobiłby, gdyby mógł sprawić, że jedna rzecz na świecie zniknie na zawsze?",
            "Co **** zrobiłby, gdyby został uwięziony w windzie z kimś sławnym?",
            "Jak **** zachowałby się, gdyby nagle znalazł się w świecie baśni?",
            "Jak **** poradziłby sobie, gdyby mógł zamieniać się miejscami z dowolną osobą przez 10 minut dziennie?",
            "Jak **** zareagowałby, gdyby dostał magiczny długopis, który spełnia życzenia?",
            "Co **** zrobiłby, gdyby każdy jego żart stawał się prawdą?",
            "Jak **** zachowałby się, gdyby musiał nosić kostium banana przez cały dzień?",
            "Jak **** zachowałby się, gdyby jego dzień powtarzał się w kółko jak w „Dniu świstaka”?",
            "Co **** zrobiłby, gdyby musiał wystąpić w reality show „Przetrwaj tydzień bez internetu”?",
            "Jak **** poradziłby sobie, gdyby został uwięziony w muzeum w nocy?",
            "Co **** zrobiłby, gdyby mógł mieć własną wyspę?",
            "Jak **** poradziłby sobie, gdyby trafił do średniowiecza z telefonem i powerbankiem?",
            "Co **** zrobiłby, gdyby jego sny zaczęły się spełniać w realu?",
            "Jak **** zachowałby się, gdyby mógł wymyślić nowe prawo dla całego świata?",
            "Co **** zrobiłby, gdyby nagle musiał wystąpić na Eurowizji?",
            "Jak **** reagowałby, gdyby mógł mówić tylko cytatami z filmów?",
            "Co **** zrobiłby, gdyby dostał list od swojego przyszłego „ja”?",
            "Co **** zrobiłby, gdyby dostał magiczny pilot do zatrzymywania czasu?",
            "Jak **** uratowałby świat, gdyby miał supermoc, której nikt nie rozumie?",
            "Co **** zrobiłby, gdyby mógł rozmawiać z roślinami?",
            "Jak **** zachowałby się, gdyby przypadkiem został celebrytą?",
            "Co **** zrobiłby, gdyby mógł teleportować się tylko w miejsca, w których już kiedyś zasnął?",
            "Co **** zrobiłby, gdyby mógł zamieniać ludzi w dowolne zwierzęta?",
            "Jak **** zareagowałby, gdyby został królem memów w internecie?",
            "Jak **** zachowałby się, gdyby dostał list z Hogwartu… ale jako nauczyciel?",
            "Co **** zrobiłby, gdyby mógł cofnąć się do jednego dnia z dzieciństwa?",
            "Co **** zrobiłby, gdyby musiał mieszkać w lodówce przez tydzień?",
            "Jak **** poradziłby sobie, gdyby został porwany przez kosmitów, którzy lubią disco polo?",
            "Co **** zrobiłby, gdyby jego najlepszy przyjaciel okazał się robotem?",
            "Jak **** poradziłby sobie, gdyby został wciągnięty do gry komputerowej?",
            "Jak **** poradziłby sobie, gdyby został zamknięty w centrum handlowym na noc?",
            "Co **** zrobiłby, gdyby nagle potrafił czytać myśli innych?",
            "Jak **** reaguje, gdy ktoś mu powie „musimy pogadać”?",
            "Co **** zrobiłby, gdyby musiał iść na randkę z kimś z tej grupy?",
            "Co **** mówi, gdy naprawdę nie ma siły na ludzi?",
            "Jak **** zachowuje się, gdy dostaje niespodziewany komplement?",
            "Co **** zrobiłby, gdyby jego rodzice przeczytali jego czaty?",
            "Jak **** reaguje, gdy ktoś daje mu zły prezent?",
            "Co **** mówi, gdy próbuje być grzeczny, ale już nie może?",
            "Jak **** reaguje, gdy ktoś wyciąga stare kompromitujące zdjęcia?",
            "Co **** zrobiłby, gdyby miał odegrać scenę romantyczną na scenie?",
            "Co **** mówi, gdy próbuje uniknąć niezręcznej ciszy?",
            "Co **** zrobiłby, gdyby musiał spędzić dzień z osobą, której nie lubi?",
            "Co **** mówi, gdy próbuje wyjść z kłopotliwej sytuacji?",
            "Jak **** reaguje, gdy ktoś opowiada żart, który nie jest śmieszny?",
            "Co **** zrobiłby, gdyby jego przyjaciel wyznał mu, że jest w nim zakochany?",
            "Jak **** reaguje, gdy ktoś mu gratuluje za coś, czego nie zrobił?",
            "Co **** mówi, gdy naprawdę nie wie, o co chodzi w rozmowie?",
            "Co **** zrobiłby, gdyby ktoś poprosił go o taniec w środku ulicy?",
            "Co **** powiedziałby, gdyby musiał napisać swoje bio na Tinderze?",
            "Co **** zrobiłby, gdyby przypadkiem podsłuchał rozmowę o sobie?",
            "Jak **** reaguje, gdy ktoś mu powie: „Nie jesteś taki, jak myślałem”?",
            "Co **** zrobiłby, gdyby ktoś napisał o nim piosenkę miłosną?",
            "Co **** zrobiłby, gdyby musiał randkować z kimś zupełnie swoim przeciwieństwem?",
            "Co **** zrobiłby, gdyby dostał 5 minut na przekonanie kogoś, że jest niesamowity?",
            "Jak wyglądałaby idealna randka wymyślona przez ****?",
            "Co **** zrobiłby, gdyby przypadkiem wysłał flirtującego SMS-a do złej osoby?",
            "Jak wyglądałby świat, gdyby wszyscy zachowywali się jak ****?",
            "Co **** powiedziałby, gdyby spotkał samego siebie z innego wymiaru?",
            "Jak wyglądałby idealny dzień wakacji według ****?",
            "Jakie jedzenie mogłoby zostać symbolem ****?",
            "Co **** powiedziałby, gdyby dostał list od samego siebie z przyszłości?",
            "Jak wyglądałby plakat filmu o życiu ****?",
            "Jak wyglądałby świat, gdyby wszyscy mieli poczucie humoru jak ****?",
            "Co **** powiedziałby, gdyby został prezenterem prognozy pogody?",
            "Co **** zrobiłby, gdyby każdy jego ruch miał dźwięk jak w kreskówkach?",
            "Jakie byłoby najgorsze motto firmy założonej przez ****?",
            "Jak wyglądałby poranek **** w alternatywnym wszechświecie?",
            "Co **** zrobiłby, gdyby dostał list z Hogwartu?",
            "Co **** powiedziałby, gdyby musiał prowadzić wesele?",
            "Jakie byłoby hasło kampanii reklamowej stworzonej przez ****?",
            "Jakie danie byłoby nazwane „Specjałem ****”?",
            "Jaka byłaby nazwa zespołu muzycznego założonego przez ****?",
            "Jakie byłoby motto religii założonej przez ****?",
            "Co **** powiedziałby, gdyby został spytany o sens życia w telewizji na żywo?",
            "Co **** zrobiłby, gdyby wygrał dożywotni zapas chipsów?",
            "Jak wyglądałaby książka kucharska napisana przez ****?",
            "Co **** zrobiłby, gdyby obudził się w ciele sławnej osoby?",
            "Jak wyglądałby film o przyjaźni między **** a lodówką?",
            "Co **** powiedziałby, gdyby mógł wysłać SMS-a do całego świata?",
            "Co **** powiedziałby kosmicznemu przybyszowi na powitanie?",
            "Co **** zrobiłby, gdyby mógł kontrolować pogodę?",
            "Jak wyglądałby vlog podróżniczy ****?",
            "Co **** zrobiłby, gdyby znalazł magiczną lampę z dżinem?",
            "Jak wyglądałby dzień **** w średniowiecznym zamku?",
            "Co **** powiedziałby, gdyby był prowadzącym „Familiadę”?",
            "Jak wyglądałby sklep prowadzony przez ****?",
            "Jakie byłoby najgorsze możliwe hobby dla ****?",
            "Jakie byłoby ulubione zaklęcie **** w świecie Harry’ego Pottera?",
            "Jakie byłoby najgorsze hasło motywacyjne wymyślone przez ****?",
            "Co **** zrobiłby, gdyby mógł przez jeden dzień łamać wszystkie zasady?",
            "Jak wyglądałby film dokumentalny o życiu ****?",
            "Co **** zrobiłby, gdyby spotkał swoje 10-letnie „ja”?",
            "Jak wyglądałby tatuaż, który idealnie pasuje do ****?",
            "Jakie jest najbardziej szalone marzenie **** z dzieciństwa?",
            "Jak wyglądałby dzień **** w średniowieczu?",
            "Co **** zrobiłby, gdyby wygrał darmowe bilety w jedną stronę w nieznane?",
            "Jaką bajkę **** oglądałby po cichu, żeby nikt nie widział?",
            "Jak wyglądałby poranek **** po wygraniu miliona?",
            "Jaką piosenkę **** śpiewałby na karaoke?",
            "Co **** najczęściej odkłada „na później”?",
            "Co **** powiedziałby, gdyby wygrał Oscara?",
            "Co **** robi w sobotnie wieczory, gdy nikt nie ma planów?",
            "Co **** zrobiłby, gdyby wygrał loterię?",
            "Jakie byłoby najbardziej nietypowe hobby ****?",
            "Co **** zrobiłby, gdyby spotkał kosmitę?",
            "Co **** zawsze chce spróbować, ale się boi?",
            "Co **** zrobiłby, gdyby mógł zamienić się ciałem z kimś na dzień?",
            "Co **** powiedziałby swojemu przyszłemu „ja” z 2050 roku?",
            "Jak by wyglądał wymarzony dom ****?",
            "Co **** robi, żeby poprawić sobie humor?",
            "Jaką najbardziej absurdalną wymówkę wymyśliłby ****, żeby nie przyjść do pracy/szkoły?",
            "Jak wyglądałby profil randkowy ****?",
            "Na co zazwyczaj brakuje czasu ****?",
            "Co **** zrobiłby, gdyby mógł rozmawiać ze zwierzętami?",
            "Co **** zrobił na ostatniej imprezie, co wszyscy pamiętają?",
            "Jaką najbardziej absurdalną rzecz kupiłby ****, gdyby miał nieograniczony budżet?",
            "Jaką najgłupszą rzecz mógłby zrobić **** po pijaku?",
            "Co **** zrobiłby, gdyby został bohaterem filmu akcji?",
            "Co **** najczęściej robi w pracy lub szkole, zamiast pracować?",
            "Jakie jest najbardziej „****owe” powiedzonko?",
            "Co **** zrobiłby, gdyby znalazł walizkę z milionem złotych?",
            "Co **** zrobiłby, gdyby był przez jeden dzień niewidzialny?",
            "Jakim zwierzęciem byłby ****, gdyby mógł się zamienić?",
            "Co **** mógłby robić zawodowo, gdyby nie to, co robi teraz?",
            "Z kim sławnym **** chciałby zjeść kolację?",
            // ---------------------------
            "Jakie jest ulubione ciasto gracza ****?",
            "O której **** zwykle wstaje?",
            "Jaki fast food **** lubi najbardziej?",
            "O czym najczęściej **** zapomina?",
            // "Ile **** chce mieć dzieci?",
            "Czego **** wstydzi się najbardziej?",
            "Z iloma osobami **** się spotkał w życiu za pomocą aplikacji randkowych?",
            "Z jakiej dziedziny **** ma największą wiedzę?",
            "Jakiego modelu telefonu **** aktualnie używa?",
            "Jaka była pierwsza praca ****?",
            // "Gdzie **** lubi być najbardziej masowany?",
            "Jaki **** ma ulubiony smak lodów?",
            "Jaką czynność domową **** najmniej lubi?",
            "Jaką kawę **** najczęściej pije?",
            "Jakim **** jest kierowcą w skali od 1 do 5?",
            "O co najczęściej **** potrafi się obrazić?",
            "Jaką zbędną rzecz **** ostatnio kupił?",
            "Jaka jest ulubiona piosenka ****?",
            "Według ****, jakie samochody są najlepsze?",
            "Jakiego przekleństwa **** używa najczęściej?",
            "Jakiego słowa **** ostatnio nadużywa?",
            "Jaki jest ulubiony serial ****?",
            "Jaki alkohol **** najczęściej pije?",
            "Co aktualnie najbardziej stresuje ****?",
            "Bez jakiego sprzętu **** nie wyobraża sobie życia?",
            "W jaką grę **** gra najczęściej?",
            "Jak **** ocenia swoje zdolności kulnarne w skali od 1 do 5?",
            "Jaki jest największy sukces ****?",
            "Jaki jest ulubiony youtuber ****?",
            "Na co **** wydaje najwięcej pieniędzy?", // OBA
            // "Której części ciała **** najbardziej w sobie nie lubi?"
            "Na czyj koncert **** najchętniej pójdzie w najbliższym czasie?",
            "Jakie **** ma największe marzenie podróżnicze?",
            "Czego **** nigdy by nie zjadł?",
            "Czego **** chce się nauczyć?",
            "Z kim **** najczęściej rozmawia przez telefon?",
            "Co **** zazwyczaj boli?", // OBA
            "Co **** lubi robić w wolnym czasie?",
            "Jakiego gatunku muzyki **** najbardziej nie lubi?",
            "Jaki sport najbardziej emocjonuje ****?",
            "Jakie wino **** lubi najbardziej?",
            "Ile kaw **** pije w ciągu dnia?",
            "Gdzie **** zrobiłby sobie tatuaż?",
            "Co **** chciałby dostać na urodziny?",
            "Jakie jest popisowe danie ****?",
            "Jaka mała rzecz potrafi poprawić humor ****?", //OBA TODO: dodac wybieranie gry
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
