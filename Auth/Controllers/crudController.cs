using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Auth.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/crud")]
    public class CrudController : ControllerBase
    {
        private static List<Item> _items = new List<Item>();

        // GET method - [Authorize] will restrict this endpoint to authenticated users
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Get()
        {
            return Ok(_items); // Returns the list of items
        }

        // POST method - [Authorize] will restrict this endpoint to authenticated users
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post([FromBody] Item item)
        {
            if (item == null)
            {
                return BadRequest("Item cannot be null.");
            }

            item.Id = _items.Count + 1;
            _items.Add(item);
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item); // Returns the created item
        }

        // PUT method - [Authorize] will restrict this endpoint to authenticated users
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] Item updatedItem)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound("Item not found.");
            }

            item.Name = updatedItem.Name;
            var returnModel = new Item
            {
                Id = id,
                Name = item.Name
            };
            return Ok(returnModel); 
        }

        // DELETE method - [Authorize] will restrict this endpoint to authenticated users
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound("Item not found.");
            }

            _items.Remove(item);

            return Ok($"the id {id} is removed"); // Return 200 Ok Content, indicating successful deletion
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
