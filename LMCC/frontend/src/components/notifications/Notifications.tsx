import { useEffect, useState } from 'react';
import './Notifications.css';

interface Notification {
  message: string;
  timestamp: string;
}

function Notifications() {
  const [notifications, setNotifications] = useState([]);

  const fetchNotifications = async () => {
    try {
      const res = await fetch(
        `${import.meta.env.VITE_API_URL}/get-notifications`
      );
      const data = await res.json();
      console.log(data);
      setNotifications(data);
    } catch (err) {
      console.log('Failed to fetch notifications:', err);
    }
  };

  useEffect(() => {
    fetchNotifications();
    // const interval = setInterval(fetchNotifications, 1000);
    // return () => {
    //   clearInterval(interval);
    // };
  }, []);

  return (
    <div className='flex flex-col w-full h-[16.5rem] p-5'>
      {notifications.map((notification: Notification, index) => (
        <div className='notification' key={`notification-${index}`}>
          <span className='notification-msg'>{notification.message}</span>
          <span className='notification-timestamp'>
            {notification.timestamp}
          </span>
        </div>
      ))}
      {/* <div className='notification'>
        <span className='notification-msg'>Begin EVA</span>
        <span className='notification-timestamp'>00:00:00</span>
      </div>
      <div className='notification'>
        <span className='notification-msg'>Begin Navigation</span>
        <span className='notification-timestamp'>00:05:00</span>
      </div>
      <div className='notification'>
        <span className='notification-msg'>Begin Navigation</span>
        <span className='notification-timestamp'>00:05:00</span>
      </div>
      <div className='notification'>
        <span className='notification-msg'>Begin Navigation</span>
        <span className='notification-timestamp'>00:05:00</span>
      </div>
      <div className='notification'>
        <span className='notification-msg'>Begin Navigation</span>
        <span className='notification-timestamp'>00:05:00</span>
      </div>
      <div className='notification'>
        <span className='notification-msg'>Begin Navigation</span>
        <span className='notification-timestamp'>00:05:00</span>
      </div>
      <div className='notification'>
        <span className='notification-msg'>Begin Navigation</span>
        <span className='notification-timestamp'>00:05:00</span>
      </div> */}
      <div className='min-h-2' />
    </div>
  );
}

export default Notifications;
