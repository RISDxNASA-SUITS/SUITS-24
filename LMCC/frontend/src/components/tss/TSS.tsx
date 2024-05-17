import Temp from '../../assets/symbols/temperature_symbol.png';
import Battery from '../../assets/symbols/battery_symbol.png';
import Suit from '../../assets/symbols/suit_symbol.png';
import Rover from '../../assets/symbols/rover_symbol.svg';
import './TSS.css';

function Telemetry() {
  return (
    <div className='w-full h-full p-5'>
      <div className='flex justify-between'>
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
      </div>
    </div>
  );
}

export default Telemetry;
