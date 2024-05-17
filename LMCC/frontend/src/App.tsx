import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Monitor1 from './pages/Monitor1';
import Monitor2 from './pages/Monitor2';
import './App.css';

function App() {
  return (
    <div>
      <Router>
        <Routes>
          <Route path='/1' element={<Monitor1 />} />
          <Route path='/2' element={<Monitor2 />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
