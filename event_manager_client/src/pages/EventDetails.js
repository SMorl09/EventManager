import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import './EventDetails.css';

const EventDetails = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [event, setEvent] = useState(null);
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isRegistered, setIsRegistered] = useState(false);
    const [userRole, setUserRole] = useState('');
    const [selectedImage, setSelectedImage] = useState(null);

    useEffect(() => {
        const token = sessionStorage.getItem('token');

        if (!token) {
            navigate('/login');
            return;
        }

        const fetchData = async () => {
            try {
                const [eventRes, usersRes] = await Promise.all([
                    fetch(`/api/events/${id}`, {
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        }
                    }),
                    fetch(`/api/events/${id}/users`, {
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        }
                    })
                ]);

                if (!eventRes.ok) {
                    if (eventRes.status === 401) {
                        navigate('/login');
                    }
                    throw new Error('Ошибка при загрузке деталей события');
                }

                if (!usersRes.ok) {
                    throw new Error('Ошибка при загрузке списка пользователей');
                }

                const eventData = await eventRes.json();
                const usersData = await usersRes.json();

                setEvent(eventData);
                setUsers(usersData);

                const userId = getUserIdFromToken(token);
                const {role}  = getUserInfoFromToken(token);
                setUserRole(role);

                const registered = usersData.some(user => user.id === userId);
                setIsRegistered(registered);

                setLoading(false);
            } catch (error) {
                console.error('Ошибка:', error);
                setLoading(false);
            }
        };

        fetchData();
    }, [id, navigate]);

    const getUserIdFromToken = (token) => {
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            const payload = JSON.parse(jsonPayload);
            return parseInt(payload.nameid);
        } catch (error) {
            console.error('Ошибка при получении userId из токена:', error);
            return null;
        }
    };

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
            return {role: null };
        }
    };

    const handleRegister = async () => {
        const token = sessionStorage.getItem('token');
        const userId = getUserIdFromToken(token);

        try {
            const response = await fetch(`/api/events/${id}/register/${userId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText);
            }

            alert('Вы успешно зарегистрировались на событие!');
            await refreshData();
        } catch (error) {
            console.error('Ошибка:', error);
            alert(`Ошибка: ${error.message}`);
        }
    };

    const handleUnregister = async () => {
        const token = sessionStorage.getItem('token');
        const userId = getUserIdFromToken(token);

        try {
            const response = await fetch(`/api/events/${id}/unregister/${userId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText);
            }

            alert('Вы успешно отменили регистрацию на событие!');
            await refreshData();
        } catch (error) {
            console.error('Ошибка:', error);
            alert(`Ошибка: ${error.message}`);
        }
    };

    const handleImageChange = (event) => {
        setSelectedImage(event.target.files[0]);
    };

    const handleImageUpload = async () => {
        const token = sessionStorage.getItem('token');

        if (!selectedImage) {
            alert('Пожалуйста, выберите изображение.');
            return;
        }

        const formData = new FormData();
        formData.append('Image', selectedImage);

        try {
            const response = await fetch(`/api/events/${id}/image`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`
                },
                body: formData
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText);
            }

            alert('Изображение успешно обновлено!');
            await refreshData();
        } catch (error) {
            console.error('Ошибка при загрузке изображения:', error);
            alert(`Ошибка: ${error.message}`);
        }
    };

    const refreshData = async () => {
        const token = sessionStorage.getItem('token');

        try {
            const [eventRes, usersRes] = await Promise.all([
                fetch(`/api/events/${id}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    }
                }),
                fetch(`/api/events/${id}/users`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`
                    }
                })
            ]);

            if (eventRes.ok && usersRes.ok) {
                const eventData = await eventRes.json();
                const usersData = await usersRes.json();

                setEvent(eventData);
                setUsers(usersData);

                const userId = getUserIdFromToken(token);

                const {role } = getUserInfoFromToken(token);
                setUserRole(role);

                const registered = usersData.some(user => user.id === userId);
                setIsRegistered(registered);
            }
        } catch (error) {
            console.error('Ошибка при обновлении данных:', error);
        }
    };

    if (loading) {
        return <p>Загрузка...</p>;
    }

    if (!event) {
        return <p>Событие не найдено.</p>;
    }

    const canRegister = users.length < event.maxNumberOfUsers && !isRegistered;

    return (
        <div className="event-details-container">
            <h2>{event.title}</h2>
            {event.imageUrl && <img src={event.imageUrl} alt={event.title} />}
            <p><strong>Описание:</strong> {event.description}</p>
            <p><strong>Дата начала:</strong> {event.startDate}</p>
            <p><strong>Категория:</strong> {event.category}</p>
            <p><strong>Максимальное количество участников:</strong> {event.maxNumberOfUsers}</p>

            {canRegister && (
                <button onClick={handleRegister}>Записаться на событие</button>
            )}

            {isRegistered && (
                <button onClick={handleUnregister}>Отменить регистрацию</button>
            )}

            {(!canRegister && !isRegistered && users.length >= event.maxNumberOfUsers) && (
                <p>Достигнуто максимальное количество участников.</p>
            )}
             {userRole === 'Admin' && (
                <div className="admin-image-upload">
                    <input type="file" accept="image/*" onChange={handleImageChange} />
                    <button onClick={handleImageUpload}>Изменить изображение</button>
                </div>
            )}
            <h3>Записавшиеся пользователи:</h3>
            {users.length > 0 ? (
                <ul>
                    {users.map(user => (
                        <li key={user.id}>
                            {user.name} {user.surename} ({user.email})
                        </li>
                    ))}
                </ul>
            ) : (
                <p>Никто не записался на это событие.</p>
            )}
        </div>
    );
};

export default EventDetails;
