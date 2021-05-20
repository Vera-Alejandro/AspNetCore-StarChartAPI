using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        [ActionName("GetById")]
        public IActionResult GetById(int Id)
        {
            CelestialObject spaceObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == Id);

            if (spaceObject == null)
            {
                return NotFound();
            }

            spaceObject.Name = "GetById";
            spaceObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == spaceObject.Id).ToList();

            return Ok(spaceObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string Name)
        {
            var spaceThings = _context.CelestialObjects.Where(c => c.Name == Name);

            if (!spaceThings.Any())
            {
                return NotFound();
            }

            foreach (var floatyObj in spaceThings)
            {
                floatyObj.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == floatyObj.Id).ToList();
            }

            return Ok(spaceThings);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allSpaceThings = _context.CelestialObjects.ToList();

            foreach (CelestialObject spaceObj in allSpaceThings)
            {
                spaceObj.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == spaceObj.Id).ToList();
            }

            return Ok(allSpaceThings);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject spaceObject)
        {
            _context.CelestialObjects.Add(spaceObject);

            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = spaceObject.Id }, spaceObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject spaceObject)
        {
            CelestialObject foundSpaceThing = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (foundSpaceThing == null)
            {
                return NotFound();
            }

            foundSpaceThing.Name = spaceObject.Name;
            foundSpaceThing.OrbitalPeriod = spaceObject.OrbitalPeriod;
            foundSpaceThing.OrbitedObjectId = spaceObject.OrbitedObjectId;

            _context.CelestialObjects.Update(foundSpaceThing);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            CelestialObject spaceThing = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (spaceThing == null)
            {
                return NotFound();
            }

            spaceThing.Name = name;

            _context.Update(spaceThing);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            CelestialObject spaceThing = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (spaceThing == null)
            {
                return NotFound();
            }

            _context.RemoveRange(spaceThing);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
