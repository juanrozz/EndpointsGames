using EndpointsPage.DTOs.Request;
using EndpointsPage.DTOs.Response;
using EndpointsPage.Entities;
using EndpointsPage.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace GamePage.Controllers
{
    [Authorize]
    [Route("Api")]
    [ApiController]
    public class GamePlaysController : ControllerBase
    {
        private readonly InternsDbContext _dbContext;

        public GamePlaysController(InternsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[Authorize]

        [HttpPost("StartMatch")]
        public async Task<ActionResult<GeneralResponseDto<StartMatchResponseDto>>> StartMatch(StartMatchRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(GeneralResponseDto<StartMatchResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "The token is invalid, or there is no content to identify the user"
                    ));

                //return Unauthorized("The token is invalid, or there is no content to identify the user");
            }

            int tokenUserId = int.Parse(userIdClaim);
            bool isActiveLevel = await _dbContext.LevelsJdrs.AnyAsync(l => l.Status);
            if (!isActiveLevel)
            {
                return BadRequest(GeneralResponseDto<StartMatchResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "The level is inactive"
                    ));
            }

            bool levelExists = await _dbContext.LevelsJdrs.AnyAsync(l => l.LevelId == request.Difficult);
            if (!levelExists)
            {
                return BadRequest(GeneralResponseDto<StartMatchResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "The level don´t exists"
                    ));
            }

            bool isActiveGame = await _dbContext.GamesJdrs.AnyAsync(a => a.Status);
            if (!isActiveGame)
            {
                return BadRequest(GeneralResponseDto<StartMatchResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "The game is inactive"
                     ));
                //return BadRequest("The game is inactive");
            }
            bool userExists = await _dbContext.UsersJdrs.AnyAsync(u => u.UserId == tokenUserId);
            bool gameExists = await _dbContext.GamesJdrs.AnyAsync(g => g.GameId == request.GameId);

            if (!userExists)
            {
                return NotFound(GeneralResponseDto<StartMatchResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "user not found"
                    ));
                //return NotFound("User not found");
            }
            if (!gameExists)
            {
                return NotFound(GeneralResponseDto<StartMatchResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "Game not found"
                    ));
                //return NotFound("game not found");
            }
            //if (!userExists || !gameExists) return NotFound("User or Game not found.");

            MatchesJdr? match = request.DtoToMatchesJdr(tokenUserId);

            await _dbContext.MatchesJdrs.AddAsync(match);
            await _dbContext.SaveChangesAsync();
            //StartMatchResponseDto response = new StartMatchResponseDto
            //{
            //    MatchId = match.MatchId
            //};

            return Ok(GeneralResponseDto<StartMatchResponseDto>.SuccesResult(
                   match.StartMatchesJdrToDto(),
                    "game started"
                   ));
        }

        [HttpPost("FinishMatch")]
        public async Task<ActionResult<GeneralResponseDto<FinishMatchResponseDto>>> FinishMatch(FinishMatchRequestDto request)
        {
            MatchesJdr? finishMatch = await _dbContext.MatchesJdrs.FirstOrDefaultAsync(m => m.MatchId == request.MatchId);

            if (finishMatch == null)
            {
                return NotFound(GeneralResponseDto<FinishMatchResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "Match not found."
                    ));
                //return NotFound("Match not found.");
            }
            finishMatch.Score = request.FinalScore;
            finishMatch.TimeSeconds = request.FinalTime;


            await _dbContext.SaveChangesAsync();
            return Ok(GeneralResponseDto<FinishMatchResponseDto>.SuccesResult(
                finishMatch.FinishMatchesJdrToDto(),
                "Game finish"
                ));
            //return Ok(finishMatch.FinishMatchesJdrToDto());
        }



        [HttpGet("PersonalStatics/{idGame:int}")]
        public async Task<ActionResult<GeneralResponseDto<UserStatisticsResponseDto>>> GetIndividualStatistics([FromRoute] int idGame)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(GeneralResponseDto<UserStatisticsResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                     "The token is invalid, or there is no content to identify the user"
                    ));
                //return Unauthorized("The token is invalid, or there is no content to identify the user");
            }

            int tokenUserId = int.Parse(userIdClaim);

            if (idGame <= 0)
            {
                return BadRequest(GeneralResponseDto<UserStatisticsResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                     "The IdentifierUser or Game cannot be less than or equal to 0"
                    ));
                //return BadRequest("The IdentifierUser or Game cannot be less than or equal to 0");
            }
            bool gameExist = await _dbContext.GamesJdrs.AnyAsync(m => m.GameId == idGame);

            if (!gameExist)
            {
                return BadRequest(GeneralResponseDto<UserStatisticsResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "The game not found"
                    ));
                //return BadRequest("The game not found");
            }

            int totalScore = await _dbContext.MatchesJdrs
                .Where(m => m.GameId == idGame)
                .Where(m => m.UserId == tokenUserId)
                .SumAsync(m => m.Score);

            var bestMatch = await _dbContext.MatchesJdrs
                .Include(b => b.Level)
                .Include(b => b.User)
                .Where(m => m.GameId == idGame)
                .Where(m => m.UserId == tokenUserId)
                .OrderByDescending(m => m.Score)
                .FirstOrDefaultAsync();

            if (bestMatch == null)
            {
                return NotFound(GeneralResponseDto<UserStatisticsResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "The user has no registered matches."
                    ));
                //return NotFound("The user has no registered matches.");
            }

            return Ok(GeneralResponseDto<UserStatisticsResponseDto>.SuccesResult(
                bestMatch.MatchesJdrsToDto(totalScore),
                "Here are your statistics"
                ));
            //return Ok(bestMatch.MatchesJdrsToDto(totalScore));
        }

        [HttpGet("Stats/{idGame:int}")]
        public async Task<ActionResult<GeneralResponseDto<List<StatsBestResponseDto>>>> GetStats(int idGame)
        {
            bool gameExist = await _dbContext.GamesJdrs.AnyAsync(m => m.GameId == idGame);

            if (!gameExist)
            {
                return BadRequest(GeneralResponseDto<UserStatisticsResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "The game not found"
                    ));
                //return BadRequest("The game not found");
            }

            List<MatchesJdr>? topScores = await _dbContext.MatchesJdrs
            .Include(b => b.Level)
            .Include(b => b.User)
            .Where(a => a.GameId == idGame)
            .OrderByDescending(p => p.Score)
            .ToListAsync();

            if (topScores.Count > 0)
            {
                return Ok(GeneralResponseDto<List<StatsBestResponseDto>>.SuccesResult(
                    topScores.Select(g => g.GlobalStatisticsToDto()).ToList(),
                    "Statistics generated"
                    ));
                //return Ok(topScores.Select(g => g.GlobalStatisticsToDto()));

            }
            else
            {
                return NotFound(GeneralResponseDto<StatsBestResponseDto>.FailureResult(
                    "sorry, there was a system error.",
                    "No statistics were found"
                    ));
                //return NotFound("No statistics were found");
            }
        }


        /* ignorar los comentarios
          [HttpPost("MatchScore")]

        public ActionResult UpdateMatchScore([FromQuery] int matchId, [FromQuery] int newScore)
        {
            var idPartida = _dbContext.MatchesJdrs.FirstOrDefault(m => m.MatchId == matchId);
            if (idPartida == null)
            {
                return NotFound($"el IdentifierUser {idPartida} no existe mano ");
            }


            idPartida.Score = newScore;

            _dbContext.SaveChanges();

            return Ok($"se actualizo el puntaje de la partida {matchId} con este puntaje {newScore}");
        }

        [HttpGet("GetCar/{IdentifierUser}")]

       public ActionResult GetCar(int IdentifierUser) { 

           var car = _dbContext.Cars.FirstOrDefault(c => c.IdentifierUser == IdentifierUser);
           if (car == null) return NotFound("Car not found");

           var response = new CarResponseDto
           {
               IdentifierUser = car.IdentifierUser,
               Brand = car.Brand,
               Model = car.Model,
               LicensePlate = car.LicensePlate,
               Year = car.Year
           };

           return Ok(response);


       }


      public class CarResponseDto
{
   public int IdentifierUser { get; set; }
   public string Brand { get; set; } // Marca
   public string Model { get; set; } // Modelo
   public string LicensePlate { get; set; } // Matrícula/Placa
   public int Year { get; set; }
}

         [HttpGet("GetId/{IdentifierUser}")]
       public ActionResult GetId(int IdentifierUser)
       {
           var level = _dbContext.LevelsJdrs.FirstOrDefault(n => n.LevelId == IdentifierUser);

           if (level == null)
           {
               return NotFound("IdentifierUser not found");
           }

           return Ok(level);
       }

       [HttpPost("ChangeLevelName")]
       public ActionResult ChangeLevelName( int IdentifierUser, string newName)
       {
           var level = _dbContext.LevelsJdrs.FirstOrDefault(n => n.LevelId == IdentifierUser);

           if (level == null)
           {
               return NotFound("IdentifierUser not found");
           }

           level.LevelName = newName;
           _dbContext.SaveChanges();

           return Ok($"The name of level IdentifierUser {IdentifierUser} was updated to {newName}");
       }

       [HttpPost("UpdateDifficulty")]
       public ActionResult UpdateDifficulty(int IdentifierUser, float newDifficulty)
       {
           var level = _dbContext.LevelsJdrs.FirstOrDefault(n => n.LevelId == IdentifierUser);

           if (level == null)
           {
               return NotFound("IdentifierUser not found");
           }

           level.Difficult = newDifficulty;
           _dbContext.SaveChanges();

           return Ok($"The difficulty of level IdentifierUser {IdentifierUser} was changed to {newDifficulty}");
       }

       [HttpPost("RegisterLevel")]
       public ActionResult RegisterLevel(int IdentifierUser, string newName, float difficulty)
       {
           var newLevel = new LevelsJdr
           {
               LevelId = IdentifierUser,
               LevelName = newName,
               Difficult = difficulty
           };

           _dbContext.LevelsJdrs.Add(newLevel);
           _dbContext.SaveChanges();

           return Ok("Level registered successfully.");
       }

       [HttpGet("GetMatchesByUser/{userId}")]
       public ActionResult GetMatchesByUser(int userId)
       {
           var matchLines = _dbContext.MatchesJdrs.Where(p => p.UserId == userId).ToList();

           return Ok(matchLines);
       }

       [HttpGet("GetUsersByLevel/{level}")]
       public ActionResult GetUsersByLevel(int level)
       {
           var userList = _dbContext.UsersJdrs.Where(n => n.CurrentLevel == level).ToList();

           return Ok(userList);
       }

       [HttpGet("CountMatches/{userId}")]
       public ActionResult CountMatches(int userId)
       {
           int totalMatches = _dbContext.MatchesJdrs.Where(m => m.UserId == userId).Count();

           return Ok(new { User = userId, TotalPlayed = totalMatches });
       }

       [HttpGet("GetTotalScore/{userId}")]
       public ActionResult GetTotalScore(int userId)
       {
           var totalScore = _dbContext.MatchesJdrs.Where(t => t.UserId == userId).Sum(s => s.Score);

           return Ok(new { User = userId, Score = totalScore });
       }

     [HttpGet("buscar")]
public ActionResult<IEnumerable<Camiseta>> BuscarPorTalla([FromQuery] string talla)
{
    // 1. Validar si el parámetro 'talla' no viene vacío o nulo
    if (!string.IsNullOrEmpty(talla))
    {
        // 2. Filtra la lista '_camisetas' usando .Where() para obtener las que coincidan con la talla.
        // Pista: Recuerda usar .ToList() al final para que coincida con el tipo de retorno.
        var resultado = _camisetas.Where(c => c.Talla == talla).ToList();
        
        return Ok(resultado);
    }

    // 3. Si no se envió el filtro, retorna la lista completa con estado OK.
    return Ok(_camisetas)
}

       [HttpGet("GetTop3Scores")]
       public ActionResult GetTop3Scores()
       {
           var topScores = _dbContext.MatchesJdrs
               .OrderByDescending(m => m.Score)
               .Take(3)
               .ToList();

           return Ok(topScores);
       }*/
    }
}