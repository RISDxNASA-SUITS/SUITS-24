import { useEffect, useState } from 'react';
import './Notifications.css';

interface Notification {
  message: string;
  timestamp: string;
  type: string;
}

function Notifications() {
  const [notifications, setNotifications] = useState([]);

  const fetchNotifications = async () => {
    try {
      const res = await fetch(
        `${import.meta.env.VITE_API_URL}/get-notifications`
      );
      const data = await res.json();
      setNotifications(data);
    } catch (err) {
      console.log('Failed to fetch notifications:', err);
    }
  };

  useEffect(() => {
    fetchNotifications();
    const interval = setInterval(fetchNotifications, 1000);
    return () => {
      clearInterval(interval);
    };
  }, []);

  return (
    <div className='flex flex-col w-full p-5 h-[5rem]'>
      {notifications.map((notification: Notification, index) => (
        <div
          className={`notification${
            notification.type == 'warning' ? ' text-orange-500' : ''
          }`}
          key={`notification-${index}`}
        >
          <span className='notification-msg'>{notification.message}</span>
          <span className='notification-timestamp'>
            {notification.timestamp}
          </span>
        </div>
      ))}
      <div className='min-h-2' />
    </div>
  );
}

export default Notifications;
