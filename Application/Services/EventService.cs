using Application.DTO.Request;
using Application.DTO.Response;
using Application.Interface;
using Domain.Data;
using Domain.Interfaces;
using Domain.Models;
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

        public EventService(IEventRepository eventReposirory)
        {
            _eventRepository = eventReposirory;
        }

        public async Task<IEnumerable<EventResponse>> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEvents();
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
        public async Task<IEnumerable<UserResponse>> GetAllUsers(int id)
        {
            var eventModel = await _eventRepository.GetAllUsers(id);
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
        public async Task<EventResponse> GetEventByIdAsync(int id)
        {
            var eventModel = await _eventRepository.GetEventById(id);
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
        public async Task<EventWithAddressResponse> GetEventWithAddressByIdAsync(int id)
        {
            var eventModel = await _eventRepository.GetEventWithAddress(id);
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
        public async Task<EventResponse> GetEventByNameAsync(string name)
        {
            var eventModel = await _eventRepository.GetEventByName(name);
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
        public async Task<EventWithAddressResponse> CreateEventAsync(EventRequest eventRequest,string imageUrl)
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

            await _eventRepository.AddAsync(eventModel);

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
        public async Task<EventResponse> CreateEventAsync(EventRequest eventRequest)
        {
            var eventModel = new Event
            {
                Title = eventRequest.Title,
                Description = eventRequest.Description,
                StartDate = eventRequest.StartDate,
                Category = Enum.Parse<EventCategory>(eventRequest.Category),
                MaxNumberOfUsers = eventRequest.MaxNumberOfUsers
            };

            await _eventRepository.AddAsync(eventModel);

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
        public async Task<EventWithAddressResponse> CreateEventWithAddressAsync(EventRequest eventRequest)
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

            await _eventRepository.AddAsync(eventModel);
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
        public async Task UpdateEventAsync(int id, EventRequest eventRequest)
        {
            var eventModel = await _eventRepository.GetEventById(id);
            if (eventModel == null)
                throw new Exception("Event not found.");

            eventModel.Title = eventRequest.Title;
            eventModel.Description = eventRequest.Description;
            eventModel.StartDate = eventRequest.StartDate;
            eventModel.Category = Enum.Parse<EventCategory>(eventRequest.Category);
            eventModel.MaxNumberOfUsers = eventRequest.MaxNumberOfUsers;

            await _eventRepository.UpdateAsync(eventModel);
        }
        public async Task UpdateEventImageAsync(int id, string imageUrl)
        {
            var eventModel = await _eventRepository.GetEventById(id);
            if (eventModel == null)
                throw new Exception("Event not found.");

            eventModel.ImageUrl = imageUrl;

            await _eventRepository.UpdateAsync(eventModel);
        }
        public async Task DeleteEventAsync(int id)
        {
            await _eventRepository.DeleteAsync(id);
        }
        public async Task RegisterUserToEventAsync(int eventId, int userId)
        {

            await _eventRepository.RegisterUserToEventAsync(eventId, userId);
        }

        public async Task UnregisterUserFromEventAsync(int eventId, int userId)
        {
            await _eventRepository.UnregisterUserFromEventAsync(eventId, userId);
        }

    }
}
