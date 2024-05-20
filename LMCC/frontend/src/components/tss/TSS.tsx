// import Temp from '../../assets/symbols/temperature_symbol.png';
// import Battery from '../../assets/symbols/battery_symbol.png';
// import Suit from '../../assets/symbols/suit_symbol.png';
// import Rover from '../../assets/symbols/rover_symbol.svg';
import { useEffect, useState } from 'react';
import './TSS.css';

enum CATEGORY {
  O2 = 'Oxygen',
  CO2 = 'Carbon Dioxide',
  SP = 'Suit Pressure',
  LS = 'Life Support',
  T = 'Temperature',
}

const categories = [
  CATEGORY.O2,
  CATEGORY.CO2,
  CATEGORY.SP,
  CATEGORY.LS,
  CATEGORY.T,
];

function Telemetry() {
  const [tss, setTss] = useState({});
  const [currCategory, setCurrCategory] = useState('Oxygen');

  const fetchTss = async () => {
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/get-tss`);
      // const res = await fetch(
      //   `${import.meta.env.VITE_TSS_URL}/json_data/teams/${
      //     import.meta.env.VITE_TEAM
      //   }/TELEMETRY.json`
      // );
      const data = await res.json();
      console.log(data);
      setTss(data);
    } catch (err) {
      console.log('Failed to fetch TSS:', err);
    }
  };

  useEffect(() => {
    fetchTss();
  }, []);

  const displayTss = (category: string) => {
    switch (category) {
      case 'Oxygen':
        break;
    }
  };

  return (
    <div className='w-full h-full p-5 flex flex-col'>
      <div className='w-full h-5 flex justify-between items-center'>
        <span className='block'>Comm Channel: A</span>
        <span className='block'>Battery: 100</span>
      </div>
      <div className='w-full flex pt-3'>
        <div className='w-1/3'>
          {categories.map((category, index) => (
            <button
              className='tss-category-button'
              key={`tss-btn-${index}`}
              onClick={() => {
                setCurrCategory(category);
              }}
            >
              {category}
            </button>
          ))}
        </div>
        <div className='w-2/3'>{displayTss(currCategory)}</div>
      </div>
      {/* <div className='flex justify-between'>
        <button className='tss-button'>
          <span>
            O<sub>2</sub>
          </span>
        </button>
        <button className='tss-button'>
          <img className='tss-button-img' src={Temp} alt='Temperature'></img>
        </button>
        <button className='tss-button'>
          <img className='tss-button-img' src={Battery} alt='Battery'></img>
        </button>
        <button className='tss-button'>
          <span>
            H<sub>2</sub>O
          </span>
        </button>
        <button className='tss-button'>
          <img className='tss-button-img' src={Suit} alt='Suit'></img>
        </button>
        <button className='tss-button'>
          <span>FAN</span>
        </button>
        <button className='tss-button'>
          <img className='tss-button-img' src={Rover} alt='Rover'></img>
        </button>
        <button className='tss-button tss-button-primary ml-5'>Abort</button>
      </div>
      <div className='table-container'>
        <table>
          <thead>
            <tr>
              <th></th>
              <th>Pressure</th>
              <th>Rate</th>
              <th>Percent</th>
              <th>Time Left</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Primary Oxygen</td>
              <td>750 psia</td>
              <td>0.5 psi/mi</td>
              <td>100%</td>
              <td>10:00:00</td>
            </tr>
            <tr>
              <td>Secondary Oxygen</td>
              <td>750 psia</td>
              <td>0.5 psi/mi</td>
              <td>100%</td>
              <td>10:00:00</td>
            </tr>
          </tbody>
        </table>
      </div> */}
    </div>
  );
}

export default Telemetry;
