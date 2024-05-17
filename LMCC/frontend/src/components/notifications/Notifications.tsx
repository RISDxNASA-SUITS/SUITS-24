import './Notifications.css';

function Notifications() {
  return (
    <div className='flex flex-col w-full h-[11.5rem] p-5'>
      <div className='notification'>
        <span className='notification-msg'>Begin EVA</span>
        <span className='notification-timestamp'>00:00:00</span>
      </div>
      <div className='notification'>
        <span className='notification-msg'>Begin Navigation</span>
        <span className='notification-timestamp'>00:05:00</span>
      </div>
    </div>
  );
}

export default Notifications;
