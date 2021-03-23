import AuthGuard from '../shared/services/auth.guard'
import React from 'react';

const HandleAuthorization = (props) => {
    let authGuard = new AuthGuard(process.env.REACT_APP_IDENTITY_URL, window.location, props.history);
    if (authGuard.IsAuthorized === 'false' || authGuard.IsAuthorized === null || authGuard.IsAuthorized === undefined) {    
        authGuard.authorize();
    }
};

const NoAuthorized = (props) => {
    return <React.Fragment>
                <div className="col-md-6">
                    <div className="d-flex">
                        <div className="d-flex justify-content-center">
                            <div className="card text-center">
                                <div className="card-body">
                                    <h2>You are not authorized</h2>
                                    <button onClick={() => HandleAuthorization(props)} className="btn btn-primary btn-block">Get Access Now</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </React.Fragment>;
};

NoAuthorized.defaultProps = {
};
export default NoAuthorized;