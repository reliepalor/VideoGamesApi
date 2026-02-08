using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VideoGameApi.Api.Dto.Games;
using VideoGameApi.Data;
using VideoGameApi.Models.Games;

namespace VideoGameApi.Api.Controllers.Games
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController(VideoGameDbContext context) : ControllerBase
    {
        private readonly VideoGameDbContext _context = context;

        //get all videogames
        [HttpGet]
        public async Task<ActionResult<List<VideoGame>>> GetVideoGames()
        { 
            return Ok(await _context.VideoGames.ToListAsync());
        }

        //get or search videogame by id
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<VideoGame>> GetVideoGameById(int id)
        {
            var game = await _context.VideoGames.FindAsync(id);
            if (game is null)
                return NotFound();

            return Ok(game);
        }

        //add new videogame
        [HttpPost]
        public async Task<IActionResult> AddVideoGame([FromForm] VideoGameCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = new VideoGame
            {
                Title = dto.Title,
                Platform = dto.Platform,
                Developer = dto.Developer,
                Publisher = dto.Publisher,
                Price = dto.Price,
            };

            if (dto.Image != null)
            {
                var uploadsPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "videogames"
                );

                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                game.ImagePath = $"/uploads/videogames/{fileName}";
            }

            _context.VideoGames.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVideoGameById), new { id = game.Id }, game);
        }

        //update/edit a videogame
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVideoGame(int id,[FromForm] VideoGameUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = await _context.VideoGames.FindAsync(id);
            if (game == null) return NotFound();

            game.Title = dto.Title;
            game.Platform = dto.Platform;
            game.Developer = dto.Developer;
            game.Publisher = dto.Publisher;
            game.Price = dto.Price;

            if (dto.Image != null)
            {
                var uploadsPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "videogames"
                );

                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Image.CopyToAsync(stream);

                game.ImagePath = $"/uploads/videogames/{fileName}";
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideoGame(int id)
        {
            var game = await _context.VideoGames.FindAsync(id);
            if (game is null)
                return NotFound();

            _context.VideoGames.Remove(game);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
