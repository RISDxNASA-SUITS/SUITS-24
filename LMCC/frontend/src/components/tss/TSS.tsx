import { useEffect, useState } from 'react';
import { TSS } from './TSSTypes';
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

const categoryFields = {
  [CATEGORY.O2]: [
    { name: 'oxy_pri_storage', label: 'Pri O2 Storage' },
    { name: 'oxy_pri_pressure', label: 'Pri O2 Pressure' },
    { name: 'oxy_sec_storage', label: 'Sec O2 Storage' },
    { name: 'oxy_sec_pressure', label: 'Sec O2 Pressure' },
  ],
  [CATEGORY.CO2]: [
    { name: 'co2_production', label: 'CO2 Production' },
    { name: 'helmet_pressure_co2', label: 'Helmet Pressure' },
    { name: 'scrubber_a_co2_storage', label: 'Scrubber A Storage' },
    { name: 'scrubber_b_co2_storage', label: 'Scrubber B Storage' },
  ],
  [CATEGORY.SP]: [
    { name: 'suit_pressure_oxy', label: 'O2' },
    { name: 'suit_pressure_co2', label: 'CO2' },
    { name: 'suit_pressure_other', label: 'Other' },
    { name: 'suit_pressure_total', label: 'Total' },
  ],
  [CATEGORY.LS]: [
    { name: 'heart_rate', label: 'Heart Rate' },
    { name: 'oxy_consumption', label: 'O2 Consumption' },
    { name: 'fan_pri_rpm', label: 'Pri Fan RPM' },
    { name: 'fan_sec_rpm', label: 'Sec Fan RPM' },
  ],
  [CATEGORY.T]: [
    { name: 'temperature', label: 'Temperature' },
    { name: 'coolant_ml', label: 'Coolant Tank' },
    { name: 'coolant_gas_pressure', label: 'Gas Pressure' },
    { name: 'coolant_liquid_pressure', label: 'Liquid Pressure' },
  ],
};

function Telemetry() {
  const [tss, setTss] = useState<TSS | null>(null);
  const [currCategory, setCurrCategory] = useState('Oxygen');

  const fetchTss = async () => {
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/get-tss`, {
        method: 'GET',
      });
      const data = await res.json();
      setTss(data.telemetry[`eva${import.meta.env.VITE_EVA_NUM}`]);
    } catch (err) {
      console.log('Failed to fetch TSS:', err);
    }
  };

  useEffect(() => {
    fetchTss();
    const interval = setInterval(fetchTss, 1000);
    return () => {
      clearInterval(interval);
    };
  }, []);

  return (
    <div className='w-full h-full p-5 flex flex-col'>
      <div className='w-full h-5 flex justify-between items-center pt-3'>
        <span className='block flex items-center'>
          Battery Time: {tss == null ? '---' : tss['batt_time_left'].toFixed(2)}
        </span>
        <button className='tss-category-button tss-category-button-primary ml-5'>
          Abort
        </button>
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
        <div className='w-2/3'>
          <div className='table-container'>
            <table>
              <tbody>
                {categoryFields[currCategory].map((item, index: number) => (
                  <tr key={`tss-table-tr-${index}`}>
                    <td>{item.label}</td>
                    {tss == null ? (
                      <td />
                    ) : (
                      <td className='text-right'>{tss[item.name]}</td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
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
