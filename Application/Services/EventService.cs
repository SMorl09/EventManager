using Application.DTO.Request;
using Application.DTO.Response;
using Application.Interface;
using Domain.Data;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{

    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IWebHostEnvironment _env;
        
        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

        public EventService(IEventRepository eventReposirory, IWebHostEnvironment env)
        {
            _eventRepository = eventReposirory;
            _env = env;
        }

        public async Task<IEnumerable<EventResponse>> GetAllEvents(CancellationToken cancellationToken)
        {
            var events = await _eventRepository.GetAllEvents(cancellationToken);
            List<EventResponse> result = new List<EventResponse>();
            foreach (var eventModel in events)
            {
                var eventResponse = new EventResponse()
                {
                    Id = eventModel.Id,
                    Title = eventModel.Title,
                    Description = eventModel.Description,
                    StartDate = eventModel.StartDate,
                    Category = eventModel.Category.ToString(),
                    MaxNumberOfUsers = eventModel.MaxNumberOfUsers,
                    ImageUrl = eventModel.ImageUrl
                };
                result.Add(eventResponse);
            }
            return result;
        }
        public async Task<IEnumerable<UserResponse>> GetAllUsers(int id, CancellationToken cancellationToken)
        {
            var eventModel = await _eventRepository.GetAllUsers(id, cancellationToken);
            List<UserResponse> result = new List<UserResponse>();
            result = eventModel.Users?.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Surename = u.Surename,
                BirthDate = u.BirthDate,
                CreatedDate = u.CreatedDate,
                Email = u.Email
            }).ToList() ?? new List<UserResponse>();
            return result;
        }
        public async Task<EventResponse> GetEventByIdAsync(int id, CancellationToken cancellationToken)
        {
            var eventModel = await _eventRepository.GetEventById(id, cancellationToken);
            if (eventModel == null)
                throw new Exception("Event not found.");

            var eventDto = new EventResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                Category = eventModel.Category.ToString(),
                MaxNumberOfUsers = eventModel.MaxNumberOfUsers,
                ImageUrl = eventModel.ImageUrl
            };

            return eventDto;
        }
        public async Task<EventWithAddressResponse> GetEventWithAddressByIdAsync(int id, CancellationToken cancellationToken)
        {
            var eventModel = await _eventRepository.GetEventWithAddress(id, cancellationToken);
            if (eventModel == null)
                throw new Exception("Event not found.");
            AddressResponse addressResponse = null;
            if (eventModel.Address != null)
            {
                addressResponse = new AddressResponse
                {
                    Id = eventModel.Address.Id,
                    State = eventModel.Address.State,
                    City = eventModel.Address.City,
                    Street = eventModel.Address.Street
                };
            }

            var eventDto = new EventWithAddressResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                Category = eventModel.Category.ToString(),
                MaxNumberOfUsers = eventModel.MaxNumberOfUsers,
                ImageUrl = eventModel.ImageUrl,
                Address = addressResponse
            };

            return eventDto;
        }
        public async Task<EventResponse> GetEventByNameAsync(string name, CancellationToken cancellationToken)
        {
            var eventModel = await _eventRepository.GetEventByName(name, cancellationToken);
            if (eventModel == null)
                throw new Exception("Event not found.");

            var eventDto = new EventResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                Category = eventModel.Category.ToString(),
                MaxNumberOfUsers = eventModel.MaxNumberOfUsers,
                ImageUrl = eventModel.ImageUrl
            };

            return eventDto;
        }
        public async Task<EventWithAddressResponse> CreateEventAsync(EventRequest eventRequest,string imageUrl, CancellationToken cancellationToken)
        {
            var eventModel = new Event
            {
                Title = eventRequest.Title,
                Description = eventRequest.Description,
                StartDate = eventRequest.StartDate,
                Category = Enum.Parse<EventCategory>(eventRequest.Category),
                MaxNumberOfUsers = eventRequest.MaxNumberOfUsers,
                ImageUrl =imageUrl,
                Address=new Address
                {
                    Street=eventRequest.Address.Street,
                    City=eventRequest.Address.City,
                    State=eventRequest.Address.State
                }
            };

            await _eventRepository.AddAsync(eventModel, cancellationToken);

            var eventResponse = new EventWithAddressResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                Category = eventModel.Category.ToString(),
                MaxNumberOfUsers = eventModel.MaxNumberOfUsers,
                ImageUrl = eventModel.ImageUrl,
                Address = new AddressResponse
                {
                    Id = eventModel.Address.Id,
                    State = eventModel.Address.State,
                    City = eventModel.Address.City,
                    Street = eventModel.Address.Street
                }
            };

            return eventResponse;
        }
        public async Task<EventResponse> CreateEventAsync(EventRequest eventRequest, CancellationToken cancellationToken)
        {
            var eventModel = new Event
            {
                Title = eventRequest.Title,
                Description = eventRequest.Description,
                StartDate = eventRequest.StartDate,
                Category = Enum.Parse<EventCategory>(eventRequest.Category),
                MaxNumberOfUsers = eventRequest.MaxNumberOfUsers
            };

            await _eventRepository.AddAsync(eventModel, cancellationToken);

            var eventResponse = new EventResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                Category = eventModel.Category.ToString(),
                MaxNumberOfUsers = eventModel.MaxNumberOfUsers,
                ImageUrl = eventModel.ImageUrl
            };

            return eventResponse;
        }
        public async Task<EventWithAddressResponse> CreateEventWithAddressAsync(EventRequest eventRequest, CancellationToken cancellationToken)
        {
            Event eventModel;
            if (eventRequest.Address != null)
            {
                eventModel = new Event
                {
                    Title = eventRequest.Title,
                    Description = eventRequest.Description,
                    StartDate = eventRequest.StartDate,
                    Category = Enum.Parse<EventCategory>(eventRequest.Category),
                    MaxNumberOfUsers = eventRequest.MaxNumberOfUsers,
                    Address = new Address
                    {
                        State = eventRequest.Address.State,
                        City = eventRequest.Address.City,
                        Street = eventRequest.Address.Street
                    }
                };
            }
            else
            {
                eventModel = new Event
                {
                    Title = eventRequest.Title,
                    Description = eventRequest.Description,
                    StartDate = eventRequest.StartDate,
                    Category = Enum.Parse<EventCategory>(eventRequest.Category),
                    MaxNumberOfUsers = eventRequest.MaxNumberOfUsers
                };
            }

            await _eventRepository.AddAsync(eventModel, cancellationToken);
            AddressResponse addressResponse = null;
            if (eventModel.Address != null)
            {
                addressResponse = new AddressResponse
                {
                    Id = eventModel.Address.Id,
                    State = eventModel.Address.State,
                    City = eventModel.Address.City,
                    Street = eventModel.Address.Street
                };
            }
            var eventResponse = new EventWithAddressResponse
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                Category = eventModel.Category.ToString(),
                MaxNumberOfUsers = eventModel.MaxNumberOfUsers,
                ImageUrl = eventModel.ImageUrl,
                Address = addressResponse
            };

            return eventResponse;
        }
        public async Task UpdateEventAsync(int id, EventRequest eventRequest, CancellationToken cancellationToken)
        {
            var eventModel = await _eventRepository.GetEventById(id, cancellationToken);
            if (eventModel == null)
                throw new Exception("Event not found.");

            eventModel.Title = eventRequest.Title;
            eventModel.Description = eventRequest.Description;
            eventModel.StartDate = eventRequest.StartDate;
            eventModel.Category = Enum.Parse<EventCategory>(eventRequest.Category);
            eventModel.MaxNumberOfUsers = eventRequest.MaxNumberOfUsers;

            await _eventRepository.UpdateAsync(eventModel, cancellationToken);
        }
        public async Task UpdateEventImageAsync(int id, string imageUrl, CancellationToken cancellationToken)
        {
            var eventModel = await _eventRepository.GetEventById(id, cancellationToken);
            if (eventModel == null)
                throw new Exception("Event not found.");

            eventModel.ImageUrl = imageUrl;

            await _eventRepository.UpdateAsync(eventModel, cancellationToken);
        }
        public async Task DeleteEventAsync(int id, CancellationToken cancellationToken)
        {
            await _eventRepository.DeleteAsync(id, cancellationToken);
        }
        public async Task RegisterUserToEventAsync(int eventId, int userId, CancellationToken cancellationToken)
        {

            await _eventRepository.RegisterUserToEventAsync(eventId, userId, cancellationToken);
        }

        public async Task UnregisterUserFromEventAsync(int eventId, int userId, CancellationToken cancellationToken)
        {
            await _eventRepository.UnregisterUserFromEventAsync(eventId, userId, cancellationToken);
        }
        public async Task<string> SaveImageAsync(IFormFile image, HttpRequest request, CancellationToken cancellationToken)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("File is empty.");
            }

            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("The file format is not valid. Allowed: .jpg, .jpeg, .png, .gif");
            }

            var fileName = Guid.NewGuid().ToString() + extension;

            var imageFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            var imagePath = Path.Combine(imageFolder, fileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var imageUrl = $"{request.Scheme}://{request.Host}/images/{fileName}";
            return imageUrl;
        }

    }
}
