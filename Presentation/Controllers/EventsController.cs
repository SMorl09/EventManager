using Application.DTO.Request;
using Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Filters;
using System.Linq;

namespace EventManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EventExceptionFilter]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents(CancellationToken cancellationToken)
        {
            var events = await _eventService.GetAllEvents(cancellationToken);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id, CancellationToken cancellationToken)
        {
            var eventModel = await _eventService.GetEventByIdAsync(id, cancellationToken);
            return Ok(eventModel);
        }
        [HttpGet("WithAddress/{id}")]
        public async Task<IActionResult> GetEventWithAddress(int id, CancellationToken cancellationToken)
        {
            var eventModel = await _eventService.GetEventWithAddressByIdAsync(id, cancellationToken);
            return Ok(eventModel);
        }
        [HttpGet("title/{title}")]
        public async Task<IActionResult> GetEventByName(string title, CancellationToken cancellationToken)
        {
            var eventModel = await _eventService.GetEventByNameAsync(title, cancellationToken);
            return Ok(eventModel);
        }
        [HttpGet("{eventId}/users")]
        public async Task<IActionResult> GetUsersByEventId(int eventId, CancellationToken cancellationToken)
        {
            var users = await _eventService.GetAllUsers(eventId, cancellationToken);
            return Ok(users);
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] EventRequest eventRequest, CancellationToken cancellationToken)
        {
            var eventResponseWithoutImage = await _eventService.CreateEventWithAddressAsync(eventRequest, cancellationToken);
            return CreatedAtAction(nameof(GetEventWithAddress), new { id = eventResponseWithoutImage.Id }, eventResponseWithoutImage);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventRequest eventRequest, CancellationToken cancellationToken)
        {
            await _eventService.UpdateEventAsync(id, eventRequest, cancellationToken);
            return NoContent();
        }
        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateEventImage(int id, [FromForm] ImageRequest imageRequest, CancellationToken cancellationToken)
        {
            var imageUrl = await _eventService.SaveImageAsync(imageRequest.Image, Request, cancellationToken);

           
                await _eventService.UpdateEventImageAsync(id, imageUrl, cancellationToken);
                return NoContent();
           
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id, CancellationToken cancellationToken)
        {
            await _eventService.DeleteEventAsync(id, cancellationToken);
            return NoContent();
            
        }
        [HttpPut("{eventId}/register/{userId}")]
        public async Task<IActionResult> RegisterUserToEvent(int eventId, int userId, CancellationToken cancellationToken)
        {
            await _eventService.RegisterUserToEventAsync(eventId, userId, cancellationToken);
            return Ok();
        }
        [HttpPut("{eventId}/unregister/{userId}")]
        public async Task<IActionResult> UnregisterUserFromEvent(int eventId, int userId, CancellationToken cancellationToken)
        {
            await _eventService.UnregisterUserFromEventAsync(eventId, userId, cancellationToken);
            return Ok();
        }

    }
}
