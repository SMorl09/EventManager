import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './CreateEvent.css';

const CreateEvent = () => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [startDate, setStartDate] = useState('');
    const [category, setCategory] = useState('');
    const [maxNumberOfUsers, setMaxNumberOfUsers] = useState('');
    const [state, setState] = useState('');
    const [city, setCity] = useState('');
    const [street, setStreet] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        const token = sessionStorage.getItem('token');
        
        let address=null;
        if(state.length>0)
        {
                address={
                    state: state,
                    city: city,
                    street: street
                };
        }
        const event = {
            title: title,
            description: description,
            startDate: startDate,
            category: category, 
            maxNumberOfUsers: maxNumberOfUsers,
            address: address
            };
        

        try {
            const response = await fetch('http://localhost:8080/api/events', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify(event)
            });
                
            if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText);
            }
            

            alert('Событие успешно создано!');
            navigate('/events');
        } catch (error) {
            console.error('Ошибка при создании события:', error);
            alert(`Ошибка: ${error.message}`);
        }
    };


    return (
        <div className="create-event-container">
            <h2>Создать новое событие</h2>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label htmlFor="title">Название:</label>
                    <input
                        type="text"
                        id="title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="description">Описание:</label>
                    <textarea
                        id="description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        required
                    ></textarea>
                </div>
                <div className="form-group">
                    <label htmlFor="startDate">Дата начала:</label>
                    <input
                        type="date"
                        id="startDate"
                        value={startDate}
                        onChange={(e) => setStartDate(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="category">Категория:</label>
                    <select
                        id="category"
                        value={category}
                        onChange={(e) => setCategory(e.target.value)}
                        required>
                        <option value="">Выберите категорию</option>
                        <option value="Concert">Concert</option>
                        <option value="Party">Party</option>
                        <option value="Conference">Conference</option>
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="maxNumberOfUsers">Максимальное количество участников:</label>
                    <input
                        type="number"
                        id="maxNumberOfUsers"
                        value={maxNumberOfUsers}
                        onChange={(e) => setMaxNumberOfUsers(e.target.value)}
                        required
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="state">Страна:</label>
                    <input
                        type="text"
                        id="state"
                        value={state}
                        onChange={(e) => setState(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="city">Город:</label>
                    <input
                        type="text"
                        id="city"
                        value={city}
                        onChange={(e) => setCity(e.target.value)}
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="street">Улица:</label>
                    <input
                        type="text"
                        id="street"
                        value={street}
                        onChange={(e) => setStreet(e.target.value)}
                    />
                </div>
                <button type="submit">Создать событие</button>
            </form>
        </div>
    );
};

export default CreateEvent;
