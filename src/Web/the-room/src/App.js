import './App.css';

function App() {
  return (
    <div className="">
      <div className="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
          <h1 className="h2">Dashboard</h1>
      </div>

      <form className="row g-3">
        <div className="col-auto">
          <label for="filter" className="visually-hidden">Password</label>
          <input type="text" className="form-control" id="filter" placeholder="Password" />
        </div>
        <div className="col-auto">
          <button type="submit" className="btn btn-outline-dark mb-3">Reset</button>
        </div>
      </form>

      <div className="services">
        
      </div>
    </div>
  );
}

export default App;
