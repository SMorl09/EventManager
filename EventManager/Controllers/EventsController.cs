using Application.DTO.Request;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EventManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IWebHostEnvironment _env;

        public EventsController(IEventService eventService, IWebHostEnvironment env)
        {
            _eventService = eventService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEvents();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id);
            if (eventModel == null)
                return NotFound();
            return Ok(eventModel);
        }
        [HttpGet("WithAddress/{id}")]
        public async Task<IActionResult> GetEventWithAddress(int id)
        {
            var eventModel = await _eventService.GetEventWithAddressByIdAsync(id);
            if (eventModel == null)
                return NotFound();
            return Ok(eventModel);
        }
        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetEventByName(string title)
        {
            var eventModel = await _eventService.GetEventByNameAsync(title);
            if (eventModel == null)
                return NotFound();
            return Ok(eventModel);
        }
        [HttpGet("{eventId}/users")]
        public async Task<IActionResult> GetUsersByEventId(int eventId)
        {
            try
            {
                var users = await _eventService.GetAllUsers(eventId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] EventRequest eventRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var eventResponseWithoutImage = await _eventService.CreateEventWithAddressAsync(eventRequest);
            return CreatedAtAction(nameof(GetEventWithAddress), new { id = eventResponseWithoutImage.Id }, eventResponseWithoutImage);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventRequest eventRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _eventService.UpdateEventAsync(id, eventRequest);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateEventImage(int id, [FromForm] ImageRequest imageRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (imageRequest.Image == null || imageRequest.Image.Length == 0)
            {
                return BadRequest("File is empty");
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageRequest.Image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("The file format is not valid. Allowed: .jpg, .jpeg, .png, .gif");
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageRequest.Image.FileName);

            var imagePath = Path.Combine(_env.WebRootPath, "images", fileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await imageRequest.Image.CopyToAsync(stream);
            }

            var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";

            try
            {
                await _eventService.UpdateEventImageAsync(id, imageUrl);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{eventId}/register/{userId}")]
        public async Task<IActionResult> RegisterUserToEvent(int eventId, int userId)
        {
            try
            {
                await _eventService.RegisterUserToEventAsync(eventId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{eventId}/unregister/{userId}")]
        public async Task<IActionResult> UnregisterUserFromEvent(int eventId, int userId)
        {
            try
            {
                await _eventService.UnregisterUserFromEventAsync(eventId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
