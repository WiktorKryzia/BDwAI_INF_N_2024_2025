using ChessManager.Areas.Identity.Data;
using ChessManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChessManager.Controllers
{
    public class TournamentController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TournamentController(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        // GET: Tournament
        public async Task<IActionResult> Index()
        {
            var model = await _context.Tournaments.Include(t => t.Arbiter).Include(t => t.Players).Include(t => t.Rounds).ToListAsync();
            return View(model);
        }

        [AllowAnonymous]
        // GET: Tournament/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.Arbiter)
                .Include(t => t.Players.OrderByDescending(p => p.Rating))
                .ThenInclude(t => t.User)
                .Include(t => t.Rounds)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        [Route("Tournament/{tournamentId}/Round/{roundNumber}")]
        public async Task<IActionResult> Round(int tournamentId, int roundNumber)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.Matches)
                        .ThenInclude(m => m.WhitePlayer)
                            .ThenInclude(p => p.User)
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.Matches)
                        .ThenInclude(m => m.BlackPlayer)
                            .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return NotFound();
            }

            var round = tournament.Rounds.FirstOrDefault(r => r.RoundNumber == roundNumber);

            if (round == null)
            {
                return NotFound();
            }

            ViewData["CurrentRound"] = tournament.Rounds.Count;

            return View(round);
        }

        [HttpPost]
        public async Task<IActionResult> SaveResults(Dictionary<int, string> matchResults)
        {
            if (matchResults == null || !matchResults.Any())
            {
                return BadRequest("No results provided.");
            }

            foreach (var matchResult in matchResults)
            {
                var match = await _context.Matches
                    .Include(m => m.WhitePlayer)
                    .Include(m => m.BlackPlayer)
                    .FirstOrDefaultAsync(m => m.Id == matchResult.Key);

                if (match != null)
                {
                    match.Result = matchResult.Value;
                }
            }

            await _context.SaveChangesAsync();

            var referer = Request.Headers["Referer"].ToString();
            return Redirect(referer);
        }

        [Route("Tournament/{tournamentId}/Result/{roundNumber}")]
        public async Task<IActionResult> Result(int tournamentId, int roundNumber)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Players)
                    .ThenInclude(p => p.User)
                .Include(t => t.Players)
                    .ThenInclude(p => p.Matches)
                    .ThenInclude(m => m.Round)
                .Include(t => t.Rounds)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return NotFound();
            }

            var playerResults = tournament.Players
                .Select(p =>
                {
                    Console.WriteLine(p.Matches.Count);
                    return new RoundResults
                    {
                        FirstName = p.User.FirstName,
                        LastName = p.User.LastName,
                        Rating = p.Rating,
                        TotalPoints = p.Matches.Where(m => m.Round.RoundNumber <= roundNumber).Sum(m => CalculatePoints(m, p.Id))
                    };
                })
                .OrderByDescending(pr => pr.TotalPoints)
                .ThenByDescending(pr => pr.Rating)
                .ToList();

            var model = new RoundResultsViewData { TournamentId = tournamentId, RoundNumber = roundNumber, CurrentRound = tournament.Rounds.Count, Results = playerResults };

            return View(model);
        }

        private double CalculatePoints(Match match, int playerId)
        {
            if (match.Result == "1-0" && match.WhitePlayerId == playerId)
                return 1;
            if (match.Result == "0-1" && match.BlackPlayerId == playerId)
                return 1;
            if (match.Result == "0.5-0.5")
                return 0.5;

            return 0;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNextRound(int tournamentId)
        {
            var players = await _context.Players
                .Where(p => p.TournamentId == tournamentId)
                .ToListAsync();

            if (players.Count == 0)
            {
                return NotFound();
            }

            var lastRound = await _context.Rounds
                .Where(r => r.TournamentId == tournamentId)
                .OrderByDescending(r => r.RoundNumber)
                .FirstOrDefaultAsync();

            var nextRoundNumber = lastRound == null ? 1 : lastRound.RoundNumber + 1;

            var round = new Round
            {
                TournamentId = tournamentId,
                RoundNumber = nextRoundNumber,
            };

            _context.Rounds.Add(round);
            await _context.SaveChangesAsync();

            var matches = new List<Match>();

            if (players.Count % 2 == 1)
            {
                var byePlayer = players.Last();
                players.Remove(byePlayer);

                // TODO?
            }

            var random = new Random();
            var shuffledPlayers = players.OrderBy(x => random.Next()).ToList();

            for (int i = 0, j = 1; i < shuffledPlayers.Count; i += 2, j++)
            {
                matches.Add(new Match
                {
                    RoundId = round.Id,
                    BoardNumber = j,
                    WhitePlayerId = shuffledPlayers[i].Id,
                    BlackPlayerId = shuffledPlayers[i + 1].Id
                });
            }

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();

            return RedirectToAction("Round", new { tournamentId = tournamentId, roundNumber = nextRoundNumber });
        }


        // GET: Tournament/SignUp/5
        public async Task<IActionResult> SignUp(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }

            var player = new Player();
            player.Rating = 1000;
            player.TournamentId = tournament.Id;
            player.Tournament = tournament;
            player.User = await _userManager.GetUserAsync(User);
            player.UserId = player.User.Id;

            return View(player);
        }

        // POST: Tournament/SignUp
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Rating,UserId,TournamentId,User,Tournament")] Player player)
        {
            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var tournament = await _context.Tournaments.FindAsync(player.TournamentId);
            if (tournament == null)
            {
                return NotFound();
            }

            player.Tournament = tournament;
            player.User = await _userManager.GetUserAsync(User);
            player.UserId = player.User.Id;

            return View(player);
        }

        // GET: Tournament/SignOut/5
        public async Task<IActionResult> SignOut(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players.Include(t => t.Tournament).Include(t => t.User).FirstOrDefaultAsync(p => p.TournamentId == id && p.UserId == _userManager.GetUserId(User));
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Tournament/SignOut/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut(int id)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.TournamentId == id && p.UserId == _userManager.GetUserId(User));
            if (player != null)
            {
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Tournament", new { id = player.TournamentId });
        }

        // GET: Tournament/Create
        public IActionResult Create()
        {
            SetArbitersList();

            return View();
        }

        // POST: Tournament/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,TotalRounds,MaxPlayers,StartDate,EndDate,CreationDate,ArbiterId")] Tournament tournament)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            SetArbitersList();

            return View(tournament);
        }

        // GET: Tournament/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }

            SetArbitersList();

            return View(tournament);
        }

        // POST: Tournament/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,TotalRounds,MaxPlayers,StartDate,EndDate,CreationDate,ArbiterId")] Tournament tournament)
        {
            if (id != tournament.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentExists(tournament.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            SetArbitersList();

            return View(tournament);
        }

        // GET: Tournament/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournament = await _context.Tournaments
                .Include(t => t.Arbiter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View(tournament);
        }

        // POST: Tournament/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament != null)
            {
                _context.Tournaments.Remove(tournament);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.Id == id);
        }

        private void SetArbitersList()
        {
            var currentUserId = _userManager.GetUserId(User);

            var isAdmin = _context.UserRoles
                .Join(_context.Roles,
                      userRole => userRole.RoleId,
                      role => role.Id,
                      (userRole, role) => new { userRole.UserId, role.Name })
                .Any(ur => ur.UserId == currentUserId && ur.Name == "Admin");

            if (isAdmin)
            {
                var arbiters = _context.Users
                .Where(u => !_context.UserRoles
                .Join(_context.Roles,
                      userRole => userRole.RoleId,
                      role => role.Id,
                      (userRole, role) => new { userRole.UserId, role.Name })
                .Any(ur => ur.UserId == u.Id && ur.Name == "Player"))
                .Select(a => new
                {
                    ArbiterId = a.Id,
                    FullName = $"{a.FirstName} {a.LastName}"
                })
                .ToList();
                ViewData["ArbiterId"] = new SelectList(arbiters, "ArbiterId", "FullName");
            }
            else
            {
                ViewBag.ArbiterId = currentUserId;
            }
        }
    }
}
