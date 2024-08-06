using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polygon.Models;

namespace Polygon.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShapesController : ControllerBase
    {
        private readonly ILogger<ShapesController> _logger;
        private readonly PolygonContext _context;

        public ShapesController(ILogger<ShapesController> logger, PolygonContext context)
        {
            _logger = logger;
            _context = context;

            if (!_context.Polygons.Any())
            {
                _context.Polygons.AddRange(
                    new Models.Shape
                    {
                        Color = 0xFF0000,
                        Points = new List<Point>
                        {
                            new Point { X = 0, Y = 0 },
                            new Point { X = 0, Y = 1 },
                            new Point { X = 1, Y = 1 },
                            new Point { X = 1, Y = 0 }
                        },
                        Area = 1,
                        LongestSide = 1.4142135623730951,
                        Perimeter = 4
                    },
                    new Models.Shape
                    {
                        Color = 0x00FF00,
                        Points = new List<Point>
                        {
                            new Point { X = 0, Y = 0 },
                            new Point { X = 0, Y = 2 },
                            new Point { X = 2, Y = 2 },
                            new Point { X = 2, Y = 0 }
                        },
                        Area = 4,
                        LongestSide = 2,
                        Perimeter = 8
                    },
                    new Models.Shape
                    {
                        Color = 0x0000FF,
                        Points = new List<Point>
                        {
                            new Point { X = 0, Y = 0 },
                            new Point { X = 0, Y = 3 },
                            new Point { X = 3, Y = 3 },
                            new Point { X = 3, Y = 0 }
                        },
                        Area = 9,
                        LongestSide = 3,
                        Perimeter = 12
                    },
                    new Models.Shape
                    {
                        Color = 0xFF00FF,
                        Points = new List<Point>
                        {
                            new Point { X = 0, Y = 0 },
                            new Point { X = 0, Y = 4 },
                            new Point { X = 4, Y = 4 },
                            new Point { X = 4, Y = 0 }
                        },
                        Area = 16,
                        LongestSide = 4,
                        Perimeter = 16
                    }
                );
                _context.SaveChanges();
            }
        }

        [HttpGet("AllShapes")]
        public async Task<ActionResult<IEnumerable<Models.Shape>>> GetAllShapes()
        {
            return await _context.Polygons.Include(s => s.Points).ToListAsync();
        }

        [HttpGet("LargestArea")]
        public async Task<ActionResult<Models.Shape>> GetLargestArea()
        {
            var shape = await _context.Polygons.Include(s => s.Points).OrderByDescending(s => s.Area).FirstOrDefaultAsync();
            if (shape == null)
            {
                return NotFound();
            }
            return Ok(shape);
        }

        [HttpGet("LargestPerimeter")]
        public async Task<ActionResult<Models.Shape>> GetLargestPerimeter()
        {
            var shape = await _context.Polygons.Include(s => s.Points).OrderByDescending(s => s.Perimeter).FirstOrDefaultAsync();
            if (shape == null)
            {
                return NotFound();
            }
            return Ok(shape);
        }

        [HttpGet("LongestSide")]
        public async Task<ActionResult<Models.Shape>> GetLongestSide()
        {
            var shape = await _context.Polygons.Include(s => s.Points).OrderByDescending(s => s.LongestSide).FirstOrDefaultAsync();
            if (shape == null)
            {
                return NotFound();
            }
            return Ok(shape);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Models.Shape>> GetShapeById(int id)
        {
            var shape = await _context.Polygons.Include(s => s.Points).FirstOrDefaultAsync(s => s.Id == id);
            if (shape == null)
            {
                return NotFound();
            }
            return Ok(shape);
        }

        [HttpPost("AddShape")]
        public async Task<ActionResult<Models.Shape>> AddShape([FromBody] List<Shape> shapes)
        {
            if (shapes == null)
            {
                return BadRequest();
            }
            
            _context.Polygons.AddRange(shapes);
            await _context.SaveChangesAsync();

            return Ok("Shapes received successfully.");
        }
    }
}
