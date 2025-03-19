import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './EventsTable.css';

const EventsTable = () => {
    const [events, setEvents] = useState([]);
    const [userRole, setUserRole] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        const fetchEvents = async () => {
            try {
                const token = sessionStorage.getItem('token');
                const response = await fetch('http://localhost:8080/api/events', {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    }
                });

                if (!response.ok) {
                    throw new Error('Ошибка загрузки событий');
                }

                const data = await response.json();
                setEvents(data);

                const { role } = getUserInfoFromToken(token);
                setUserRole(role);
            } catch (error) {
                console.error('Ошибка при загрузке событий:', error);
            }
        };

        fetchEvents();
    }, [navigate]);

    const getUserInfoFromToken = (token) => {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            const payload = JSON.parse(jsonPayload);
            return {
                role: payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
            };
        } catch (error) {
            console.error('Ошибка при получении информации из токена:', error);
            return { role: null };
        }
    };

    return (
        <div className="events-table-container">
            <h2>Список событий</h2>
            {userRole === 'Admin' && (
                <Link to="/create-event" className="create-event-button">Создать событие</Link>
            )}
            <table className="events-table">
                <thead>
                    <tr>
                        <th>Изображение</th>
                        <th>Название</th>
                        <th>Описание</th>
                        <th>Дата начала</th>
                        <th>Категория</th>
                        <th>Макс. кол-во пользователей</th>
                        <th>Действия</th>
                    </tr>
                </thead>
                <tbody>
                    {events.map(event => (
                        <tr key={event.id}>
                            <td>
                                {event.imageUrl ? (
                                    <img src={event.imageUrl} alt={event.title} className="event-image" />
                                ) : (
                                    'Нет изображения'
                                )}
                            </td>
                            <td>{event.title}</td>
                            <td>{event.description}</td>
                            <td>{event.startDate}</td>
                            <td>{event.category}</td>
                            <td>{event.maxNumberOfUsers}</td>
                            <td>
                                <Link to={`/events/${event.id}`}>Перейти</Link>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default EventsTable;
